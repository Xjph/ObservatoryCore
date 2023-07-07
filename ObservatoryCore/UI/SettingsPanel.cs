using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

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
            var settings = PluginManagement.PluginManager.GetSettingDisplayNames(plugin.Settings).Where(s => !Attribute.IsDefined(s.Key, typeof (SettingIgnore)));
            CreateControls(settings);

        }

        private void CreateControls(IEnumerable<KeyValuePair<PropertyInfo, string>> settings)
        {
            int controlRow = 0;
            bool recentHalfCol = false;

            foreach (var setting in settings)
            {
                // Reset the column tracking for checkboxes if this isn't a checkbox
                if (setting.Key.PropertyType.Name != "Boolean" && setting.Key.PropertyType.Name != "Button")
                    recentHalfCol = false;

                switch (setting.Key.GetValue(_plugin.Settings))
                {
                    case bool:
                        var checkBox = CreateBoolSetting(setting);
                        controlRow += recentHalfCol ? 0 : 1;
                        checkBox.Location = GetSettingPosition(controlRow, recentHalfCol);

                        recentHalfCol = !recentHalfCol;

                        Controls.Add(checkBox);
                        break;
                    case string:
                        var stringLabel = CreateSettingLabel(setting.Value);
                        var textBox = CreateStringSetting(setting.Key);
                        controlRow++;
                        stringLabel.Location = GetSettingPosition(controlRow);
                        textBox.Location = GetSettingPosition(controlRow, true);
                        
                        Controls.Add(stringLabel);
                        Controls.Add(textBox);

                        break;
                    case FileInfo:
                        var fileLabel = CreateSettingLabel(setting.Value);
                        var pathTextBox = CreateFilePathSetting(setting.Key);
                        var pathButton = CreateFileBrowseSetting(setting.Key, pathTextBox);

                        controlRow++;

                        fileLabel.Location = GetSettingPosition(controlRow);
                        pathTextBox.Location = GetSettingPosition(controlRow, true);
                        pathButton.Location = GetSettingPosition(++controlRow, true);

                        Controls.Add(fileLabel);
                        Controls.Add(pathTextBox);
                        Controls.Add(pathButton);

                        break;
                    case int:
                        // We have two options for integer values:
                        // 1) A slider (explicit by way of the SettingIntegerUseSlider attribute and bounded to 0..100 by default)
                        // 2) A numeric up/down (default otherwise, and is unbounded by default).
                        // Bounds for both can be set via the SettingNumericBounds attribute, only the up/down uses Increment.
                        var intLabel = CreateSettingLabel(setting.Value);
                        Control intControl;
                        controlRow++;
                        if (System.Attribute.IsDefined(setting.Key, typeof(SettingNumericUseSlider)))
                        {
                            intControl = CreateSettingTrackbar(setting.Key);
                        }
                        else
                        {
                            intControl = CreateSettingNumericUpDown(setting.Key);
                        }
                        intLabel.Location = GetSettingPosition(controlRow);
                        intControl.Location = GetSettingPosition(controlRow, true);

                        Controls.Add(intLabel);
                        Controls.Add(intControl);
                        break;
                    case Action action:
                        var button = CreateSettingButton(setting.Value, action);

                        controlRow += recentHalfCol ? 0 : 1;
                        button.Location = GetSettingPosition(controlRow, recentHalfCol);
                        recentHalfCol = !recentHalfCol;

                        Controls.Add(button);
                        break;
                    case Dictionary<string, object> dictSetting:
                        var dictLabel = CreateSettingLabel(setting.Value);
                        var dropdown = CreateSettingDropdown(setting.Key, dictSetting);
                        controlRow++;

                        dictLabel.Location = GetSettingPosition(controlRow);
                        dropdown.Location = GetSettingPosition(controlRow, true);
                        Controls.Add(dictLabel);
                        Controls.Add(dropdown);
                        break;
                    default:
                        break;
                }
            }
            Height = 3 + controlRow * 29;
        }

        private static Point GetSettingPosition(int rowNum, bool secondCol = false)
        {
            return new Point(10 + (secondCol ? 200 : 0), -26 + rowNum * 29);
        }
        

        private Label CreateSettingLabel(string settingName)
        {
            Label label = new()
            {
                Text = settingName + ": ",
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                Width = 200,
                ForeColor = Color.LightGray
            };

            return label;
        }

        private ComboBox CreateSettingDropdown(PropertyInfo setting, Dictionary<string, object> dropdownItems)
        {
            var backingValueName = (SettingBackingValue?)Attribute.GetCustomAttribute(setting, typeof(SettingBackingValue));

            var backingValue = from s in PluginManagement.PluginManager.GetSettingDisplayNames(_plugin.Settings)
                               where s.Value == backingValueName?.BackingProperty
                               select s.Key;

            if (backingValue.Count() != 1)
                throw new($"{_plugin.ShortName}: Dictionary settings must have exactly one backing value.");

            ComboBox comboBox = new()
            {
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            comboBox.Items.AddRange(dropdownItems.OrderBy(s => s.Key).Select(s => s.Key).ToArray());

            string? currentSelection = backingValue.First().GetValue(_plugin.Settings)?.ToString();

            if (currentSelection?.Length > 0)
            {
                comboBox.SelectedItem = currentSelection;
            }

            comboBox.SelectedValueChanged += (sender, e) =>
            {
                backingValue.First().SetValue(_plugin.Settings, comboBox.SelectedItem.ToString());
                SaveSettings();
            };

            return comboBox;
        }

        private Button CreateSettingButton(string settingName, Action action)
        {
            Button button = new()
            {
                Text = settingName
            };

            button.Click += (sender, e) =>
            {
                action.Invoke();
                SaveSettings();
            };

            return button;
        }

        private TrackBar CreateSettingTrackbar(PropertyInfo setting)
        {
            SettingNumericBounds? bounds = (SettingNumericBounds?)System.Attribute.GetCustomAttribute(setting, typeof(SettingNumericBounds));
            TrackBar trackBar = new ()
            {
                Orientation = Orientation.Horizontal,
                TickStyle = TickStyle.Both,
                Width = 200, 
                Minimum = Convert.ToInt32(bounds?.Minimum ?? 0),
                Maximum = Convert.ToInt32(bounds?.Maximum ?? 100)
            };

            trackBar.Value = (int?)setting.GetValue(_plugin.Settings) ?? 0;

            trackBar.ValueChanged += (sender, e) =>
            {
                setting.SetValue(_plugin.Settings, trackBar.Value);
                SaveSettings();
            };

            return trackBar;
        }

        private NumericUpDown CreateSettingNumericUpDown(PropertyInfo setting)
        {
            SettingNumericBounds? bounds = (SettingNumericBounds?)System.Attribute.GetCustomAttribute(setting, typeof(SettingNumericBounds));
            NumericUpDown numericUpDown = new()
            {
                
                Width = 200,
                Minimum = Convert.ToInt32(bounds?.Minimum ?? Int32.MinValue),
                Maximum = Convert.ToInt32(bounds?.Maximum ?? Int32.MaxValue),
                Increment = Convert.ToInt32(bounds?.Increment ?? 1)
            };

            numericUpDown.Value = (int?)setting.GetValue(_plugin.Settings) ?? 0;

            numericUpDown.ValueChanged += (sender, e) =>
            {
                setting.SetValue(_plugin.Settings, numericUpDown.Value);
                SaveSettings();
            };

            return numericUpDown;
        }

        private CheckBox CreateBoolSetting(KeyValuePair<PropertyInfo, string> setting)
        {
            CheckBox checkBox = new()
            {
                Text = setting.Value,
                TextAlign= System.Drawing.ContentAlignment.MiddleLeft,
                Checked = (bool?)setting.Key.GetValue(_plugin.Settings) ?? false,
                Width = 200,
                ForeColor = Color.LightGray
            };

            checkBox.CheckedChanged += (sender, e) =>
            {
                setting.Key.SetValue(_plugin.Settings, checkBox.Checked);
                SaveSettings();
            };

            return checkBox;
        }

        private TextBox CreateStringSetting(PropertyInfo setting)
        {
            TextBox textBox = new()
            {
                Text = (setting.GetValue(_plugin.Settings) ?? String.Empty).ToString(),
                Width = 200
            };

            textBox.TextChanged += (object? sender, EventArgs e) =>
            {
                setting.SetValue(_plugin.Settings, textBox.Text);
                SaveSettings();
            };

            return textBox;
        }

        private TextBox CreateFilePathSetting(PropertyInfo setting)
        {
            var fileInfo = (FileInfo?)setting.GetValue(_plugin.Settings);

            TextBox textBox = new()
            {
                Text = fileInfo?.FullName ?? string.Empty,
                Width = 200
            };

            textBox.TextChanged += (object? sender, EventArgs e) =>
            {
                setting.SetValue(_plugin.Settings, new FileInfo(textBox.Text));
                SaveSettings();
            };

            return textBox;
        }

        private Button CreateFileBrowseSetting(PropertyInfo setting, TextBox textBox)
        {
            Button button = new()
            {
                Text = "Browse"
            };

            button.Click += (object? sender, EventArgs e) =>
            {
                var currentDir = ((FileInfo?)setting.GetValue(_plugin.Settings))?.DirectoryName;

                OpenFileDialog ofd = new OpenFileDialog()
                { 
                    Title = "Select File...",
                    Filter = "Lua files (*.lua)|*.lua|All files (*.*)|*.*",
                    FilterIndex = 0,
                    InitialDirectory = currentDir ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                var browseResult = ofd.ShowDialog();

                if (browseResult == DialogResult.OK)
                {
                    textBox.Text = ofd.FileName;
                }
            };

            return button;
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

        private void SaveSettings()
        {
            PluginManagement.PluginManager.GetInstance.SaveSettings(_plugin, _plugin.Settings);
        }
    }
}
