﻿using NAudio.Wave;
using System.Collections.Concurrent;
using System.IO;

namespace Observatory.Utils
{
    internal class AudioHandler : ConcurrentQueue<KeyValuePair<Guid, string>>
    {
        private bool processingQueue = false;
        
        private List<Guid> audioTasks = [];

        internal Task EnqueueAndPlay(string filePath, bool instant = false)
        {
            if (!instant)
            {
                Guid thisTask = Guid.NewGuid();
                audioTasks.Add(thisTask);
                Enqueue(new(thisTask, filePath));
                return Task.Run(() =>
                {
                    if (!processingQueue)
                    {
                        processingQueue = true;
                        ProcessQueue();
                    }
                    else
                    {
                        while (audioTasks.Contains(thisTask))
                        {
                            Thread.Sleep(250);
                        }
                    }
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    if (new FileInfo(filePath).Length > 0)
                        try
                        {
                            using (var file = new AudioFileReader(filePath))
                            using (var output = new WaveOutEvent() { DeviceNumber = AudioHandler.GetDeviceIndex(Properties.Core.Default.AudioDevice) })
                            {
                                output.Init(file);
                                output.Play();
                                output.Volume = Properties.Core.Default.AudioVolume;

                                while (output.PlaybackState == PlaybackState.Playing)
                                {
                                    Thread.Sleep(250);
                                }
                            };
                        }
                        catch (Exception ex)
                        {
                            ErrorReporter.ShowErrorPopup("Audio Playback Error", [(ex.Message, ex.StackTrace ?? string.Empty)]);
                        }
                });
            }
        }

        private void ProcessQueue()
        {
            while (TryDequeue(out KeyValuePair<Guid, string> audioTask))
            {
                if (new FileInfo(audioTask.Value).Length > 0)
                try
                {
                    using (var file = new AudioFileReader(audioTask.Value))
                    using (var output = new WaveOutEvent(){ DeviceNumber = AudioHandler.GetDeviceIndex(Properties.Core.Default.AudioDevice) })
                    {
                        output.Init(file);
                        output.Play();
                        output.Volume = Properties.Core.Default.AudioVolume;

                        while (output.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(250);
                        }
                        audioTasks.Remove(audioTask.Key);
                        file.Close();
                        File.Delete(audioTask.Value);
                    };
                }
                catch (Exception ex)
                {
                    ErrorReporter.ShowErrorPopup("Audio Playback Error", [(ex.Message,ex.StackTrace??string.Empty)]);
                }
            }
           
            processingQueue = false;
        }

        public static List<string> GetDevices()
        {
            List<string> devices = new();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
                devices.Add(WaveOut.GetCapabilities(n).ProductName); // Index will be offset by 1 due to the default device being -1
            return devices;
        }
        public static int GetDeviceIndex(string deviceName)
        {
            for (int n = -1; n < WaveOut.DeviceCount; n++)
                if (WaveOut.GetCapabilities(n).ProductName == deviceName)
                    return n;
            return -1;
        }
        public static string GetDeviceName(int deviceIndex)
        {
            if (!(-1 <= deviceIndex && deviceIndex < WaveOut.DeviceCount)) // If the device index is out of range
                deviceIndex = -1; // Set to default device
            return WaveOut.GetCapabilities(deviceIndex).ProductName;
        }
    }
}
