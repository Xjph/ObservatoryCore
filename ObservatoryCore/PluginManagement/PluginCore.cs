using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Interfaces;
using Observatory.NativeNotification;
using Observatory.UI;
using Observatory.Utils;

namespace Observatory.PluginManagement
{
    public class PluginCore : IObservatoryCore
    {

        private readonly NativeVoice NativeVoice;
        private readonly NativePopup NativePopup;
        private bool OverridePopup;
        private bool OverrideAudio;

        public PluginCore(bool OverridePopup = false, bool OverrideAudio = false)
        {
            NativeVoice = new();
            NativePopup = new();
            this.OverridePopup = OverridePopup;
            this.OverrideAudio = OverrideAudio;
        }

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
            if (notificationArgs.Rendering.HasFlag(NotificationRendering.PluginNotifier))
            {
                var handler = Notification;
                handler?.Invoke(this, notificationArgs);
            }
#endif
            if (!IsLogMonitorBatchReading)
            {
#if !DEBUG
                if (notificationArgs.Rendering.HasFlag(NotificationRendering.PluginNotifier))
                {
                    var handler = Notification;
                    handler?.Invoke(this, notificationArgs);
                }
#endif

                if (!OverridePopup && Properties.Core.Default.NativeNotify && notificationArgs.Rendering.HasFlag(NotificationRendering.NativeVisual))
                {
                    guid = NativePopup.InvokeNativeNotification(notificationArgs);
                }

                if (!OverrideAudio && Properties.Core.Default.VoiceNotify && notificationArgs.Rendering.HasFlag(NotificationRendering.NativeVocal))
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
                if (notificationArgs.Rendering.HasFlag(NotificationRendering.PluginNotifier))
                {
                    var handler = Notification;
                    handler?.Invoke(this, notificationArgs);
                }

                if (notificationArgs.Rendering.HasFlag(NotificationRendering.NativeVisual))
                    NativePopup.UpdateNotification(id, notificationArgs);

                if (Properties.Core.Default.VoiceNotify && notificationArgs.Rendering.HasFlag(NotificationRendering.NativeVocal))
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

        public async Task PlayAudioFile(string filePath)
        {
            await AudioHandler.PlayFile(filePath);
        }

        public void SendPluginMessage(IObservatoryPlugin plugin, object message)
        {
            PluginMessage?.Invoke(this, new PluginMessageArgs(plugin.Name, plugin.Version, message));
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

        private PluginListView? FindPluginListView(IObservatoryWorker worker)
        {
            if (worker.PluginUI.PluginUIType != PluginUI.UIType.Basic
                || !(worker.PluginUI.UI is Panel)) return null;

            PluginListView? listView = null;
            Panel? panel = worker.PluginUI.UI as Panel;

            if (panel != null)
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

        private CoreForm? FindCoreForm()
        {
            foreach (var f in Application.OpenForms)
            {
                if (f is CoreForm)
                {
                    return (CoreForm)f;
                }
            }
            return null;
        }
    }
}
