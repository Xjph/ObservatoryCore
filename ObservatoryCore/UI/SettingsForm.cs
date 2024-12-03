using System.Data;
using System.Reflection;
using System.Windows.Forms;
using Observatory.Assets;
using Observatory.Framework;
using Observatory.Framework.Interfaces;

namespace Observatory.UI
{
    public partial class SettingsForm : Form
    {
        private readonly IObservatoryPlugin _plugin;
        private readonly double _scale;

        public SettingsForm(IObservatoryPlugin plugin)
        {
            InitializeComponent();
            _plugin = plugin;
            _scale = (double)DeviceDpi / 96;

            var settings = PluginManagement.PluginManager.GetSettingDisplayNames(plugin.Settings).Where(s => !Attribute.IsDefined(s.Key, typeof(SettingIgnore)));
            CreateControls(settings);

            
            Text = plugin.Name + " Settings";
            Icon = Resources.EOCIcon_Presized;
            ThemeManager.GetInstance.RegisterControl(this);

            var currentScreen = Screen.FromControl(this);

            var vScrollMargin = SettingsFlowPanel.DisplayRectangle.Height - SettingsFlowPanel.ClientRectangle.Height;
            var hScrollMargin = SettingsFlowPanel.DisplayRectangle.Width - SettingsFlowPanel.ClientRectangle.Width;
            if (vScrollMargin > 0)
            {
                Height = Math.Min(Height + vScrollMargin, currentScreen.WorkingArea.Height);
            }
            if (hScrollMargin > 0)
            {
                Width = Math.Min(Width + hScrollMargin, currentScreen.WorkingArea.Width);
            }   

            // Recenter after size modified by adding controls.
            var midPoint = new Point(currentScreen.WorkingArea.Width / 2, currentScreen.WorkingArea.Height / 2);
            Location = new Point(midPoint.X - Width / 2, midPoint.Y - Height / 2);
            
        }

        private TableLayoutPanel WrapToSingleRow(Control[] controls)
        {
            var table = new TableLayoutPanel()
            {
                Height = (int)(33 * _scale),
                AutoSize = true
            };
            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].Anchor = AnchorStyles.Right;
                table.Controls.Add(controls[i]);
                table.SetRow(controls[i], 0);
                table.SetColumn(controls[i], i);
            }
            return table;
        }

        private static void AlignControls(List<Control> controls)
        {
            var maxWidth = controls.Max(control => control.Width);
            foreach (var control in controls)
            {
                control.AutoSize = false;
                control.Width = maxWidth;
            }
            controls.Clear();
        }

        private static void InsertFlowBreak(FlowLayoutPanel flowLayoutPanel)
        {
            if (flowLayoutPanel.Controls.Count > 0)
            {
                var lastControl = flowLayoutPanel.Controls[flowLayoutPanel.Controls.Count - 1];
                flowLayoutPanel.SetFlowBreak(lastControl, true);
            }
        }

        private void CreateControls(IEnumerable<KeyValuePair<PropertyInfo, string>> settings)
        {
            List<Control> groupedControls = [];
            bool largeGroup = false;
            int largeWidth = 0;
            foreach (var setting in settings)
            {
                SettingNewGroup? newGroup = Attribute.GetCustomAttribute(setting.Key, typeof(SettingNewGroup)) as SettingNewGroup;
                if (newGroup != null)
                {
                    if (groupedControls.Count > 0)
                    {
                        if (groupedControls.Count > 5)
                        {
                            largeGroup = true;
                            var maxWidth = groupedControls.Max(control => control.Width);
                            if (maxWidth > largeWidth)
                                largeWidth = maxWidth;
                        }
                        AlignControls(groupedControls);
                    }

                    if (!string.IsNullOrEmpty(newGroup.Label))
                    {
                        var label = CreateGroupLabel(newGroup.Label);
              
                        if (SettingsFlowPanel.Controls.Count > 0)
                        {
                            InsertFlowBreak(SettingsFlowPanel);
                        }

                        SettingsFlowPanel.Controls.Add(label);
                        InsertFlowBreak(SettingsFlowPanel);
                    }
                }

                bool handled = true;

                switch (setting.Key.GetValue(_plugin.Settings))
                {
                    case bool:
                        var checkBox = CreateBoolSetting(setting);
                        SettingsFlowPanel.Controls.Add(checkBox);
                        break;
                    case string:
                        InsertFlowBreak(SettingsFlowPanel);
                        var stringLabel = CreateSettingLabel(setting.Value);
                        var textBox = CreateStringSetting(setting.Key);
                        SettingsFlowPanel.Controls.Add(WrapToSingleRow([stringLabel, textBox]));
                        InsertFlowBreak(SettingsFlowPanel);
                        break;
                    case FileInfo:
                        InsertFlowBreak(SettingsFlowPanel);
                        var fileLabel = CreateSettingLabel(setting.Value);
                        var pathTextBox = CreateFilePathSetting(setting.Key);
                        var pathButton = CreateFileBrowseSetting(setting.Key, pathTextBox);
                        fileLabel.Height = pathTextBox.Height;
                        fileLabel.TextAlign = ContentAlignment.MiddleRight;
                        SettingsFlowPanel.Controls.Add(WrapToSingleRow([fileLabel, pathTextBox]));
                        SettingsFlowPanel.Controls.Add(pathButton);
                        InsertFlowBreak(SettingsFlowPanel);
                        break;
                    case DirectoryInfo:
                        InsertFlowBreak(SettingsFlowPanel);
                        var dirLabel = CreateSettingLabel(setting.Value);
                        var dirTextBox = CreateDirPathSetting(setting.Key);
                        var dirButton = CreateDirBrowseSetting(setting.Key, dirTextBox);
                        SettingsFlowPanel.Controls.Add(WrapToSingleRow([dirLabel, dirTextBox]));
                        SettingsFlowPanel.Controls.Add(dirButton);
                        InsertFlowBreak(SettingsFlowPanel);
                        break;
                    case int:
                        InsertFlowBreak(SettingsFlowPanel);
                        // We have two options for integer values:
                        // 1) A slider (explicit by way of the SettingNumericUseSlider attribute and bounded to 0..100 by default)
                        // 2) A numeric up/down (default otherwise, and is unbounded by default).
                        // Bounds for both can be set via the SettingNumericBounds attribute, only the up/down uses Increment.
                        var intLabel = CreateSettingLabel(setting.Value);
                        Control intControl;
                        if (Attribute.IsDefined(setting.Key, typeof(SettingNumericUseSlider)))
                        {
                            intControl = CreateSettingTrackbar(setting.Key);
                        }
                        else
                        {
                            intControl = CreateSettingNumericUpDownForInt(setting.Key);
                        }
                        intLabel.AutoSize = true;
                        intControl.Height = intLabel.Height;
                        SettingsFlowPanel.Controls.Add(WrapToSingleRow([intLabel, intControl]));
                        InsertFlowBreak(SettingsFlowPanel);
                        break;
                    case double:
                        InsertFlowBreak(SettingsFlowPanel);
                        // We have one options for double values:
                        // 1) A numeric up/down (default otherwise, and is unbounded by default).
                        // Bounds can be set via the SettingNumericBounds attribute.
                        var doubleLabel = CreateSettingLabel(setting.Value);
                        Control doubleControl = CreateSettingNumericUpDownForDouble(setting.Key);
                        doubleLabel.AutoSize = true;
                        doubleLabel.Height = doubleControl.Height;
                        SettingsFlowPanel.Controls.Add(WrapToSingleRow([doubleLabel, doubleControl]));
                        InsertFlowBreak(SettingsFlowPanel);
                        break;
                    case Action action:
                        var button = CreateSettingButton(setting.Value, action);
                        SettingsFlowPanel.Controls.Add(button);
                        break;
                    case Dictionary<string, object> dictSetting:
                        InsertFlowBreak(SettingsFlowPanel);
                        var dictLabel = CreateSettingLabel(setting.Value);
                        var dropdown = CreateSettingDropdown(setting.Key, dictSetting);

                        SettingsFlowPanel.Controls.Add(WrapToSingleRow([dictLabel, dropdown]));
                        InsertFlowBreak(SettingsFlowPanel);
                        break;
                    default:
                        handled = false;
                        break;
                }
                if (handled)
                    groupedControls.Add(SettingsFlowPanel.Controls[SettingsFlowPanel.Controls.Count - 1]);
            }

            if (groupedControls.Count > 0)
            {
                if (groupedControls.Count > 5)
                {
                    largeGroup = true;
                    var maxWidth = groupedControls.Max(control => control.Width);
                    if (maxWidth > largeWidth)
                        largeWidth = maxWidth;
                }
                AlignControls(groupedControls);
            }

            // If we have a group with a lot of items allow for multiple columns
            if (largeGroup)
            {
                Width = largeWidth * 2 // Two columns
                    + Width - SettingsFlowPanel.Width // Plus margin
                    + 40; // And allowance for scrollbar
            }
        }

        private static Label CreateSettingLabel(string settingName)
        {
            // If this is long, add a line-break
            bool ohLawd = settingName.Length > 30;
            if (ohLawd)
            {
                var settingWords = settingName.Split(' ');

                var halfwayIndex = settingWords.Length / 2;

                settingName = string.Join(' ', settingWords[0..halfwayIndex])
                    + Environment.NewLine
                    + string.Join(' ', settingWords[halfwayIndex..^0]);
            }

            Label label = new()
            {
                Text = settingName + ": ",
                ForeColor = Color.LightGray,
                AutoSize = true
            };

            return label;
        }

        private Label CreateGroupLabel(string groupLabel)
        {
            Label label = new()
            {
                Text = groupLabel,
                TextAlign = ContentAlignment.BottomLeft,
                ForeColor = Color.LightGray,
                AutoSize = true,
                Padding = new Padding(0, 5, 0, 0)
            };
            label.Font = new Font(label.Font.FontFamily, label.Font.Size + 1, FontStyle.Bold);
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
                AutoSize = true,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var comboWidth = dropdownItems.Max(item => TextRenderer.MeasureText(item.Key, comboBox.Font).Width);

            // Slightly wider to account for arrow at end of dropdown.
            comboBox.Width = comboWidth + (int)(20 * _scale);

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
                Text = settingName,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                MinimumSize = new Size(0, (int)(25 * _scale))
            };

            button.FlatAppearance.BorderSize = 0;
            button.Click += (sender, e) =>
            {
                action.Invoke();
                SaveSettings();
            };

            return button;
        }

        private TrackBar CreateSettingTrackbar(PropertyInfo setting)
        {
            SettingNumericBounds? bounds = (SettingNumericBounds?)Attribute.GetCustomAttribute(setting, typeof(SettingNumericBounds));

            var minBound = Convert.ToInt32(bounds?.Minimum ?? 0);
            var maxBound = Convert.ToInt32(bounds?.Maximum ?? 100);

            var tickFrequency = maxBound - minBound >= 20 ? (maxBound - minBound) / 10 : 1;

            TrackBar trackBar = new()
            {
                Orientation = Orientation.Horizontal,
                TickStyle = TickStyle.Both,
                TickFrequency = tickFrequency,
                AutoSize = true,
                Minimum = minBound,
                Maximum = maxBound,
            };

            trackBar.Value = Math.Clamp((int?)setting.GetValue(_plugin.Settings) ?? 0, minBound, maxBound);

            trackBar.ValueChanged += (sender, e) =>
            {
                setting.SetValue(_plugin.Settings, trackBar.Value);
                SaveSettings();
            };

            return trackBar;
        }

        private NumericUpDown CreateSettingNumericUpDownForInt(PropertyInfo setting)
        {
            SettingNumericBounds? bounds = (SettingNumericBounds?)Attribute.GetCustomAttribute(setting, typeof(SettingNumericBounds));
            NumericUpDown numericUpDown = new()
            {
                AutoSize = true,
                Minimum = Convert.ToInt32(bounds?.Minimum ?? Int32.MinValue),
                Maximum = Convert.ToInt32(bounds?.Maximum ?? Int32.MaxValue),
                Increment = Convert.ToInt32(bounds?.Increment ?? 1)
            };

            numericUpDown.Value = Math.Clamp((int?)setting.GetValue(_plugin.Settings) ?? 0, numericUpDown.Minimum, numericUpDown.Maximum);
            numericUpDown.ValueChanged += (sender, e) =>
            {
                setting.SetValue(_plugin.Settings, Convert.ToInt32(numericUpDown.Value));
                SaveSettings();
            };

            return numericUpDown;
        }

        private NumericUpDown CreateSettingNumericUpDownForDouble(PropertyInfo setting)
        {
            SettingNumericBounds? bounds = (SettingNumericBounds?)Attribute.GetCustomAttribute(setting, typeof(SettingNumericBounds));
            NumericUpDown numericUpDown = new()
            {
                AutoSize = true,
                Minimum = Convert.ToDecimal(bounds?.Minimum ?? Double.MinValue),
                Maximum = Convert.ToDecimal(bounds?.Maximum ?? Double.MaxValue),
                Increment = Convert.ToDecimal(bounds?.Increment ?? 1.0),
                DecimalPlaces = bounds?.Precision ?? 1,
            };

            numericUpDown.Value = Math.Clamp(Convert.ToDecimal(setting.GetValue(_plugin.Settings) ?? 0), numericUpDown.Minimum, numericUpDown.Maximum);
            numericUpDown.ValueChanged += (sender, e) =>
            {
                setting.SetValue(_plugin.Settings, Convert.ToDouble(numericUpDown.Value));
                SaveSettings();
            };

            return numericUpDown;
        }

        private CheckBox CreateBoolSetting(KeyValuePair<PropertyInfo, string> setting)
        {
            CheckBox checkBox = new()
            {
                Text = setting.Value,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Checked = (bool?)setting.Key.GetValue(_plugin.Settings) ?? false,
                AutoSize = true,
                ForeColor = Color.LightGray,
                MinimumSize = new(0, (int)(25 * _scale))
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
                AutoSize = true
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
                AutoSize = true,
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
                Text = "Browse",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                MinimumSize = new Size(0, (int)(25 * _scale))
            };

            button.FlatAppearance.BorderSize = 0;
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

        private TextBox CreateDirPathSetting(PropertyInfo setting)
        {
            var dirInfo = (DirectoryInfo?)setting.GetValue(_plugin.Settings);

            TextBox textBox = new()
            {
                Text = dirInfo?.FullName ?? string.Empty,
                AutoSize = true,
                Width = 200
            };

            textBox.TextChanged += (object? sender, EventArgs e) =>
            {
                setting.SetValue(_plugin.Settings, new DirectoryInfo(textBox.Text));
                SaveSettings();
            };

            return textBox;
        }

        private Button CreateDirBrowseSetting(PropertyInfo setting, TextBox textBox)
        {
            Button button = new()
            {
                Text = "Browse",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                MinimumSize = new Size(0, (int)(25 * _scale))
            };

            button.FlatAppearance.BorderSize = 0;
            button.Click += (object? sender, EventArgs e) =>
            {
                var currentDir = ((DirectoryInfo?)setting.GetValue(_plugin.Settings))?.FullName;

                FolderBrowserDialog fbd = new FolderBrowserDialog()
                {
                    Description = "Select File...",
                    InitialDirectory = currentDir ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    UseDescriptionForTitle = true                    
                };

                var browseResult = fbd.ShowDialog();

                if (browseResult == DialogResult.OK)
                {
                    textBox.Text = fbd.SelectedPath;
                }
            };

            return button;
        }

        private void SaveSettings()
        {
            PluginManagement.PluginManager.GetInstance.SaveSettings(_plugin);
        }

        private void PluginSettingsCloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
