using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework
{
    public class PluginUI
    {
        public readonly UIType PluginUIType;
        public object UI;
        public ObservableCollection<object> DataGrid;
        
        public PluginUI(ObservableCollection<object> DataGrid)
        {
            PluginUIType = UIType.Basic;
            this.DataGrid = DataGrid;
        }

        public PluginUI(UIType uiType, object UI)
        {
            PluginUIType = uiType;
            this.UI = UI;
        }

        public enum UIType
        {
            None = 0,
            Basic = 1,
            Avalonia = 2,
            Core = 3
        }
    }
}
