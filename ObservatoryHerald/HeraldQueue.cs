using Observatory.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System;
using System.Diagnostics;
using Observatory.Framework.Interfaces;

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
        private Action<Exception, String> ErrorLogger;
        private IObservatoryCore core;

        public HeraldQueue(SpeechRequestManager speechManager, Action<Exception, String> errorLogger, IObservatoryCore core)
        {
            this.speechManager = speechManager;
            this.core = core;
            processing = false;
            notifications = new();
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

            Debug.WriteLine("Attempting to de-dupe notification titles against '{0}': '{1}'",
                notification.Title.Trim().ToLower(),
                String.Join(',', notifications.Select(n => n.Title.Trim().ToLower())));

            if (notifications.Where(n => n.Title.Trim().ToLower() == notification.Title.Trim().ToLower()).Any())
            {
                // Suppress title.
                notification.Suppression |= NotificationSuppression.Title;
            }
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
            Thread.Sleep(200); // Allow time for other notifications to arrive.
            NotificationArgs notification = null;
            try
            {
                while (notifications.Any())
                {
                    // audioPlayer.SetVolume(volume).Wait();
                    notification = notifications.Dequeue();
                    Debug.WriteLine("Processing notification: {0} - {1}", notification.Title, notification.Detail);

                    List<Task<string>> audioRequestTasks = new();

                    if (!notification.Suppression.HasFlag(NotificationSuppression.Title))
                    {
                        audioRequestTasks.Add(string.IsNullOrWhiteSpace(notification.TitleSsml)
                            ? RetrieveAudioToFile(notification.Title)
                            : RetrieveAudioSsmlToFile(notification.TitleSsml));
                    }

                    if (!notification.Suppression.HasFlag(NotificationSuppression.Detail))
                    {
                        audioRequestTasks.Add(string.IsNullOrWhiteSpace(notification.DetailSsml)
                            ? RetrieveAudioToFile(notification.Detail)
                            : RetrieveAudioSsmlToFile(notification.DetailSsml));
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

        private async void PlayAudioRequestsSequentially(List<Task<string>> requestTasks)
        {
            foreach (var request in requestTasks)
            {
                string file = request.Result;
                try
                {
                    Debug.WriteLine($"Playing audio file: {file}");
                    await core.PlayAudioFile(file);
                    // audioPlayer.Play(file).Wait();
                } catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to play {file}: {ex.Message}");
                    ErrorLogger(ex, $"while playing: {file}");
                }

                //while (audioPlayer.Playing)
                //    Thread.Sleep(50);

                //// Explicit stop to ensure device is ready for next file.
                //// ...hopefully.
                //audioPlayer.Stop(true).Wait();

            }
            speechManager.CommitCache();
        }
    }
}
