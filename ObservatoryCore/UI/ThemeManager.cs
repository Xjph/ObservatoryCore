using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Observatory.UI
{
    internal class ThemeManager
    {
        public static ThemeManager GetInstance
        {
            get
            {
                return _instance.Value;
            }
        }
        private static HashSet<string> _excludedControlNames = new()
        {
            "ColourButton",
        };
        private static readonly Lazy<ThemeManager> _instance = new(() => new ThemeManager());

        private ThemeManager()
        {
            controls = new();
            menuItems = new();
            Themes = new()
            {
                { "Dark", DarkTheme },
                { "Light", LightTheme }
            };
            SelectedTheme = "Light"; // Must be Light for initializing the Light theme.
        }

        private readonly List<Control> controls;
        private readonly List<ToolStripMenuItem> menuItems;

        public List<string> GetThemes
        {
            get => Themes.Keys.ToList();
        }

        public string CurrentTheme
        {
            get => SelectedTheme;

            set
            {
                if (Themes.ContainsKey(value))
                {
                    SelectedTheme = value;
                    foreach (var control in controls)
                    {
                        ApplyTheme(control);
                    }
                    foreach (var menuItem in menuItems)
                    {
                        ApplyTheme(menuItem);
                    }
                }
            }
        }

        public void RegisterControl(Control control)
        {
            // First time registering a control, build the "light" theme based
            // on defaults of all root objects (ie. the ObsCore window and plugin panels).
            if (control.Parent == null)
            {
                SaveTheme(control, LightTheme);
            }

            controls.Add(control);
            ApplyTheme(control);
            //if (control.HasChildren)
            //    foreach (Control child in control.Controls)
            //    {
            //        if (_excludedControlNames.Contains(child.Name)) continue;
            //        RegisterControl(child);
            //    }
        }

        // This doesn't inherit from Control? Seriously?
        public void RegisterControl(ToolStripMenuItem toolStripMenuItem)
        {
            if (menuItems.Count == 0)
            {
                SaveThemeControl(toolStripMenuItem, LightTheme);
            }
            menuItems.Add(toolStripMenuItem);
            ApplyTheme(toolStripMenuItem);
        }

        private void SaveTheme(Control control, Dictionary<string, Color> theme)
        {
            Control rootControl = control;
            while (rootControl.Parent != null)
            {
                rootControl = rootControl.Parent;
            }

            SaveThemeControl(rootControl, theme);
        }

        private void SaveThemeControl(Object control, Dictionary<string, Color> theme)
        {
            var properties = control.GetType().GetProperties();
            var colorProperties = properties.Where(p => p.PropertyType == typeof(Color));

            foreach (var colorProperty in colorProperties)
            {
                string controlKey = control.GetType().Name + "." + colorProperty.Name;
                if (!theme.ContainsKey(controlKey))
                {
                    theme.Add(controlKey, (Color)colorProperty.GetValue(control)!);
                }
            }

            if (control is Control)
            {
                foreach (Control child in ((Control)control).Controls)
                {
                    SaveThemeControl(child, theme);
                }
            }
        }

        public void DeRegisterControl(Control control)
        { 
            //if (control.HasChildren)
            //    foreach (Control child in control.Controls)
            //    {
            //        DeRegisterControl(child);
            //    }
            controls.Remove(control); 
        }

        private void ApplyTheme(Object control)
        {
            var controlType = control.GetType();

            var theme = Themes.ContainsKey(SelectedTheme)
                ? Themes[SelectedTheme] : Themes["Light"];

            foreach (var property in controlType.GetProperties().Where(p => p.PropertyType == typeof(Color)))
            {
                string themeControl = Themes[SelectedTheme].ContainsKey(controlType.Name + "." + property.Name)
                    ? controlType.Name
                    : "Default";

                if (Themes[SelectedTheme].ContainsKey(themeControl + "." + property.Name))
                    property.SetValue(control, Themes[SelectedTheme][themeControl + "." + property.Name]);
            }

            Control actualControl = control as Control;
            if (actualControl != null && actualControl.HasChildren)
            {
                foreach (Control child in actualControl.Controls)
                {
                    ApplyTheme(child);
                }
            }
        }

        private Dictionary<string, Dictionary<string, Color>> Themes;

        private string SelectedTheme;

        private Dictionary<string, Color> LightTheme = new Dictionary<string, Color>();

        static private Dictionary<string, Color> DarkTheme = new Dictionary<string, Color>
        {
            {"Default.ForeColor", Color.LightGray },
            {"Default.BackColor", Color.Black },
            {"Button.ForeColor", Color.LightGray },
            {"Button.BackColor", Color.DimGray }
        };
    }
}
