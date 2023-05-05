using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Interfaces;
using Observatory.NativeNotification;
using Observatory.Utils;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Observatory.PluginManagement
{
    public class PluginCore : IObservatoryCore
    {

        private readonly NativeVoice NativeVoice;
        private readonly NativePopup NativePopup;

        public PluginCore()
        {
            NativeVoice = new();
            NativePopup = new();
        }

        public string Version => System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0";

        public Action<Exception, String> GetPluginErrorLogger(IObservatoryPlugin plugin)
        {
            return (ex, context) =>
            {
                ObservatoryCore.LogError(ex, $"from plugin {plugin.ShortName} {context}");
            };
        }

        public Status GetStatus()
        {
            throw new NotImplementedException();
        }

        public Guid SendNotification(string title, string text)
        {
            return SendNotification(new NotificationArgs() { Title = title, Detail = text });
        }

        public Guid SendNotification(NotificationArgs notificationArgs)
        {
            var guid = Guid.Empty;

            if (!IsLogMonitorBatchReading)
            {
                if (notificationArgs.Rendering.HasFlag(NotificationRendering.PluginNotifier))
                {
                    var handler = Notification;
                    handler?.Invoke(this, notificationArgs);
                }

                if (Properties.Core.Default.NativeNotify && notificationArgs.Rendering.HasFlag(NotificationRendering.NativeVisual))
                {
                    guid = NativePopup.InvokeNativeNotification(notificationArgs);
                }

                if (Properties.Core.Default.VoiceNotify && notificationArgs.Rendering.HasFlag(NotificationRendering.NativeVocal))
                {
                    NativeVoice.EnqueueAndAnnounce(notificationArgs);
                }
            }

            return guid;
        }

        public void CancelNotification(Guid id)
        {
            NativePopup.CloseNotification(id);
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
            //TODO: Add to winform list
        }

        public void ClearGrid(IObservatoryWorker worker, object templateItem)
        {
            //TODO: Clear winform list
        }

        public void ExecuteOnUIThread(Action action)
        {
            //TODO: Execute action
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

        public string PluginStorageFolder
        {
            get
            {
                var context = new System.Diagnostics.StackFrame(1).GetMethod();

                string folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + $"{Path.DirectorySeparatorChar}ObservatoryCore{Path.DirectorySeparatorChar}{context?.DeclaringType?.Assembly.GetName().Name}{Path.DirectorySeparatorChar}";

                if (!Directory.Exists(folderLocation))
                    Directory.CreateDirectory(folderLocation);

                return folderLocation;
            }
        }

        internal void Shutdown()
        {
            NativePopup.CloseAll();
        }

        private static bool FirstRowIsAllNull(IObservatoryWorker worker)
        {
            bool allNull = true;
            Type itemType = worker.PluginUI.DataGrid[0].GetType();
            foreach (var property in itemType.GetProperties())
            {
                if (property.GetValue(worker.PluginUI.DataGrid[0], null) != null)
                {
                    allNull = false;
                    break;
                }
            }

            return allNull;
        }
    }
}
