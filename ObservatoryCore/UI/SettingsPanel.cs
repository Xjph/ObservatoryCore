using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.UI
{
    internal class SettingsPanel : Panel
    {
        public Label Header;
        private IObservatoryPlugin _plugin;
        private Action<Control, CoreForm.AdjustmentDirection> _adjustPanelsBelow;

        internal SettingsPanel(IObservatoryPlugin plugin, Action<Control, CoreForm.AdjustmentDirection> adjustPanelsBelow) : base()
        {
            Header = CreateHeader(plugin.Name);
            _plugin = plugin;
            _adjustPanelsBelow = adjustPanelsBelow;

            // Filtered to only settings without SettingIgnore attribute
            var settings = PluginManagement.PluginManager.GetSettingDisplayNames(plugin).Where(s => !Attribute.IsDefined(s.Key, typeof (SettingIgnore)));
            CreateControls(settings);

        }

        private void CreateControls(IEnumerable<KeyValuePair<PropertyInfo, string>> settings)
        {
            int controlRow = 0;
            bool nextColumn = true;

            // Handle bool (checkbox) settings first and keep them grouped together
            foreach (var setting in settings.Where(s => s.Key.PropertyType == typeof(bool)))
            {
                CheckBox checkBox = new()
                {
                    Text = setting.Value,
                    Checked = (bool?)setting.Key.GetValue(_plugin.Settings) ?? false
                };

                checkBox.CheckedChanged += (object? _, EventArgs _) =>
                {
                    setting.Key.SetValue(_plugin.Settings, checkBox.Checked);
                    PluginManagement.PluginManager.GetInstance.SaveSettings(_plugin, _plugin.Settings);
                };

                checkBox.Location = new Point(nextColumn ? 10 : 130, 3 + controlRow * 29);
                controlRow += nextColumn ? 0 : 1;
                nextColumn = !nextColumn;

                Controls.Add(checkBox);
            }

            // Then the rest
            foreach (var setting in settings.Where(s => s.Key.PropertyType != typeof(bool)))
            {
                
            }
        }

        private Label CreateHeader(string pluginName)
        {
            var headerLabel = new Label()
            {
                Text = "❯ " + pluginName,
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.White
            };

            headerLabel.Click += HeaderLabel_Click;

            return headerLabel;
        }

        private void HeaderLabel_Click(object? _, EventArgs e)
        {
            this.Parent?.SuspendLayout();
            if (Header.Text[0] == '❯')
            {
                Header.Text = Header.Text.Replace('❯', '⌵');
                this.Visible = true;
                _adjustPanelsBelow.Invoke(this, CoreForm.AdjustmentDirection.Down);
            }
            else
            {
                Header.Text = Header.Text.Replace('⌵', '❯');
                this.Visible = false;
                _adjustPanelsBelow.Invoke(this, CoreForm.AdjustmentDirection.Up);
            }
            this.Parent?.ResumeLayout();
        }
    }
}
