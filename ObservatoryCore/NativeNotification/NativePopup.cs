using Observatory.Framework;
using Observatory.UI;

namespace Observatory.NativeNotification
{
    public class NativePopup
    {
        private Dictionary<Guid, NotificationForm> notifications;

        public NativePopup()
        {
            notifications = new();
        }

        public Guid InvokeNativeNotification(NotificationArgs notificationArgs)
        {
            var notificationGuid = Guid.NewGuid();
            var notification = new NotificationForm()
            {
                Guid = notificationGuid
            };
            notification.Show();
            notifications.Add(notificationGuid, notification);
            
            //TODO: Implement winform notification

            return notificationGuid;
        }

        private void NotifyWindow_Closed(object sender, EventArgs e)
        {
            var currentNotification = (NotificationForm)sender;

            if (notifications.ContainsKey(currentNotification.Guid))
            {
                notifications.Remove(currentNotification.Guid);
            }
        }

        public void CloseNotification(Guid guid)
        {
            if (notifications.ContainsKey(guid))
            {
                notifications[guid].Close();
            }
        }

        public void UpdateNotification(Guid guid, NotificationArgs notificationArgs)
        {
            if (notifications.ContainsKey(guid))
            {
                //TODO: Update notification content
                // notifications[guid].DataContext = new NotificationViewModel(notificationArgs);
            }
        }

        public void CloseAll()
        {
            foreach (var notification in notifications)
            {
                notification.Value?.Close();
            }
        }
    }
}
