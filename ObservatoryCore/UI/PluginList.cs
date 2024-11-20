using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using Observatory.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Observatory.UI
{
    internal class PluginList : TableLayoutPanel
    {
        private Label _title;

        internal PluginList(IEnumerable<IObservatoryPlugin> plugins)
        {
            ColumnCount = 7;
            _title = new() 
            { 
                Text = "Plugins",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(Font.FontFamily, Font.Size * 1.2f, FontStyle.Bold)
            };
            AddWithLocation(_title, 0, 0);
            SetColumnSpan(_title, 2);

            Label createHeaderLabel(string text)
            {
                return new Label()
                {
                    Text = text,
                    Font = new Font(Font, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft
                };
            };

            Label nameHeader = createHeaderLabel("Name");
            Label authorHeader = createHeaderLabel("Author");
            Label typeHeader = createHeaderLabel("Type");
            Label versionHeader = createHeaderLabel("Version");
            Label statusHeader = createHeaderLabel("Status");
            Label enabledHeader = createHeaderLabel("Enabled");
            enabledHeader.TextAlign = ContentAlignment.MiddleCenter;

            AddWithLocation(nameHeader, 1, 0);
            AddWithLocation(authorHeader, 1, 1);
            AddWithLocation(typeHeader, 1, 2);
            AddWithLocation(versionHeader, 1, 3);
            AddWithLocation(statusHeader, 1, 4);
            AddWithLocation(enabledHeader, 1, 5);

            // Silly
            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            int row = 2;

            var enabledPlugins = GetEnabledPlugins();

            foreach (var plugin in plugins)
            {
                var typeString = string.Empty;
                bool worker = plugin is IObservatoryWorker _;
                bool notifier = plugin is IObservatoryNotifier _;
                if (worker)
                {
                    typeString += "Worker";
                }
                if (worker && notifier)
                {
                    typeString += ", ";
                }
                if (notifier)
                {
                    typeString += "Notifier";
                }

                Label createLineLabel(string text)
                {
                    return new Label()
                    {
                        Text = text,
                        Dock = DockStyle.Fill,
                        Padding = new(8),
                        AutoSize = true
                    };
                };

                var pluginStatusValue = PluginManager.GetInstance.GetPluginStatus(plugin);

                Label pluginName = createLineLabel(plugin.Name);
                Label pluginAuthor = createLineLabel(plugin.AboutInfo?.AuthorName ?? "(not provided)");
                Label pluginVersion = createLineLabel(plugin.Version);
                Label pluginType = createLineLabel(typeString);
                Label pluginStatus = createLineLabel(PluginStatusString(pluginStatusValue));
                
                Button pluginSettings = new() 
                { 
                    Text = "Settings", 
                    Dock = DockStyle.Left,
                    FlatStyle = FlatStyle.Flat                    
                };
                pluginSettings.FlatAppearance.BorderSize = 0;
                
                Button pluginAbout = new()
                {
                    Text = "About",
                    Dock = DockStyle.Left,
                    FlatStyle = FlatStyle.Flat
                };
                pluginAbout.FlatAppearance.BorderSize = 0;

                bool enable = true;
                if (enabledPlugins.TryGetValue(plugin.Name, out bool value))
                {
                    enable = value;
                }

                if (pluginStatusValue == PluginManager.PluginStatus.Outdated ||
                    pluginStatusValue == PluginManager.PluginStatus.Errored)
                {
                    enable = false;
                    PluginManager.GetInstance.SetPluginEnabled(plugin, false);
                }

                CheckBox pluginEnabled = new()
                {
                    CheckAlign = ContentAlignment.MiddleCenter,
                    Checked = enable,
                    Enabled = pluginStatusValue != PluginManager.PluginStatus.Errored
                };

                pluginEnabled.CheckedChanged += (_, _) =>
                {
                    PluginManager.GetInstance.SetPluginEnabled(plugin, pluginEnabled.Checked);
                };

                Button pluginMenu = new()
                {
                    Text = "▾",
                    Dock = DockStyle.Left,
                    FlatStyle = FlatStyle.Flat,
                    Size = new(20,20),
                    Enabled = pluginStatusValue != PluginManager.PluginStatus.Errored
                };
                pluginMenu.FlatAppearance.BorderSize = 0;

                PluginContextMenu pluginContextMenu = new(plugin);

                pluginMenu.Click += (o, e) =>
                {
                    pluginContextMenu.Show(this, pluginMenu.Location);
                };

                AddWithLocation(pluginName, row, 0);
                AddWithLocation(pluginAuthor, row, 1);
                AddWithLocation(pluginType, row, 2);
                AddWithLocation(pluginVersion, row, 3);
                AddWithLocation(pluginStatus, row, 4);
                AddWithLocation(pluginEnabled, row, 5);
                AddWithLocation(pluginMenu, row, 6);

                row++;
            }

            Resize += PluginList_Resize;
        }

        private void PluginList_Resize(object? sender, EventArgs e)
        {
            if (Parent?.Width < 710)
            {
                ColumnStyles[2].SizeType = SizeType.Absolute;
                ColumnStyles[2].Width = 0;
            }
            else
            {
                ColumnStyles[2].SizeType = SizeType.AutoSize;
            }

            if (Parent?.Width < 610)
            {
                ColumnStyles[3].SizeType = SizeType.Absolute;
                ColumnStyles[3].Width = 0;
            }
            else
            {
                ColumnStyles[3].SizeType = SizeType.AutoSize;
            }
        }

        private void AddWithLocation(Control control, int row, int column)
        {
            Controls.Add(control);
            SetRow(control, row);
            SetColumn(control, column);
        }

        private Dictionary<string, bool> GetEnabledPlugins()
        {
            Dictionary<string, bool> enabledPlugins = [];
            string enabledPluginsSettings = Properties.Core.Default.PluginsEnabled;
            if (!string.IsNullOrWhiteSpace(enabledPluginsSettings))
            {
                try
                {
                    enabledPlugins = JsonSerializer.Deserialize<Dictionary<string, bool>>(enabledPluginsSettings) ?? [];
                }
                catch 
                {
                    // Failed deserialization means bad value, blow it away.
                    Properties.Core.Default.PluginsEnabled = string.Empty;
                    SettingsManager.Save();
                }
            }
            return enabledPlugins;
        }

        private static string PluginStatusString(PluginManager.PluginStatus status)
        {
            switch (status)
            {
                case PluginManager.PluginStatus.OK:
                    return "OK";

                case PluginManager.PluginStatus.InvalidPlugin:
                    return "Invalid Plugin";

                case PluginManager.PluginStatus.InvalidLibrary:
                    return "Invalid File";

                case PluginManager.PluginStatus.Outdated:
                    return "Update Required";

                case PluginManager.PluginStatus.Errored:
                    return "Error";

                default:
                    return string.Empty;
            }
        }
    }
}
