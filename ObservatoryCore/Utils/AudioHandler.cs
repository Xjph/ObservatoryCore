using NAudio.Wave;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace Observatory.Utils
{
    internal class AudioHandler : ConcurrentQueue<KeyValuePair<Guid, string>>
    {
        private bool processingQueue = false;
        
        private List<Guid> audioTasks = [];

        internal Task EnqueueAndPlay(string filePath, bool instant = false)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Debug.WriteLine($"Attempted to enqueue and play a empty or non-existant file: {filePath}; (instantly? {instant}");
                return Task.CompletedTask;
            }

            if (!instant)
            {
                Guid thisTask = Guid.NewGuid();
                audioTasks.Add(thisTask);
                Enqueue(new(thisTask, filePath));
                return Task.Run(() =>
                {
                    try
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
                    }
                    catch (Exception ex)
                    {
                        ErrorReporter.ShowErrorPopup("Audio Playback Error (for instant playback)", [(ex.Message, ex.StackTrace ?? string.Empty)]);
                    }
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    try
                    {
                        if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
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
                try
                {
                    if (File.Exists(audioTask.Value) && new FileInfo(audioTask.Value).Length > 0)
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
                        };
                    }
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
