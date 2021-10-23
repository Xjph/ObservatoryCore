using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Interfaces;
using Observatory.NativeNotification;
using System;
using System.Runtime.InteropServices;

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

        public string Version => System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

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

            if (!LogMonitor.GetInstance.ReadAllInProgress())
            {
                var handler = Notification;
                handler?.Invoke(this, notificationArgs);

                if (Properties.Core.Default.NativeNotify)
                {
                    guid = NativePopup.InvokeNativeNotification(notificationArgs);
                }

                if (Properties.Core.Default.VoiceNotify)
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
            NativePopup.UpdateNotification(id, notificationArgs);
        }

        /// <summary>
        /// Adds an item to the datagrid on UI thread to ensure visual update.
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="item"></param>
        public void AddGridItem(IObservatoryWorker worker, object item)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                worker.PluginUI.DataGrid.Add(item);

                //Hacky removal of original empty object if one was used to populate columns
                if (worker.PluginUI.DataGrid.Count == 2)
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

                    if (allNull)
                        worker.PluginUI.DataGrid.RemoveAt(0);
                }

            });
        }

        public void ClearGrid(IObservatoryWorker worker, object templateItem)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                worker.PluginUI.DataGrid.Add(templateItem);
                while (worker.PluginUI.DataGrid.Count > 1)
                    worker.PluginUI.DataGrid.RemoveAt(0);
            });
        }

        public void ExecuteOnUIThread(Action action)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(action);
        }

        public event EventHandler<NotificationArgs> Notification;
    }
}
