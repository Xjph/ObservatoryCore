using System.Collections.ObjectModel;
using static Observatory.Framework.PluginUI;

namespace Observatory.Framework
{
    /// <summary>
    /// Instantiate PluginUI of UIType.Basic.
    /// </summary>
    /// <param name="DataGrid">
    /// <para>Collection bound to DataGrid used by plugins with UIType.Basic.</para>
    /// <para>Objects in collection should be of a class defined within the plugin consisting of string properties.<br/>Each object is a single row, and the property names are used as column headers.</para>
    /// </param>
    public class PluginActionUI(
        ObservableCollection<object> DataGrid,
        PluginActionUI.ActionDock pluginActionLocation
    ) : PluginUI(DataGrid)
    {
        /// <summary>
        /// Actions to be added as controls below the plugin UI grid. Control type is specified by ActionType.<br/>
        /// Controls are labeled with the name of the action, or the name specified by the ActionLabel property, and clicking them invokes the corresponding Action delegate.
        /// </summary>
        public List<PluginAction> Actions { get; set; } = [];

        /// <summary>
        /// Dock location of action controls within the plugin panel. If unspecified bottom will be used.
        /// </summary>
        public ActionDock PluginActionLocation { get; } = pluginActionLocation;

        /// <summary>
        /// Defines an action control for the plugin UI.
        /// </summary>
        public class PluginAction(
            string label,
            Action<bool> action,
            PluginActionUI.ActionType actionType = PluginActionUI.ActionType.Button,
            bool initialState = false
        )
        {
            /// <summary>
            /// Text displayed on the action control.
            /// </summary>
            public string Label { get; } = label;

            /// <summary>
            /// Action to be invoked by the control when clicked. Checkbox and Radiobutton controls pass their updated checked state.
            /// </summary>
            public Action<bool> Action { get; } = action;

            /// <summary>
            /// Type of control to represent the action. Button, Checkbox, or Radiobutton. Default is Button.
            /// </summary>
            public ActionType ActionType { get; } = actionType;

            /// <summary>
            /// Initial state of the control for Checkbox and Radiobutton action types.
            /// Should be set in plugin constructor or `Load` method.
            /// </summary>
            public bool InitialState { get; init; } = initialState;
        }

        /// <summary>
        /// Indicates the control type of the actions.
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// Action is a button. Bool parameter unused and can be discarded.
            /// </summary>
            Button = 0,

            /// <summary>
            /// Actions is a checkbox. Bool parameter indicates the checked state.
            /// </summary>
            Checkbox = 1,

            /// <summary>
            /// Actions are radio buttons. Bool parameter indicates the selected state. Sequential radio buttons are grouped.
            /// </summary>
            Radiobutton = 2,
        }

        /// <summary>
        /// Edge of plugin UI area where action controls are placed.
        /// </summary>
        public enum ActionDock
        {
            /// <summary>
            /// Dock to bottom
            /// </summary>
            Bottom = 0,

            /// <summary>
            /// Dock to top
            /// </summary>
            Top = 1,

            /// <summary>
            /// Dock to left
            /// </summary>
            Left = 2,

            /// <summary>
            /// Dock to right
            /// </summary>
            Right = 3,
        }
    }
}
