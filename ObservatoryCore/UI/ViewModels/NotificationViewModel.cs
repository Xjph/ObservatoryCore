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
            Notification = new() { Title = title, Detail = detail };
            
        }

        public Models.NotificationModel Notification { get; set; }
    }
}
