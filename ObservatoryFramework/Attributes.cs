using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework
{

    #region Settings class attributes
    /// <summary>
    /// Specifies the width of a settings column in the settings view. There are two columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SettingSuggestedColumnWidth : Attribute
    {
        /// <summary>
        /// Specifies the width of a settings column in the settings view. There are two columns.
        /// </summary>
        /// <param name="width">Provides a hint of the width of a settings column.</param>
        public SettingSuggestedColumnWidth(int width)
        {
            Width = width;
        }

        /// <summary>
        /// Provides a hint of the width of a settings column.
        /// </summary>
        public int Width { get; }
    }
    #endregion

    #region Setting property attributes

    /// <summary>
    /// Specifies text to display as the name of the setting in the UI instead of the property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingDisplayName : Attribute
    {
        private string name;

        /// <summary>
        /// Specifies text to display as the name of the setting in the UI instead of the property name.
        /// </summary>
        /// <param name="name">Name to display</param>
        public SettingDisplayName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Accessor to get/set displayed name.
        /// </summary>
        public string DisplayName
        {
            get => name;
            set => name = value;
        }
    }

    /// <summary>
    /// Starts a new visual group of settings beginning with the current setting with an optional label.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingNewGroup : Attribute
    {
        /// <summary>
        /// Starts a new visual group of settings beginning with the current setting with an optional label.
        /// </summary>
        /// <param name="label">An optional label describing the group.</param>
        public SettingNewGroup(string label = "")
        {
            Label = label;
        }

        /// <summary>
        /// An optional label describing the group.
        /// </summary>
        public string Label { get; }
    }

    /// <summary>
    /// Indicates that the property should not be displayed to the user in the UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingIgnore : Attribute
    { }

    /// <summary>
    /// Indicates numeric properly should use a slider control instead of a numeric textbox with roller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingNumericUseSlider : Attribute
    { }

    /// <summary>
    /// Specify backing value used by Dictionary&lt;string, object&gt; to indicate selected option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingBackingValue : Attribute
    {
        private string property;

        /// <summary>
        /// Specify backing value used by Dictionary&lt;string, object&gt; to indicate selected option.
        /// </summary>
        /// <param name="property">Property name for backing value.</param>
        public SettingBackingValue(string property)
        {
            this.property = property;
        }

        /// <summary>
        /// Accessor to get/set backing value property name.
        /// </summary>
        public string BackingProperty
        {
            get => property;
            set => property = value;
        }
    }

    /// <summary>
    /// Specify bounds for numeric inputs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingNumericBounds : Attribute
    {
        private double minimum;
        private double maximum;
        private double increment;
        private int precision;

        /// <summary>
        /// Specify bounds for numeric inputs.
        /// </summary>
        /// <param name="minimum">Minimum allowed value.</param>
        /// <param name="maximum">Maximum allowed value.</param>
        /// <param name="increment">Increment between allowed values in slider/roller inputs.</param>
        /// <param name="precision">The number of digits to display for non integer values.</param>
        public SettingNumericBounds(double minimum, double maximum, double increment = 1.0, int precision = 1)
        {
            this.minimum = minimum;
            this.maximum = maximum;
            this.increment = increment;
            this.precision = precision;
        }

        /// <summary>
        /// Minimum allowed value.
        /// </summary>
        public double Minimum
        {
            get => minimum;
            set => minimum = value;
        }

        /// <summary>
        /// Maximum allowed value.
        /// </summary>
        public double Maximum
        {
            get => maximum;
            set => maximum = value;
        }

        /// <summary>
        /// Increment between allowed values in slider/roller inputs.
        /// </summary>
        public double Increment
        {
            get => increment;
            set => increment = value;
        }

        /// <summary>
        /// The number of digits to display for non integer values.
        /// </summary>
        public int Precision
        {
            get => precision;
            set => precision = value;
        }
    }
    #endregion

    #region BasicUI attributes
    /// <summary>
    /// Suggests default column width when building basic plugin grid UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ColumnSuggestedWidth : Attribute
    {
        /// <summary>
        /// Suggests default column width when building basic plugin grid UI.
        /// </summary>
        /// <param name="width">The suggested width of the annotated column.</param>
        public ColumnSuggestedWidth(int width)
        {
            Width = width;
        }

        /// <summary>
        /// The suggested width of the annotated column.
        /// </summary>
        public int Width { get; }
    }
    #endregion
}
