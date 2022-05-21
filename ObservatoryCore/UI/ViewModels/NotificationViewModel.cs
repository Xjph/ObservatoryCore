using Observatory.Framework;

namespace Observatory.UI.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {
        public NotificationViewModel(NotificationArgs notificationArgs)
        {

            Notification = new()
            {
                Title = notificationArgs.Title,
                Detail = notificationArgs.Detail,
                Timeout = notificationArgs.Timeout,
                XPos = notificationArgs.XPos,
                YPos = notificationArgs.YPos,
                Colour = Avalonia.Media.Color.FromUInt32(Properties.Core.Default.NativeNotifyColour).ToString()
            };
            
        }

        public Models.NotificationModel Notification { get; set; }
    }
}
