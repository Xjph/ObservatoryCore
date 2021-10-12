using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.UI.Models
{
    public class NotificationModel
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Colour { get; set; }
        public int Timeout { get; set; }
        public double XPos { get; set; }
        public double YPos { get; set; }
    }
}
