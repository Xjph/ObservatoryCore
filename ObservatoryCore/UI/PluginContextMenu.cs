using Observatory.Framework;
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
                    FormsManager.OpenPluginPopoutForm(plugin, pluginTab);
                }
                if (e.ClickedItem == settings)
                {
                    FormsManager.OpenPluginSettingsForm(plugin);
                }
                if (e.ClickedItem == about)
                {
                    FormsManager.OpenAboutForm(plugin.AboutInfo);
                }
                if (e.ClickedItem == folder)
                {
                    var storageDir = PluginManager.GetInstance.Core.GetStorageFolderForPlugin(plugin);
                    if (!string.IsNullOrWhiteSpace(storageDir) && Directory.Exists(storageDir))
                    {
                        var fileExplorerInfo = new ProcessStartInfo() { FileName = storageDir, UseShellExecute = true };
                        Process.Start(fileExplorerInfo);
                    }
                }
            };
        }
    }
}
