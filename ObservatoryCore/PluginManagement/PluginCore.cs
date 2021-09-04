using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace Observatory.PluginManagement
{
    public class PluginCore : IObservatoryCore
    {

        public string Version => System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

        public Status GetStatus()
        {
            throw new NotImplementedException();
        }

        public void SendNotification(string title, string text)
        {
            if (!LogMonitor.GetInstance.ReadAllInProgress())
            {
                var handler = Notification;
                handler?.Invoke(this, new NotificationEventArgs() { Title = title, Detail = text });

                if (Properties.Core.Default.NativeNotify)
                {
                    InvokeNativeNotification(title, text);
                }

                if (Properties.Core.Default.VoiceNotify && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var speech = new System.Speech.Synthesis.SpeechSynthesizer()
                    {
                        Volume = Properties.Core.Default.VoiceVolume,
                        Rate = Properties.Core.Default.VoiceRate
                    };
                    speech.SelectVoice(Properties.Core.Default.VoiceSelected);
                    speech.SpeakAsync(title + ": " + text);
                }
            }
        }

        private void InvokeNativeNotification(string title, string text)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                var notifyWindow = new UI.Views.NotificationView() { DataContext = new UI.ViewModels.NotificationViewModel(title, text) };
                notifyWindow.Show();
            });
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

        public event EventHandler<NotificationEventArgs> Notification;
    }
}
