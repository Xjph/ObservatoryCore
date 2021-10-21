using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework
{
    public class SettingDisplayName : Attribute
    {
        private string name;

        public SettingDisplayName(string name)
        {
            this.name = name;
        }

        public string DisplayName
        {
            get => name;
            set => name = value;
        }
    }

    public class SettingIgnore : Attribute
    { }

    public class SettingNumericUseSlider : Attribute
    { }

    public class SettingNumericBounds : Attribute
    {
        private double minimum;
        private double maximum;
        private double increment;

        public SettingNumericBounds(double minimum, double maximum, double increment = 1.0)
        {
            this.minimum = minimum;
            this.maximum = maximum;
            this.increment = increment;
        }

        public double Minimum
        {
            get => minimum;
            set => minimum = value;
        }
        public double Maximum
        {
            get => maximum;
            set => maximum = value;
        }
        public double Increment
        {
            get => increment;
            set => increment = value;
        }
    }
}
