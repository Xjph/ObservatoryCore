using Observatory.Framework;
using System;
using System.Collections.Generic;
using Observatory.UI.Views;
using Observatory.UI.ViewModels;

namespace Observatory.NativeNotification
{
    public class NativePopup
    {
        private Dictionary<Guid, NotificationView> notifications;

        public NativePopup()
        {
            notifications = new();
        }

        public Guid InvokeNativeNotification(NotificationArgs notificationArgs)
        {
            var notificationGuid = Guid.NewGuid();
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                var notifyWindow = new NotificationView(notificationGuid) { DataContext = new NotificationViewModel(notificationArgs) };
                notifyWindow.Closed += NotifyWindow_Closed;
                notifications.Add(notificationGuid, notifyWindow);
                notifyWindow.Show();
            });

            return notificationGuid;
        }

        private void NotifyWindow_Closed(object sender, EventArgs e)
        {
            var notification = (NotificationView)sender;

            if (notifications.ContainsKey(notification.Guid))
                notifications.Remove(notification.Guid);
        }

        public void CloseNotification(Guid guid)
        {
            if (notifications.ContainsKey(guid))
                notifications[guid].Close();
        }

        public void UpdateNotification(Guid guid, NotificationArgs notificationArgs)
        {
            if (notifications.ContainsKey(guid))
                notifications[guid].DataContext = new NotificationViewModel(notificationArgs);
        }
    }
}
