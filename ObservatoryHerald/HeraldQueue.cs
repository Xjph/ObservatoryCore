﻿using Observatory.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using NetCoreAudio;
using System.Threading;
using System;
using System.Diagnostics;

namespace Observatory.Herald
{
    class HeraldQueue
    {
        private Queue<NotificationArgs> notifications;
        private bool processing;
        private string voice;
        private string style;
        private string rate;
        private byte volume;
        private SpeechRequestManager speechManager;
        private Player audioPlayer;
        private Action<Exception, String> ErrorLogger;

        public HeraldQueue(SpeechRequestManager speechManager, Action<Exception, String> errorLogger)
        {
            this.speechManager = speechManager;
            processing = false;
            notifications = new();
            audioPlayer = new();
            ErrorLogger = errorLogger;
        }


        internal void Enqueue(NotificationArgs notification, string selectedVoice, string selectedStyle = "", string selectedRate = "", int volume = 75)
        {
            voice = selectedVoice;
            style = selectedStyle;
            rate = selectedRate;
            // Ignore invalid values; assume default.
            volume = volume >= 0 && volume <= 100 ? volume : 75;

            // Volume is perceived logarithmically, convert to exponential curve
            // to make perceived volume more in line with value set.
            this.volume = ((byte)System.Math.Floor(System.Math.Pow(volume / 100.0, 2.0) * 100));

            if (!notifications.Where(n => n.Title.Trim().ToLower() == notification.Title.Trim().ToLower()).Any())
                notifications.Enqueue(notification);

            if (!processing)
            {
                processing = true;
                ProcessQueueAsync();
            }
        }

        private async void ProcessQueueAsync()
        {
            await Task.Factory.StartNew(ProcessQueue);
        }

        private void ProcessQueue()
        {

            NotificationArgs notification = null;
            try
            {
                while (notifications.Any())
                {
                    audioPlayer.SetVolume(volume).Wait();
                    notification = notifications.Dequeue();
                    Debug.WriteLine("Processing notification: {0} - {1}", notification.Title, notification.Detail);

                    Task<string>[] audioRequestTasks = new Task<string>[2];

                    if (string.IsNullOrWhiteSpace(notification.TitleSsml))
                    {
                        audioRequestTasks[0] = RetrieveAudioToFile(notification.Title);
                    }
                    else
                    {
                        audioRequestTasks[0] = RetrieveAudioSsmlToFile(notification.TitleSsml);
                    }

                    if (string.IsNullOrWhiteSpace(notification.DetailSsml))
                    {
                        audioRequestTasks[1] = RetrieveAudioToFile(notification.Detail);
                    }
                    else
                    {
                        audioRequestTasks[1] = RetrieveAudioSsmlToFile(notification.DetailSsml);
                    }

                    PlayAudioRequestsSequentially(audioRequestTasks);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to fetch/play notification: {notification?.Title} - {notification?.Detail}");
                ErrorLogger(ex, "while retrieving and playing audio for a notification");
            }
            finally
            {
                processing = false;
            }
        }

        private async Task<string> RetrieveAudioToFile(string text)
        {
            return await RetrieveAudioSsmlToFile($"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">{System.Security.SecurityElement.Escape(text)}</voice></speak>");
        }

        private async Task<string> RetrieveAudioSsmlToFile(string ssml)
        {
            return await speechManager.GetAudioFileFromSsml(ssml, voice, style, rate);
        }

        private void PlayAudioRequestsSequentially(Task<string>[] requestTasks)
        {
            foreach (var request in requestTasks)
            {
                string file = request.Result;
                try
                {
                    Debug.WriteLine($"Playing audio file: {file}");
                    audioPlayer.Play(file).Wait();
                } catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to play {file}: {ex.Message}");
                    ErrorLogger(ex, $"while playing: {file}");
                }

                while (audioPlayer.Playing)
                    Thread.Sleep(50);

                // Explicit stop to ensure device is ready for next file.
                // ...hopefully.
                audioPlayer.Stop(true).Wait();

            }
            speechManager.CommitCache();
        }
    }
}
