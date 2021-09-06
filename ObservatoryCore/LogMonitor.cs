using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Observatory.Framework;
using Observatory.Framework.Files;

namespace Observatory
{
    class LogMonitor
    {
        #region Singleton Instantiation

        public static LogMonitor GetInstance
        {
            get
            {
                return _instance.Value;
            }
        }

        private static readonly Lazy<LogMonitor> _instance = new Lazy<LogMonitor>(NewLogMonitor);

        private static LogMonitor NewLogMonitor()
        {
            return new LogMonitor();
        }

        private LogMonitor()
        {
            currentLine = new();
            journalTypes = JournalReader.PopulateEventClasses();
            InitializeWatchers(string.Empty);
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            if (firstStartMonitor)
            {
                // Only pre-read on first start monitor. Beyond that it's simply pause/resume.
                firstStartMonitor = false;
                PrereadJournals();
            }
            journalWatcher.EnableRaisingEvents = true;
            statusWatcher.EnableRaisingEvents = true;
            monitoring = true;
            JournalPoke();
        }

        public void Stop()
        {
            journalWatcher.EnableRaisingEvents = false;
            statusWatcher.EnableRaisingEvents = false;
            monitoring = false;
        }

        public void ChangeWatchedDirectory(string path)
        {
            journalWatcher.Dispose();
            statusWatcher.Dispose();
            InitializeWatchers(path);
        }

        public bool IsMonitoring()
        {
            return monitoring;
        }

        public bool ReadAllInProgress()
        {
            return readall;
        }

        public void ReadAllJournals()
        {
            ReadAllJournals(string.Empty);
        }

        public void ReadAllJournals(string path)
        {
            // Prevent pre-reading when starting monitoring after reading all.
            firstStartMonitor = false;
            readall = true;
            DirectoryInfo logDirectory = GetJournalFolder(path);
            var files = logDirectory.GetFiles("Journal.????????????.??.log");
            var readErrors = new List<(Exception ex, string file, string line)>();
            foreach (var file in files)
            {
                readErrors.AddRange(
                    ProcessLines(ReadAllLines(file.FullName), file.Name));
            }

            ReportErrors(readErrors);
            readall = false;
        }

        public void PrereadJournals()
        {
            if (!Properties.Core.Default.TryPrimeSystemContextOnStartMonitor) return;

            DirectoryInfo logDirectory = GetJournalFolder(Properties.Core.Default.JournalFolder);
            var files = logDirectory.GetFiles("Journal.????????????.??.log");

            // Read at most the last two files (in case we were launched after the game and the latest
            // journal is mostly empty) but keeping only the lines since the last FSDJump.
            List<String> lastSystemLines = new();
            string lastLoadGame = String.Empty;
            bool sawFSDJump = false;
            foreach (var file in files.Skip(Math.Max(files.Length - 2, 0)))
            {
                var lines = ReadAllLines(file.FullName);
                foreach (var line in lines)
                {
                    var eventType = JournalUtilities.GetEventType(line);
                    if (eventType.Equals("FSDJump"))
                    {
                        // Reset, start collecting again.
                        lastSystemLines.Clear();
                        sawFSDJump = true;
                    }
                    else if (eventType.Equals("LoadGame"))
                    {
                        lastLoadGame = line;
                    }
                    lastSystemLines.Add(line);
                }
            }

            // So we didn't see a jump in the recent logs. We could be re-logging, or something.
            // Just bail on this attempt.
            if (!sawFSDJump) return;

            // If we saw a LoadGame, insert it as well. This ensures odyssey biologicials are properly
            // counted/presented.
            if (!String.IsNullOrEmpty(lastLoadGame))
            {
                lastSystemLines.Insert(0, lastLoadGame);
            }

            // We found an FSD jump, buffered the lines for that system (possibly including startup logs
            // over a file boundary). Pump these through the plugins.
            readall = true;
            ReportErrors(ProcessLines(lastSystemLines, "Pre-read"));
            readall = false;
        }

        #endregion

        #region Public Events

        public event EventHandler<JournalEventArgs> JournalEntry;

        public event EventHandler<JournalEventArgs> StatusUpdate;

        #endregion

        #region Private Fields

        private FileSystemWatcher journalWatcher;
        private FileSystemWatcher statusWatcher;
        private Dictionary<string, Type> journalTypes;
        private Dictionary<string, int> currentLine;
        private bool monitoring = false;
        private bool readall = false;
        private bool firstStartMonitor = true;

        #endregion

        #region Private Methods

        private void InitializeWatchers(string path)
        {
            DirectoryInfo logDirectory = GetJournalFolder(path);

            journalWatcher = new FileSystemWatcher(logDirectory.FullName, "Journal.????????????.??.log")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size |
                                NotifyFilters.FileName | NotifyFilters.CreationTime
            };
            journalWatcher.Changed += LogChangedEvent;
            journalWatcher.Created += LogCreatedEvent;

            statusWatcher = new FileSystemWatcher(logDirectory.FullName, "Status.json")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            statusWatcher.Changed += StatusUpdateEvent;
        }

        private DirectoryInfo GetJournalFolder(string path = "")
        {
            DirectoryInfo logDirectory;

            if (path.Length == 0 && Properties.Core.Default.JournalFolder.Trim().Length > 0)
            {
                path = Properties.Core.Default.JournalFolder;
            }

            if (path.Length > 0)
            {
                if (Directory.Exists(path))
                {
                    logDirectory = new DirectoryInfo(path);
                }
                else
                {
                    //throw new DirectoryNotFoundException($"Directory '{path}' does not exist.");
                    //Don't throw, not handling that right now. Just set to current folder.
                    logDirectory = new DirectoryInfo(".");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logDirectory = new DirectoryInfo(GetSavedGamesPath() + @"\Frontier Developments\Elite Dangerous");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                logDirectory = new DirectoryInfo(".");
            }
            else
            {
                throw new NotImplementedException("Current OS Platform Not Supported.");
            }

            Properties.Core.Default.JournalFolder = path;
            Properties.Core.Default.Save();

            return logDirectory;
        }

        private List<(Exception ex, string file, string line)> ProcessLines(List<String> lines, string file)
        {
            var readErrors = new List<(Exception ex, string file, string line)>();
            foreach (var line in lines)
            {
                try
                {
                    DeserializeAndInvoke(line);
                }
                catch (Exception ex)
                {
                    readErrors.Add((ex, file, line));
                }
            }
            return readErrors;
        }

        private void DeserializeAndInvoke(string line)
        {
            var eventType = JournalUtilities.GetEventType(line);
            if (!journalTypes.ContainsKey(eventType))
            {
                eventType = "JournalBase";
            }

            var eventClass = journalTypes[eventType];
            MethodInfo journalRead = typeof(JournalReader).GetMethod(nameof(JournalReader.ObservatoryDeserializer));
            MethodInfo journalGeneric = journalRead.MakeGenericMethod(eventClass);
            object entry = journalGeneric.Invoke(null, new object[] { line });
            var journalEvent = new JournalEventArgs() { journalType = eventClass, journalEvent = entry };
            var handler = JournalEntry;

            handler?.Invoke(this, journalEvent);

        }

        private void ReportErrors(List<(Exception ex, string file, string line)> readErrors)
        {
            if (readErrors.Any())
            {
                var errorContent = new System.Text.StringBuilder();
                int count = 0;
                foreach (var error in readErrors)
                {

                    if (error.ex.InnerException == null)
                    {
                        errorContent.AppendLine(error.ex.Message);
                    }
                    else
                    {
                        errorContent.AppendLine(error.ex.InnerException.Message);
                    }
                    
                    errorContent.AppendLine($"File: {error.file}");
                    if (error.line.Length > 200)
                    {
                        errorContent.AppendLine($"Line (first 200 chars): {error.line.Substring(0, 200)}");
                    }
                    else
                    {
                        errorContent.AppendLine($"Line: {error.line}");
                    }

                    if (error != readErrors.Last())
                    {
                        errorContent.AppendLine();
                        if (count++ == 5)
                        {
                            errorContent.AppendLine($"There are {readErrors.Count - 6} more errors but let's keep this window manageable.");
                            break;
                        }
                    }
                }

                if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var errorMessage = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow(new MessageBox.Avalonia.DTO.MessageBoxStandardParams
                    {
                        ContentTitle = $"Journal Read Error{(readErrors.Count > 1 ? "s" : "")}",
                        ContentMessage = errorContent.ToString()
                    });
                    errorMessage.ShowDialog(desktop.MainWindow);

                }

            }
        }

        private void LogChangedEvent(object source, FileSystemEventArgs eventArgs)
        {
            var fileContent = ReadAllLines(eventArgs.FullPath);

            if (!currentLine.ContainsKey(eventArgs.FullPath))
            {
                currentLine.Add(eventArgs.FullPath, fileContent.Count - 1);
            }

            foreach (string line in fileContent.Skip(currentLine[eventArgs.FullPath]))
            {
                DeserializeAndInvoke(line);
            }

            currentLine[eventArgs.FullPath] = fileContent.Count;
        }

        private List<string> ReadAllLines(string path)
        {
            var lines = new List<string>();
            using (StreamReader file = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                while (!file.EndOfStream)
                {
                    lines.Add(file.ReadLine());
                }
            }
            return lines;
        }

        private void LogCreatedEvent(object source, FileSystemEventArgs eventArgs)
        {
            currentLine.Add(eventArgs.FullPath, 0);
            LogChangedEvent(source, eventArgs);
        }

        private void StatusUpdateEvent(object source, FileSystemEventArgs eventArgs)
        {
            var handler = StatusUpdate;
            var statusLines = ReadAllLines(eventArgs.FullPath);
            if (statusLines.Count > 0)
            {
                var status = JournalReader.ObservatoryDeserializer<Status>(statusLines[0]);
                handler?.Invoke(this, new JournalEventArgs() { journalType = typeof(Status), journalEvent = status });
            }
        }

        /// <summary>
        /// Touches most recent journal file once every 250ms while LogMonitor is monitoring.
        /// Forces pending file writes to flush to disk and fires change events for new journal lines.
        /// </summary>
        private async void JournalPoke()
        {
            await System.Threading.Tasks.Task.Run(() => 
            { 
                while (monitoring)
                {
                    var journalFolder = GetJournalFolder();
                    FileInfo fileToPoke = null;

                    foreach (var file in journalFolder.GetFiles("Journal.????????????.??.log"))
                    {
                        if (fileToPoke == null || string.Compare(file.Name, fileToPoke.Name) > 0)
                        {
                            fileToPoke = file;
                        }
                    }

                    using FileStream stream = fileToPoke.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    stream.Close();
                    System.Threading.Thread.Sleep(250);
                }
            });
        }

        private static string GetSavedGamesPath()
        {
            if (Environment.OSVersion.Version.Major < 6) throw new NotSupportedException();
            IntPtr pathPtr = IntPtr.Zero;
            try
            {
                Guid FolderSavedGames = new Guid("4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4");
                SHGetKnownFolderPath(ref FolderSavedGames, 0, IntPtr.Zero, out pathPtr);
                return Marshal.PtrToStringUni(pathPtr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pathPtr);
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);

        #endregion
    }
}
