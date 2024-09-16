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
                using (var output = new WaveOutEvent() { DeviceNumber = AudioHandler.GetDeviceIndex(Properties.Core.Default.AudioDevice) - 1 })
                {
                    output.Init(file);
                    output.Volume = Properties.Core.Default.AudioVolume;
                    output.Play();

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
            // Indexing starts with default device as -1
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                try
                {
                    var deviceName = WaveOut.GetCapabilities(n).ProductName;
                    if (deviceName == "Microsoft Sound Mapper")
                        deviceName = "Default Audio Device";
                    devices.Add(deviceName); 
                }
                catch
                {
                    // -1 potentially not present, ignore and continue
                }
            }
                
            return devices;
        }

        public static int GetDeviceIndex(string deviceName) => GetDevices().IndexOf(deviceName);

        public static string GetDeviceName(int deviceIndex)
        {
            var devices = GetDevices();
            if (devices.Count <= deviceIndex)
                return devices.FirstOrDefault(string.Empty);
            else
                return devices[deviceIndex];
        }

        public static string GetFirstDevice() => GetDevices().FirstOrDefault(string.Empty);
    }

    internal class AudioTaskData
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public AudioOptions Options { get; set; }
    }
}
