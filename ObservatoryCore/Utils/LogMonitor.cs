using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Observatory.Framework;
using Observatory.Framework.Files;

namespace Observatory.Utils
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
            SetLogMonitorState(LogMonitorState.Idle);
        }

        #endregion

        #region Public properties
        public LogMonitorState CurrentState
        {
            get => currentState;
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
            SetLogMonitorState(LogMonitorState.Realtime);
            JournalPoke();
        }

        public void Stop()
        {
            journalWatcher.EnableRaisingEvents = false;
            statusWatcher.EnableRaisingEvents = false;
            SetLogMonitorState(LogMonitorState.Idle);
        }

        public void ChangeWatchedDirectory(string path)
        {
            journalWatcher.Dispose();
            statusWatcher.Dispose();
            InitializeWatchers(path);
        }

        public bool IsMonitoring()
        {
            return currentState.HasFlag(LogMonitorState.Realtime);
        }

        // TODO(fredjk_gh): Remove?
        public bool ReadAllInProgress()
        {
            return LogMonitorStateChangedEventArgs.IsBatchRead(currentState);
        }

        public void ReadAllJournals()
        {
            ReadAllJournals(string.Empty);
        }

        public void ReadAllJournals(string path)
        {
            // Prevent pre-reading when starting monitoring after reading all.
            firstStartMonitor = false;
            SetLogMonitorState(currentState | LogMonitorState.Batch);

            DirectoryInfo logDirectory = GetJournalFolder(path);
            var files = GetJournalFilesOrdered(logDirectory);
            var readErrors = new List<(Exception ex, string file, string line)>();
            foreach (var file in files)
            {
                readErrors.AddRange(
                    ProcessLines(ReadAllLines(file.FullName), file.Name));
            }

            ReportErrors(readErrors);
            SetLogMonitorState(currentState & ~LogMonitorState.Batch);
        }

        public void PrereadJournals()
        {
            if (!Properties.Core.Default.TryPrimeSystemContextOnStartMonitor ||
                Properties.Core.Default.StartReadAll) return;

            SetLogMonitorState(currentState | LogMonitorState.PreRead);

            DirectoryInfo logDirectory = GetJournalFolder(Properties.Core.Default.JournalFolder);
            var files = GetJournalFilesOrdered(logDirectory);

            // Read at most the last two files (in case we were launched after the game and the latest
            // journal is mostly empty) but keeping only the lines since the last FSDJump.
            List<string> lastSystemLines = new();
            List<string> lastFileLines = new();
            string lastLoadGame = string.Empty;
            bool sawFSDJump = false;
            foreach (var file in files.Skip(Math.Max(files.Count() - 2, 0)))
            {
                var lines = ReadAllLines(file.FullName);
                foreach (var line in lines)
                {
                    var eventType = JournalUtilities.GetEventType(line);
                    if (eventType.Equals("FSDJump") || eventType.Equals("CarrierJump") && line.Contains("\"Docked\":true"))
                    {
                        // Reset, start collecting again.
                        lastSystemLines.Clear();
                        sawFSDJump = true;
                    }
                    else if (eventType.Equals("Fileheader"))
                    {
                        lastFileLines.Clear();
                    }
                    else if (eventType.Equals("LoadGame"))
                    {
                        lastLoadGame = line;
                    }
                    lastSystemLines.Add(line);
                    lastFileLines.Add(line);
                }
            }

            // If we didn't see a jump in the recent logs (Cmdr is stationary in a system for a while
            // ie. deep-space mining from a carrier), at very least, read from the beginning of the
            // current journal file which includes the important stuff like the last "LoadGame", etc. This
            // also helps out in cases where one forgets to hit "Start Monitor" until part-way into the
            // session (if auto-start is not enabled).
            List<string> linesToRead = lastFileLines;
            if (sawFSDJump)
            {
                // If we saw a LoadGame, insert it as well. This ensures odyssey biologicials are properly
                // counted/presented.
                if (!string.IsNullOrEmpty(lastLoadGame))
                {
                    lastSystemLines.Insert(0, lastLoadGame);
                }
                linesToRead = lastSystemLines;
            }

            ReportErrors(ProcessLines(linesToRead, "Pre-read"));
            SetLogMonitorState(currentState & ~LogMonitorState.PreRead);
        }

        #endregion

        #region Public Events

        public event EventHandler<LogMonitorStateChangedEventArgs> LogMonitorStateChanged;

        public event EventHandler<JournalEventArgs> JournalEntry;

        public event EventHandler<JournalEventArgs> StatusUpdate;

        #endregion

        #region Private Fields

        private FileSystemWatcher journalWatcher;
        private FileSystemWatcher statusWatcher;
        private Dictionary<string, Type> journalTypes;
        private Dictionary<string, int> currentLine;
        private LogMonitorState currentState = LogMonitorState.Idle; // Change via #SetLogMonitorState
        private bool firstStartMonitor = true;
        private string[] EventsWithAncillaryFile = new string[]
        {
            "Cargo",
            "NavRoute",
            "Market",
            "Outfitting",
            "Shipyard",
            "Backpack",
            "FCMaterials",
            "ModuleInfo",
            "ShipLocker"
        };

        #endregion

        #region Private Methods

        private void SetLogMonitorState(LogMonitorState newState)
        {
            var oldState = currentState;
            currentState = newState;
            LogMonitorStateChanged?.Invoke(this, new LogMonitorStateChangedEventArgs
            {
                PreviousState = oldState,
                NewState = newState
            }); ;

            System.Diagnostics.Debug.WriteLine("LogMonitor State change: {0} -> {1}", oldState, newState);
        }

        private void InitializeWatchers(string path)
        {
            DirectoryInfo logDirectory = GetJournalFolder(path);

            journalWatcher = new FileSystemWatcher(logDirectory.FullName, "Journal.*.??.log")
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
                    logDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string defaultJournalPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                    + "/.steam/debian-installation/steamapps/compatdata/359320/pfx/drive_c/users/steamuser/Saved Games/Frontier Developments/Elite Dangerous"
                    : GetSavedGamesPath() + @"\Frontier Developments\Elite Dangerous";

                logDirectory =
                    Directory.Exists(defaultJournalPath)
                    ? new DirectoryInfo(defaultJournalPath)
                    : new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            }
            else
            {
                throw new NotImplementedException("Current OS Platform Not Supported.");
            }

            if (Properties.Core.Default.JournalFolder != path)
            {
                Properties.Core.Default.JournalFolder = path;
                SettingsManager.Save();
            }

            return logDirectory;
        }

        private List<(Exception ex, string file, string line)> ProcessLines(List<string> lines, string file)
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

        private JournalEventArgs DeserializeToEventArgs(string eventType, string line)
        {

            var eventClass = journalTypes[eventType];
            MethodInfo journalRead = typeof(JournalReader).GetMethod(nameof(JournalReader.ObservatoryDeserializer));
            MethodInfo journalGeneric = journalRead.MakeGenericMethod(eventClass);
            object entry = journalGeneric.Invoke(null, new object[] { line });
            return new JournalEventArgs() { journalType = eventClass, journalEvent = entry };
        }

        private void DeserializeAndInvoke(string line)
        {
            var eventType = JournalUtilities.GetEventType(line);
            if (!journalTypes.ContainsKey(eventType))
            {
                eventType = "JournalBase";
            }

            var journalEvent = DeserializeToEventArgs(eventType, line);

            JournalEntry?.Invoke(this, journalEvent);

            // Files are only valid if realtime, otherwise they will be stale or empty.
            if (!currentState.HasFlag(LogMonitorState.Batch) && EventsWithAncillaryFile.Contains(eventType))
            {
                HandleAncillaryFile(eventType);
            }
        }

        private void HandleAncillaryFile(string eventType)
        {
            string filename = eventType == "ModuleInfo"
                ? "ModulesInfo.json" // Just FDev things
                : eventType + ".json";

            // I have no idea what order Elite writes these files or if they're already written
            // by the time the journal updates.
            // Brief sleep to ensure the content is updated before we read it.

            // Some files are still locked by another process after 50ms.
            // Retry every 50ms for 0.5 seconds before giving up.

            string fileContent = null;
            int retryCount = 0;

            while (fileContent == null && retryCount < 10)
            {
                Thread.Sleep(50);
                try
                {
                    using var fileStream = File.Open(journalWatcher.Path + Path.DirectorySeparatorChar + filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(fileStream);
                    fileContent = reader.ReadToEnd();
                    var fileObject = DeserializeToEventArgs(eventType + "File", fileContent);
                    JournalEntry?.Invoke(this, fileObject);
                }
                catch
                {
                    retryCount++;
                }
            }
        }

        private void ReportErrors(List<(Exception ex, string file, string line)> readErrors)
        {
            if (readErrors.Any())
            {
                var errorList = readErrors.Select(error =>
                {
                    string message;
                    if (error.ex.InnerException == null)
                    {
                        message = error.ex.Message;
                    }
                    else
                    {
                        message = error.ex.InnerException.Message;
                    }
                    return ($"Error reading file {error.file}: {message}", error.line);
                });

                ErrorReporter.ShowErrorPopup($"Journal Read Error{(readErrors.Count > 1 ? "s" : "")}", errorList.ToList());

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
                try
                {
                    DeserializeAndInvoke(line);
                }
                catch (Exception ex)
                {
                    ReportErrors(new List<(Exception ex, string file, string line)>() { (ex, eventArgs.Name, line) });
                }
            }

            currentLine[eventArgs.FullPath] = fileContent.Count;
        }

        private List<string> ReadAllLines(string path)
        {
            var lines = new List<string>();
            try
            {
                using (StreamReader file = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    while (!file.EndOfStream)
                    {
                        lines.Add(file.ReadLine());
                    }
                }
            }
            catch (IOException ioEx)
            {
                ReportErrors(new List<(Exception, string, string)>() { (ioEx, path, "<reading all lines>") });
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
            var journalFolder = GetJournalFolder();

            await Task.Run(() =>
            {
                while (IsMonitoring())
                {
                    var journals = GetJournalFilesOrdered(journalFolder);

                    if (journals.Any())
                    {
                        FileInfo fileToPoke = GetJournalFilesOrdered(journalFolder).Last();

                        using FileStream stream = fileToPoke.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        stream.Close();
                    }
                    Thread.Sleep(250);
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

        private IEnumerable<FileInfo> GetJournalFilesOrdered(DirectoryInfo journalFolder)
        {
            return from file in journalFolder.GetFiles("Journal.*.??.log")
                   orderby file.LastWriteTime
                   select file;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);

        #endregion
    }
}
