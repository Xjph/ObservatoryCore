using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.UI.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {
        public NotificationViewModel(string title, string detail)
        {

            Notification = new()
            {
                Title = title,
                Detail = detail,
                Colour = Avalonia.Media.Color.FromUInt32(Properties.Core.Default.NativeNotifyColour).ToString()
            };
            
        }

        public Models.NotificationModel Notification { get; set; }
    }
}
