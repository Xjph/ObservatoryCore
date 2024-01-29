using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Observatory.Assets;
using Observatory.Framework;
using Observatory.Framework.Interfaces;

namespace Observatory.UI
{
    public partial class SettingsForm : Form
    {
        private readonly IObservatoryPlugin _plugin;
        private readonly List<int> _colHeight = new List<int>();
        private int _colWidth = 400;

        public SettingsForm(IObservatoryPlugin plugin)
        {
            InitializeComponent();
            _plugin = plugin;

            // Filtered to only settings without SettingIgnore attribute
            var attrib = _plugin.Settings.GetType().GetCustomAttribute<SettingSuggestedColumnWidth>();
            if (attrib != null && attrib.Width > 0)
            {
                int minScreenWidth = Screen.AllScreens.Min(s => s.Bounds.Width);
                _colWidth = Math.Min(attrib.Width, minScreenWidth / 2);
            }
            var settings = PluginManagement.PluginManager.GetSettingDisplayNames(plugin.Settings).Where(s => !Attribute.IsDefined(s.Key, typeof(SettingIgnore)));
            CreateControls(settings);

            
            Text = plugin.Name + " Settings";
            Icon = Resources.EOCIcon_Presized;
            ThemeManager.GetInstance.RegisterControl(this);
        }

        private void CreateControls(IEnumerable<KeyValuePair<PropertyInfo, string>> settings)
        {
            bool recentHalfCol = false;

            int settingsHeight = 0;
            
            var trackBottomEdge = (Control control) =>
            {
                var controlBottom = control.Location.Y + control.Height;
                if (controlBottom > settingsHeight)
                    settingsHeight = controlBottom;
            };


            foreach (var setting in settings)
            {
                // Reset the column tracking for checkboxes if this isn't a checkbox or explicitly requested
                // to start a new grouping of settings.
                int addedHeight = 35;
                var newGroup = Attribute.GetCustomAttribute(setting.Key, typeof(SettingNewGroup)) as SettingNewGroup;

                if (setting.Key.PropertyType.Name != "Boolean" || newGroup != null)
                {
                    if (recentHalfCol) _colHeight.Add(addedHeight);
                    recentHalfCol = false;

                    if (newGroup != null)
                    {
                        if (!string.IsNullOrEmpty(newGroup.Label))
                        {
                            var label = CreateGroupLabel(newGroup.Label);
                            label.Location = GetSettingPosition();

                            Controls.Add(label);
                            trackBottomEdge(label);
                            _colHeight.Add(label.Height);
                        }
                        else
                            _colHeight.Add(10);
                    }
                }

                switch (setting.Key.GetValue(_plugin.Settings))
                {
                    case bool:
                        var checkBox = CreateBoolSetting(setting);
                        addedHeight = recentHalfCol ? addedHeight : 0;
                        checkBox.Location = GetSettingPosition(recentHalfCol);

                        recentHalfCol = !recentHalfCol;

                        Controls.Add(checkBox);
                        trackBottomEdge(checkBox);
                        break;
                    case string:
                        var stringLabel = CreateSettingLabel(setting.Value);
                        var textBox = CreateStringSetting(setting.Key);
                        stringLabel.Location = GetSettingPosition();
                        textBox.Location = GetSettingPosition(true);

                        Controls.Add(stringLabel);
                        Controls.Add(textBox);
                        trackBottomEdge(textBox);
                        break;
                    case FileInfo:
                        var fileLabel = CreateSettingLabel(setting.Value);
                        var pathTextBox = CreateFilePathSetting(setting.Key);
                        var pathButton = CreateFileBrowseSetting(setting.Key, pathTextBox);

                        fileLabel.Location = GetSettingPosition();
                        pathTextBox.Location = GetSettingPosition(true);
                        _colHeight.Add(addedHeight);
                        pathButton.Location = GetSettingPosition(true);

                        Controls.Add(fileLabel);
                        Controls.Add(pathTextBox);
                        Controls.Add(pathButton);
                        trackBottomEdge(pathButton);
                        break;
                    case int:
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
                        intLabel.Location = GetSettingPosition();
                        intControl.Location = GetSettingPosition(true);

                        addedHeight = intControl.Height + 2;
                        intLabel.Height = intControl.Height;
                        intLabel.TextAlign = ContentAlignment.MiddleRight;

                        Controls.Add(intLabel);
                        Controls.Add(intControl);
                        trackBottomEdge(intControl);
                        break;
                    case double:
                        // We have one options for double values:
                        // 1) A numeric up/down (default otherwise, and is unbounded by default).
                        // Bounds can be set via the SettingNumericBounds attribute.
                        var doubleLabel = CreateSettingLabel(setting.Value);
                        Control doubleControl = CreateSettingNumericUpDownForDouble(setting.Key);
                        doubleLabel.Location = GetSettingPosition();
                        doubleControl.Location = GetSettingPosition(true);

                        addedHeight = doubleControl.Height + 2;
                        doubleLabel.Height = doubleControl.Height;
                        doubleLabel.TextAlign = ContentAlignment.MiddleRight;

                        Controls.Add(doubleLabel);
                        Controls.Add(doubleControl);
                        trackBottomEdge(doubleControl);
                        break;
                    case Action action:
                        var button = CreateSettingButton(setting.Value, action);

                        button.Location = GetSettingPosition();

                        Controls.Add(button);
                        addedHeight = button.Height;
                        trackBottomEdge(button);
                        break;
                    case Dictionary<string, object> dictSetting:
                        var dictLabel = CreateSettingLabel(setting.Value);
                        var dropdown = CreateSettingDropdown(setting.Key, dictSetting);
                       
                        dictLabel.Location = GetSettingPosition();
                        dropdown.Location = GetSettingPosition(true);
                        Controls.Add(dictLabel);
                        Controls.Add(dropdown);
                        trackBottomEdge(dropdown);
                        break;
                    default:
                        break;
                }
                _colHeight.Add(addedHeight);
            }
            Height = settingsHeight + 160;
            Width = _colWidth * 2 + 80;
        }

        private Point GetSettingPosition(bool secondCol = false)
        {
            return new Point(20 + (secondCol ? _colWidth + 20 : 0), 15 + _colHeight.Sum());
        }


        private Label CreateSettingLabel(string settingName)
        {
            Label label = new()
            {
                Text = settingName + ": ",
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                Width = _colWidth,
                ForeColor = Color.LightGray
            };

            return label;
        }

        private Label CreateGroupLabel(string groupLabel)
        {
            Label label = new()
            {
                Text = groupLabel,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Width = _colWidth * 2,
                ForeColor = Color.LightGray,
            };
            label.Font = new Font(label.Font.FontFamily, label.Font.Size + 1, FontStyle.Bold);
            label.Height += 10; // Add spacing.
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
                Width = _colWidth,
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
                Text = settingName,
                Width = Convert.ToInt32(_colWidth * 0.8),
                Height = 35,
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

            var minBound = Convert.ToInt32(bounds?.Minimum ?? 0);
            var maxBound = Convert.ToInt32(bounds?.Maximum ?? 100);

            var tickFrequency = maxBound - minBound >= 20 ? (maxBound - minBound) / 10 : 1;

            TrackBar trackBar = new()
            {
                Orientation = Orientation.Horizontal,
                TickStyle = TickStyle.Both,
                TickFrequency = tickFrequency,
                Width = _colWidth,
                Minimum = minBound,
                Maximum = maxBound,
            };

            trackBar.Value = (int?)setting.GetValue(_plugin.Settings) ?? 0;

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
                Width = _colWidth,
                Minimum = Convert.ToInt32(bounds?.Minimum ?? Int32.MinValue),
                Maximum = Convert.ToInt32(bounds?.Maximum ?? Int32.MaxValue),
                Increment = Convert.ToInt32(bounds?.Increment ?? 1)
            };

            numericUpDown.Value = (int?)setting.GetValue(_plugin.Settings) ?? 0;
            numericUpDown.ValueChanged += (sender, e) =>
            {
                setting.SetValue(_plugin.Settings, Convert.ToInt32(numericUpDown.Value));
                SaveSettings();
            };

            return numericUpDown;
        }

        private NumericUpDown CreateSettingNumericUpDownForDouble(PropertyInfo setting)
        {
            SettingNumericBounds? bounds = (SettingNumericBounds?)System.Attribute.GetCustomAttribute(setting, typeof(SettingNumericBounds));
            NumericUpDown numericUpDown = new()
            {
                Width = _colWidth,
                Minimum = Convert.ToDecimal(bounds?.Minimum ?? Double.MinValue),
                Maximum = Convert.ToDecimal(bounds?.Maximum ?? Double.MaxValue),
                Increment = Convert.ToDecimal(bounds?.Increment ?? 1.0),
                DecimalPlaces = bounds?.Precision ?? 1,
            };

            numericUpDown.Value = Convert.ToDecimal(setting.GetValue(_plugin.Settings) ?? 0);
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
                Height = 30,
                Width = _colWidth,
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
                Width = _colWidth,
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
                Width = _colWidth,
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
                Height = 35,
                Width = _colWidth / 2,
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

        private void SaveSettings()
        {
            PluginManagement.PluginManager.GetInstance.SaveSettings(_plugin, _plugin.Settings);
        }

        private void PluginSettingsCloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
