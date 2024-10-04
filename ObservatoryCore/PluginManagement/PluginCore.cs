using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Interfaces;
using Observatory.Framework.ParameterTypes;
using Observatory.NativeNotification;
using Observatory.UI;
using Observatory.Utils;
using System.Dynamic;

namespace Observatory.PluginManagement
{
    public class PluginCore : IObservatoryCore
    {
        
        private readonly NativePopup NativePopup = new();
        private readonly AudioHandler AudioHandler = new();
        private readonly NativeVoice NativeVoice;

        internal PluginCore()
        {
            NativeVoice = new(AudioHandler);
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
            var handler = Notification;

#if DEBUG // For exercising testing notifier plugins in read-all
            if ((notificationArgs.Rendering & NotificationRendering.PluginNotifier) != 0)
            {
                handler?.Invoke(this, notificationArgs);
            }
#endif
            if (!IsLogMonitorBatchReading)
            {
#if !DEBUG
                if ((notificationArgs.Rendering & NotificationRendering.PluginNotifier) != 0)
                {
                    handler?.Invoke(this, notificationArgs);
                }
#endif
                if ((notificationArgs.Rendering & NotificationRendering.NativeVisual) != 0)
                {
                    if (Properties.Core.Default.NativeNotify && !PluginManager.GetInstance.HasPopupOverrideNotifiers)
                    {
                        guid = NativePopup.InvokeNativeNotification(notificationArgs);
                    }
                    else if (PluginManager.GetInstance.HasPopupOverrideNotifiers)
                    {
                        // We have an overriding plugin for a native handler. Route it there.
                        handler?.Invoke(this, notificationArgs);
                    }
                }

                if ((notificationArgs.Rendering & NotificationRendering.NativeVocal) != 0)
                {
                    if (Properties.Core.Default.VoiceNotify && !PluginManager.GetInstance.HasAudioOverrideNotifiers)
                    {
                        NativeVoice.AudioHandlerEnqueue(notificationArgs);
                    }
                    else if (PluginManager.GetInstance.HasAudioOverrideNotifiers)
                    {
                        // We have an overriding plugin for a native handler.
                        handler?.Invoke(this, notificationArgs);
                    }
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
                    NativeVoice.AudioHandlerEnqueue(notificationArgs);
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

        public void AddGridItems(IObservatoryWorker worker, IEnumerable<object> items, bool grouped = false)
        {
            BeginBulkUpdate(worker);
            grouped = true;
            if (grouped)
            {
                var groupId = Guid.NewGuid();
                foreach (var item in items)
                {
                    dynamic groupedItem = new ExpandoObject();
                    var interimDict = (IDictionary<string, object?>)groupedItem;

                    foreach (var property in item.GetType().GetProperties())
                        interimDict.Add(property.Name, property.GetValue(item));

                    groupedItem.ObservatoryListViewGroupID = groupId;

                    worker.PluginUI.DataGrid.Add(groupedItem);
                }
            }
            else 
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
                var pluginAssemblyName = context?.DeclaringType?.Assembly.GetName().Name;
                return GetStorageFolderForPlugin(pluginAssemblyName);
            }
        }

        internal string GetStorageFolderForPlugin(string pluginAssemblyName = "")
        {
#if PORTABLE
            string? observatoryLocation = System.Diagnostics.Process.GetCurrentProcess()?.MainModule?.FileName;
            var obsDir = new FileInfo(observatoryLocation ?? String.Empty).DirectoryName;
            var rootdataDir = $"{obsDir}{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}";
            string pluginDataDir = $"{rootdataDir}{pluginAssemblyName}-Data{Path.DirectorySeparatorChar}";
#else
            var rootdataDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}ObservatoryCore{Path.DirectorySeparatorChar}";
            string pluginDataDir = $"{rootdataDir}{pluginAssemblyName}{Path.DirectorySeparatorChar}";
#endif
            // Return the root data directory if no plugin assembly name specified.
            if (string.IsNullOrWhiteSpace(pluginAssemblyName))
            {
                if (!Directory.Exists(rootdataDir))
                    Directory.CreateDirectory(rootdataDir);

                return rootdataDir;
            }
            else
            {
                if (!Directory.Exists(pluginDataDir))
                    Directory.CreateDirectory(pluginDataDir);

                return pluginDataDir;
            }
        }

        public Task PlayAudioFile(string filePath, AudioOptions? options = null)
            => AudioHandler.EnqueueAndPlay(filePath, options ?? new());

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

        public void OpenAbout(IObservatoryPlugin plugin)
        {
            ExecuteOnUIThread(() =>
            {
                FindCoreForm()?.OpenAbout(plugin.AboutInfo);
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
