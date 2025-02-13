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
            notificationArgs.Guid ??= Guid.NewGuid();
            var handler = Notification;

            // Always send notifications to plugins. PluginEventHandler filters out plugins
            // which have not explicitly allowed batch-mode notifications.
            if ((notificationArgs.Rendering & NotificationRendering.PluginNotifier) != 0)
            {
                handler?.Invoke(this, notificationArgs);
            }

            if (!IsLogMonitorBatchReading)
            {
                if ((notificationArgs.Rendering & NotificationRendering.NativeVisual) != 0)
                {
                    if (Properties.Core.Default.NativeNotify && !PluginManager.GetInstance.HasPopupOverrideNotifiers)
                    {
                        NativePopup.InvokeNativeNotification(notificationArgs);
                    }
                    else if (PluginManager.GetInstance.HasPopupOverrideNotifiers && (notificationArgs.Rendering & NotificationRendering.PluginNotifier) == 0)
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
                    else if (PluginManager.GetInstance.HasAudioOverrideNotifiers && (notificationArgs.Rendering & NotificationRendering.PluginNotifier) == 0)
                    {
                        // We have an overriding plugin for a native handler.
                        handler?.Invoke(this, notificationArgs);
                    }
                }
            }

            return notificationArgs.Guid ?? Guid.Empty;
        }

        public void CancelNotification(Guid id)
        {
            ExecuteOnUIThread(() => NativePopup.CloseNotification(id));

            CancelNotificationEvent?.Invoke(this, id);
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

                UpdateNotificationEvent?.Invoke(this, notificationArgs);
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
            FormsManager.FindCoreForm()?.Invoke(action);
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
        public event EventHandler<NotificationArgs> UpdateNotificationEvent;
        public event EventHandler<Guid> CancelNotificationEvent;

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
            PluginMessage?.Invoke(this, new PluginMessageArgs(plugin.Name, plugin.Version, String.Empty, message));
        }
        public void SendPluginMessage(IObservatoryPlugin plugin, string targetShortName, object message)
        {
            PluginMessage?.Invoke(this, new PluginMessageArgs(plugin.Name, plugin.Version, targetShortName, message));
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
                FormsManager.OpenPluginSettingsForm(plugin);
            });
        }

        public void OpenAbout(IObservatoryPlugin plugin)
        {
            ExecuteOnUIThread(() =>
            {
                FormsManager.OpenAboutForm(plugin.AboutInfo);
            });
        }

        public JournalEventArgs DeserializeEvent(string json, bool replay = false)
        {
            var logMonitor = LogMonitor.GetInstance;
            return logMonitor.DeserializeAndInvoke(json, replay);
        }

        public void FocusPlugin(string pluginName)
        {
            // pluginName here is based on "short name" whereas windows are named using full name.
            // Find the plugin by short name and proceed with the full name.
            IObservatoryPlugin? plugin = null;
            foreach (var p in PluginManager.GetInstance.AllUIPlugins)
            {
                if (p.ShortName == pluginName)
                {
                    plugin = p;
                    break;
                }
            }
            if (plugin != null)
            {
                ExecuteOnUIThread(() =>
                {
                    FormsManager.FocusPluginTabOrWindow(plugin);
                });
            }
        }

        internal void Shutdown()
        {
            NativePopup.CloseAll();
        }

        private void BeginBulkUpdate(IObservatoryWorker worker)
        {
            PluginUIGrid? listView = FindPluginListView(worker);
            if (listView == null) return;

            ExecuteOnUIThread(() => { listView.SuspendDrawing(); });
        }

        private void EndBulkUpdate(IObservatoryWorker worker)
        {
            PluginUIGrid? listView = FindPluginListView(worker);
            if (listView == null) return;

            ExecuteOnUIThread(() => { listView.ResumeDrawing(); });
        }

        private static PluginUIGrid? FindPluginListView(IObservatoryWorker worker)
        {
            if (worker.PluginUI.PluginUIType != PluginUI.UIType.Basic
                || worker.PluginUI.UI is not Panel) return null;

            PluginUIGrid? listView;

            if (worker.PluginUI.UI is Panel panel)
                foreach (var control in panel.Controls)
                {
                    if (control?.GetType() == typeof(PluginUIGrid))
                    {
                        listView = (PluginUIGrid)control;
                        return listView;
                    }
                }
            return null;
        }
    }
}
