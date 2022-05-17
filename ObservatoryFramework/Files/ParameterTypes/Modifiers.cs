using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Modifiers
    {
        public string Label { get; init; }

        public object Value 
        { 
            get
            {
                if (!string.IsNullOrEmpty(ValueString))
                    return ValueString;
                else
                    return ValueNumeric;
            }

            init
            {
                if (value.GetType() == typeof(string))
                    ValueString = value.ToString();
                else
                    ValueNumeric = (double)value;
            }
        }

        public double OriginalValue { get; init; }

        [JsonConverter(typeof(Converters.IntBoolConverter))]
        public bool LessIsGood { get; init; }

        private double ValueNumeric;
        private string ValueString;
    }
}
