using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework
{
    /// <summary>
    /// Class permitting plugins to provide their UI, if any, to Observatory Core.
    /// </summary>
    public class PluginUI
    {
        /// <summary>
        /// Type of UI used by plugin.
        /// </summary>
        public readonly UIType PluginUIType;

        /// <summary>
        /// <para>UI object used by plugins with UIType.Avalonia.</para>
        /// <para>(Untested/not implemented)</para>
        /// </summary>
        public object UI;

        /// <summary>
        /// <para>Collection bound to DataGrid used byu plugins with UIType.Basic.</para>
        /// <para>Objects in collection should be of a class defined within the plugin consisting of string properties.<br/>Each object is a single row, and the property names are used as column headers.</para>
        /// </summary>
        public ObservableCollection<object> DataGrid;

        /// <summary>
        /// Instantiate PluginUI of UIType.Basic.
        /// </summary>
        /// <param name="DataGrid">
        /// <para>Collection bound to DataGrid used byu plugins with UIType.Basic.</para>
        /// <para>Objects in collection should be of a class defined within the plugin consisting of string properties.<br/>Each object is a single row, and the property names are used as column headers.</para>
        /// </param>
        public PluginUI(ObservableCollection<object> DataGrid)
        {
            PluginUIType = UIType.Basic;
            this.DataGrid = DataGrid;
        }

        /// <summary>
        /// Instantiate PluginUI of specified UIType.
        /// <para>(Untested/not implemented)</para>
        /// </summary>
        /// <param name="uiType">UIType for plugin.</param>
        /// <param name="UI">Avalonia control to place in plugin tab.</param>
        public PluginUI(UIType uiType, object UI)
        {
            PluginUIType = uiType;
            this.UI = UI;
        }

        /// <summary>
        /// Options for plugin UI types.
        /// </summary>
        public enum UIType
        {
            /// <summary>
            /// No UI. Tab will not be added to list.
            /// </summary>
            None = 0,
            /// <summary>
            /// Simple DataGrid, to which items can be added or removed.
            /// </summary>
            Basic = 1,
            /// <summary>
            /// AvaloniaUI control which is placed in plugin tab.
            /// </summary>
            Avalonia = 2,
            /// <summary>
            /// UI used by Observatory Core settings tab.<br/>
            /// Not intended for use by plugins.
            /// </summary>
            Core = 3
        }
    }
}
