using NAudio.Wave;
using System.Collections.Concurrent;

namespace Observatory.Utils
{
    internal class AudioHandler : ConcurrentQueue<KeyValuePair<Guid, string>>
    {
        private bool processingQueue = false;
        
        private List<Guid> audioTasks = [];

        internal Task EnqueueAndPlay(string filePath)
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

        private void ProcessQueue()
        {
            while (TryDequeue(out KeyValuePair<Guid, string> audioTask))
            {
                using (var file = new AudioFileReader(audioTask.Value))
                using (var output = new WaveOutEvent())
                {
                    output.Init(file);
                    output.Play();

                    while (output.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(250);
                    }
                    audioTasks.Remove(audioTask.Key);
                };
            }
           
            processingQueue = false;
        }
    }
}
