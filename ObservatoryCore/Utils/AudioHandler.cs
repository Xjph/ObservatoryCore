using NAudio.Wave;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.ParameterTypes;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Observatory.Utils
{
    internal class AudioHandler : ConcurrentQueue<AudioTaskData>
    {
        private bool processingQueue = false;

        private ConcurrentDictionary<Guid, AudioTaskData> audioTasks = new();

        internal Task EnqueueAndPlay(string filePath, AudioOptions options)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Debug.WriteLine($"Attempted to enqueue and play a empty or non-existant file: {filePath}; (instantly? {options.Instant}");
                return Task.CompletedTask;
            }

            if (!options.Instant)
            {
                AudioTaskData taskData = new()
                {
                    Id = Guid.NewGuid(),
                    FilePath = filePath,
                    Options = options,
                };
                audioTasks.TryAdd(taskData.Id, taskData);
                Enqueue(taskData);
                return Task.Run(() =>
                {
                    try
                    {
                        if (!processingQueue)
                        {
                            processingQueue = true;
                            while (TryDequeue(out AudioTaskData audioTask))
                            {
                                PlayAudioFile(audioTask);
                            }

                            processingQueue = false;
                        }
                        else
                        {
                            while (audioTasks.ContainsKey(taskData.Id))
                            {
                                Thread.Sleep(250);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorReporter.ShowErrorPopup("Audio Playback Error (queued)", [(ex.Message, ex.StackTrace ?? string.Empty)]);
                    }
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    PlayAudioFile(new()
                    {
                        Id = Guid.Empty,
                        FilePath = filePath,
                        Options = options,
                    });
                });
            }
        }

        private void PlayAudioFile(AudioTaskData audioTask)
        {
            try
            {
                if (!File.Exists(audioTask.FilePath) || new FileInfo(audioTask.FilePath).Length == 0)
                    return;

                using (var file = new AudioFileReader(audioTask.FilePath))
                using (var output = new WaveOutEvent() { DeviceNumber = AudioHandler.GetDeviceIndex(Properties.Core.Default.AudioDevice) })
                {
                    output.Init(file);
                    output.Play();
                    output.Volume = Properties.Core.Default.AudioVolume;

                    while (output.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(250);
                    }
                    if (!audioTask.Options.Instant && audioTask.Id != Guid.Empty) // Ignore "instant" plays which weren't tracked.
                        audioTasks.TryRemove(new(audioTask.Id, audioTask));

                    if (audioTask.Options.DeleteAfterPlay)
                    {
                        file.Close(); // Ensure the file is not open before attempting to delete it.
                        try
                        {
                            File.Delete(audioTask.FilePath);
                        }
                        catch (Exception deleteEx)
                        {
                            // Ignore for now. Yes, this will result in some files being leaked.
                            // TODO: Clean up on app close?
                            Debug.WriteLine($"Unable to clean up file {audioTask.FilePath} due to error: {deleteEx.Message}");
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                ErrorReporter.ShowErrorPopup("Audio Playback Error", [(ex.Message, ex.StackTrace ?? string.Empty)]);
            }
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

    internal class AudioTaskData
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public AudioOptions Options { get; set; }
    }
}
