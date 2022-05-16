﻿using NetCoreAudio.Interfaces;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NetCoreAudio.Players
{
    internal class WindowsPlayer : IPlayer
    {
        [DllImport("winmm.dll")]
        private static extern int mciSendString(string command, StringBuilder stringReturn, int returnLength, IntPtr hwndCallback);

		[DllImport("winmm.dll")]
		private static extern int mciGetErrorString(int errorCode, StringBuilder errorText, int errorTextSize);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

		private Timer _playbackTimer;
        private Stopwatch _playStopwatch;

		private string _fileName;

        public event EventHandler PlaybackFinished;

        public bool Playing { get; private set; }
        public bool Paused { get; private set; }

        public Task Play(string fileName)
        {
            _fileName = $"\"{fileName}\"";
            _playbackTimer = new Timer
            {
                AutoReset = false
            };
            _playStopwatch = new Stopwatch();
            
            ExecuteMciCommand($"Status {_fileName} Length");
            ExecuteMciCommand($"Play {_fileName}");
            Paused = false;
            Playing = true;
            _playbackTimer.Elapsed += HandlePlaybackFinished;
            _playbackTimer.Start();
            _playStopwatch.Start();

            return Task.CompletedTask;
        }

        public Task Pause()
        {
            if (Playing && !Paused)
            {
                ExecuteMciCommand($"Pause {_fileName}");
                Paused = true;
                _playbackTimer.Stop();
                _playStopwatch.Stop();
                _playbackTimer.Interval -= _playStopwatch.ElapsedMilliseconds;
            }

            return Task.CompletedTask;
        }

        public Task Resume()
        {
            if (Playing && Paused)
            {
                ExecuteMciCommand($"Resume {_fileName}");
                Paused = false;
                _playbackTimer.Start();
                _playStopwatch.Reset();
                _playStopwatch.Start();
            }
            return Task.CompletedTask;
        }

        public Task Stop(bool force = false)
        {
            if (Playing || force)
            {
                ExecuteMciCommand($"Stop {_fileName}");
				Playing = false;
                Paused = false;
                _playbackTimer?.Stop();
                _playStopwatch?.Stop();
            }
            return Task.CompletedTask;
        }

		private void HandlePlaybackFinished(object sender, ElapsedEventArgs e)
        {
            Playing = false;
            PlaybackFinished?.Invoke(this, e);
            _playbackTimer?.Dispose();
            _playbackTimer = null;
        }

        private Task ExecuteMciCommand(string commandString)
        {
            var sb = new StringBuilder();

            var result = mciSendString(commandString, sb, 1024 * 1024, IntPtr.Zero);

            if (result != 0)
            {
				var errorSb = new StringBuilder($"Error executing MCI command '{commandString}'. Error code: {result}.");
				var sb2 = new StringBuilder(128);

				mciGetErrorString(result, sb2, 128);
				errorSb.Append($" Message: {sb2}");

				throw new Exception(errorSb.ToString());
            }

            if (commandString.ToLower().StartsWith("status") && int.TryParse(sb.ToString(), out var length))
                _playbackTimer.Interval = length + 75;

            return Task.CompletedTask;
        }

        public Task SetVolume(byte percent)
        {
            // Calculate the volume that's being set
            int NewVolume = ushort.MaxValue / 100 * percent;
            // Set the same volume for both the left and the right channels
            uint NewVolumeAllChannels = ((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16);
            // Set the volume
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Stop().Wait();
            ExecuteMciCommand("Close All");
        }
	}
}
