using Observatory.Framework;
using Observatory.UI;

namespace Observatory.NativeNotification
{
    public class NativePopup
    {
        private Dictionary<Guid, NotificationForm> notifications;

        public NativePopup()
        {
            notifications = [];
        }

        public Guid InvokeNativeNotification(NotificationArgs notificationArgs)
        {
            var notificationGuid = notificationArgs.Guid ?? Guid.NewGuid();
            Application.OpenForms[0]?.Invoke(() =>
            {
                var notification = new NotificationForm(notificationGuid, notificationArgs);

                notification.FormClosed += NotifyWindow_Closed;

                foreach(var notificationForm in notifications)
                {
                    notificationForm.Value.AdjustOffset(true);
                }

                notifications.Add(notificationGuid, notification);
                notification.Show();
            });
            
            return notificationGuid;
        }

        private void NotifyWindow_Closed(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                var currentNotification = (NotificationForm)sender;

                foreach (var notification in notifications.Where(n => n.Value.CreationTime < currentNotification.CreationTime))
                {
                    notification.Value.AdjustOffset(false);
                }

                if (notifications.ContainsKey(currentNotification.Guid))
                {
                    notifications.Remove(currentNotification.Guid);
                }
            }
        }

        public void CloseNotification(Guid guid)
        {
            if (notifications.TryGetValue(guid, out NotificationForm? value))
            {
                value.Close();
            }
        }

        public void UpdateNotification(NotificationArgs notificationArgs)
        {
            var guid = notificationArgs.Guid 
                ?? throw new ArgumentNullException(nameof(notificationArgs), "Cannot update notification without Guid.");
            if (notifications.TryGetValue(guid, out NotificationForm? value))
            {
                value.Update(notificationArgs);
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
