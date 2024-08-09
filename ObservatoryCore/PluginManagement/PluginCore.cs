using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Interfaces;
using Observatory.NativeNotification;
using Observatory.UI;
using Observatory.Utils;

namespace Observatory.PluginManagement
{
    public class PluginCore() : IObservatoryCore
    {
        private readonly NativeVoice NativeVoice = new();
        private readonly NativePopup NativePopup = new();
        private readonly AudioHandler AudioHandler = new();

        public string Version => System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0";

        public Action<Exception, String> GetPluginErrorLogger(IObservatoryPlugin plugin)
        {
            return (ex, context) =>
            {
                ObservatoryCore.LogError(ex, $"from plugin {plugin.ShortName} (v{plugin.Version}) {context}");
            };
        }

        public Status GetStatus() => LogMonitor.GetInstance.Status;

        public Guid SendNotification(string title, string text)
        {
            return SendNotification(new NotificationArgs() { Title = title, Detail = text });
        }

        public Guid SendNotification(NotificationArgs notificationArgs)
        {
            var guid = Guid.Empty;

#if DEBUG // For exercising testing notifier plugins in read-all
            if ((notificationArgs.Rendering & NotificationRendering.PluginNotifier) != 0)
            {
                var handler = Notification;
                handler?.Invoke(this, notificationArgs);
            }
#endif
            if (!IsLogMonitorBatchReading)
            {
#if !DEBUG
                if ((notificationArgs.Rendering & NotificationRendering.PluginNotifier) != 0)
                {
                    var handler = Notification;
                    handler?.Invoke(this, notificationArgs);
                }
#endif
                if (!PluginManager.GetInstance.HasPopupOverrideNotifiers
                    && Properties.Core.Default.NativeNotify 
                    && (notificationArgs.Rendering & NotificationRendering.NativeVisual) != 0)
                {
                    guid = NativePopup.InvokeNativeNotification(notificationArgs);
                }

                if (!PluginManager.GetInstance.HasAudioOverrideNotifiers
                    && Properties.Core.Default.VoiceNotify
                    && (notificationArgs.Rendering & NotificationRendering.NativeVocal) != 0)
                {
                    NativeVoice.EnqueueAndAnnounce(notificationArgs);
                }
            }

            return guid;
        }

        public void CancelNotification(Guid id)
        {
            ExecuteOnUIThread(() => NativePopup.CloseNotification(id));
        }

        public void UpdateNotification(Guid id, NotificationArgs notificationArgs)
        {
            if (!IsLogMonitorBatchReading)
            {
                if ((notificationArgs.Rendering & NotificationRendering.PluginNotifier) != 0)
                {
                    var handler = Notification;
                    handler?.Invoke(this, notificationArgs);
                }

                if ((notificationArgs.Rendering & NotificationRendering.NativeVisual) != 0)
                    NativePopup.UpdateNotification(id, notificationArgs);

                if (Properties.Core.Default.VoiceNotify && (notificationArgs.Rendering & NotificationRendering.NativeVocal) != 0)
                {
                    NativeVoice.EnqueueAndAnnounce(notificationArgs);
                }
            }
        }

        /// <summary>
        /// Adds an item to the datagrid on UI thread to ensure visual update.
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="item"></param>
        public void AddGridItem(IObservatoryWorker worker, object item)
        {
            worker.PluginUI.DataGrid.Add(item);
        }

        public void AddGridItems(IObservatoryWorker worker, IEnumerable<object> items)
        {
            BeginBulkUpdate(worker);

            foreach (var item in items)
            {
                worker.PluginUI.DataGrid.Add(item);
            }

            EndBulkUpdate(worker);
        }

        public void SetGridItems(IObservatoryWorker worker, IEnumerable<object> items)
        {
            BeginBulkUpdate(worker);

            worker.PluginUI.DataGrid.Clear();
            foreach (var item in items)
            {
                worker.PluginUI.DataGrid.Add(item);
            }

            EndBulkUpdate(worker);
        }

        public void ClearGrid(IObservatoryWorker worker, object templateItem)
        {
            worker.PluginUI.DataGrid.Clear();
        }

        public void ExecuteOnUIThread(Action action)
        {
            FindCoreForm()?.Invoke(action);
        }

        public System.Net.Http.HttpClient HttpClient
        {
            get => Utils.HttpClient.Client;
        }

        public LogMonitorState CurrentLogMonitorState
        {
            get => LogMonitor.GetInstance.CurrentState;
        }

        public bool IsLogMonitorBatchReading
        {
            get => LogMonitorStateChangedEventArgs.IsBatchRead(LogMonitor.GetInstance.CurrentState);
        }

        public event EventHandler<NotificationArgs> Notification;

        internal event EventHandler<PluginMessageArgs> PluginMessage;

        public string PluginStorageFolder
        {
            get
            {
                var context = new System.Diagnostics.StackFrame(1).GetMethod();
#if PORTABLE
                string? observatoryLocation = System.Diagnostics.Process.GetCurrentProcess()?.MainModule?.FileName;
                var obsDir = new FileInfo(observatoryLocation ?? String.Empty).DirectoryName;
                string folderLocation = $"{obsDir}{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}{context?.DeclaringType?.Assembly.GetName().Name}-Data{Path.DirectorySeparatorChar}";
#else
                string folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + $"{Path.DirectorySeparatorChar}ObservatoryCore{Path.DirectorySeparatorChar}{context?.DeclaringType?.Assembly.GetName().Name}{Path.DirectorySeparatorChar}";
#endif
                if (!Directory.Exists(folderLocation))
                    Directory.CreateDirectory(folderLocation);

                return folderLocation;

            }
        }

        public Task PlayAudioFile(string filePath) => AudioHandler.EnqueueAndPlay(filePath);

        public void SendPluginMessage(IObservatoryPlugin plugin, object message)
        {
            PluginMessage?.Invoke(this, new PluginMessageArgs(plugin.Name, plugin.Version, message));
        }

        public void RegisterControl(object control, Func<object, bool> applyTheme)
        {
            ThemeManager.GetInstance.RegisterControl(control, applyTheme);
        }

        public void RegisterControl(object control)
        {
            ThemeManager.GetInstance.RegisterControl(control);
        }

        public void UnregisterControl(object control)
        {
            ThemeManager.GetInstance.UnregisterControl(control);
        }

        public string GetCurrentThemeName() =>
            ThemeManager.GetInstance.CurrentTheme;

        public Dictionary<string, Color> GetCurrentThemeDetails() =>
            ThemeManager.GetInstance.CurrentThemeDetails;

        public void SaveSettings(IObservatoryPlugin plugin)
        {
            PluginManager.GetInstance.SaveSettings(plugin);
        }

        public void OpenSettings(IObservatoryPlugin plugin)
        {
            ExecuteOnUIThread(() =>
            {
                FindCoreForm()?.OpenSettings(plugin);
            });
        }

        public JournalEventArgs DeserializeEvent(string json, bool replay = false)
        {
            var logMonitor = LogMonitor.GetInstance;
            return logMonitor.DeserializeAndInvoke(json, replay);
        }

        public void FocusPlugin(string pluginName)
        {
            ExecuteOnUIThread(() =>
            {
                FindCoreForm()?.FocusPlugin(pluginName);
            });
        }

        internal void Shutdown()
        {
            NativePopup.CloseAll();
        }

        private void BeginBulkUpdate(IObservatoryWorker worker)
        {
            PluginListView? listView = FindPluginListView(worker);
            if (listView == null) return;

            ExecuteOnUIThread(() => { listView.SuspendDrawing(); });
        }

        private void EndBulkUpdate(IObservatoryWorker worker)
        {
            PluginListView? listView = FindPluginListView(worker);
            if (listView == null) return;

            ExecuteOnUIThread(() => { listView.ResumeDrawing(); });
        }

        private static PluginListView? FindPluginListView(IObservatoryWorker worker)
        {
            if (worker.PluginUI.PluginUIType != PluginUI.UIType.Basic
                || worker.PluginUI.UI is not Panel) return null;

            PluginListView? listView;

            if (worker.PluginUI.UI is Panel panel)
                foreach (var control in panel.Controls)
                {
                    if (control?.GetType() == typeof(PluginListView))
                    {
                        listView = (PluginListView)control;
                        return listView;
                    }
                }
            return null;
        }

        private static CoreForm? FindCoreForm()
        {
            foreach (var f in Application.OpenForms)
            {
                if (f is CoreForm form)
                {
                    return form;
                }
            }
            return null;
        }
    }
}
