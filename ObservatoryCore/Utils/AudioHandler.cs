using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Observatory.Utils
{
    internal static class AudioHandler
    {
        internal static async Task PlayFile(string filePath)
        {
            await Task.Run(() =>
            {
                using (var file = new AudioFileReader(filePath))
                using (var output = new WaveOutEvent())
                {
                    output.Init(file);
                    output.Play();

                    while (output.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(250);
                    }
                };
            });
        }
    }
}
