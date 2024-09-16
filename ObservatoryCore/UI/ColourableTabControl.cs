using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.UI
{
    public class ColourableTabControl : TabControl
    {
        public Color TabColor { get; set; }
        public Color SelectedTabColor { get; set; }
    }
}
