using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using System.Diagnostics;

namespace Observatory.UI
{
    internal class PluginContextMenu : ContextMenuStrip
    {
        internal PluginContextMenu(IObservatoryPlugin plugin, TabPage? pluginTab = null)
        {
            ToolStripMenuItem popout = new()
            {
                Text = $"Popout {plugin.ShortName}"
            };
            ToolStripMenuItem settings = new()
            {
                Text = $"{plugin.ShortName} Settings"
            };
            ToolStripMenuItem about = new()
            {
                Text = $"About {plugin.ShortName}"
            };
            ToolStripMenuItem folder = new()
            {
                Text = $"Open {plugin.ShortName} Data Folder"
            };

            if (pluginTab != null)
            {
                Items.Add(popout);
            }
            Items.Add(settings);
            Items.Add(about);
            Items.Add(folder);

            ItemClicked += (o, e) =>
            {
                if (e.ClickedItem == popout)
                {
                    var form = GetFormByTitle(plugin.Name);
                    if (form != null)
                    {
                        form.Activate();
                    }
                    else
                    {
                        var popoutForm = new PopoutForm(pluginTab, plugin.Name);
                        ThemeManager.GetInstance.RegisterControl(popoutForm);
                        popoutForm.Show();
                    }
                }
                if (e.ClickedItem == settings)
                {
                    var settingsTitle = $"{plugin.Name} Settings";
                    var form = GetFormByTitle(settingsTitle);
                    if (form != null)
                    {
                        form.Activate();
                    }
                    else
                    {
                        var settingsForm = new SettingsForm(plugin);
                        settingsForm.Show();
                    }
                }
                if (e.ClickedItem == about)
                {
                    if (plugin.AboutInfo != null)
                    {
                        var form = GetFormByTitle($"About {plugin.AboutInfo.FullName}");
                        if (form != null)
                        {
                            form.Activate();
                        }
                        else
                        {
                            var aboutForm = new AboutForm(plugin.AboutInfo);
                            ThemeManager.GetInstance.RegisterControl(aboutForm);
                            aboutForm.FormClosing += (_, _) => ThemeManager.GetInstance.UnregisterControl(aboutForm);
                            aboutForm.Show();
                        }
                    }
                }
                if (e.ClickedItem == folder)
                {
                    var assemblyName = plugin.GetType().Assembly.GetName().Name ?? "";
                    var storageDir = PluginManager.GetInstance.Core.GetStorageFolderForPlugin(assemblyName);
                    if (!string.IsNullOrWhiteSpace(storageDir) && Directory.Exists(storageDir))
                    {
                        var fileExplorerInfo = new ProcessStartInfo() { FileName = storageDir, UseShellExecute = true };
                        Process.Start(fileExplorerInfo);
                    }
                }
            };
        }

        private Form? GetFormByTitle(string title)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Text == title)
                    return form;
            }
            return null;
        }
    }
}
