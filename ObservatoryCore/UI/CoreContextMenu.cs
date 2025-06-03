using NAudio.Gui;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.PluginManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.UI
{
    internal class CoreContextMenu : ContextMenuStrip
    {
        internal CoreContextMenu(AboutInfo coreAboutInfo, Action coreSettingsDelegate, Action pluginsFolderDelegate)
        {
            ToolStripMenuItem settings = new()
            {
                Text = "Core Settings"
            };
            ToolStripMenuItem about = new()
            {
                Text = $"About Core"
            };
            ToolStripMenuItem pluginsFolder = new()
            {
                Text = $"Open Plugins Folder"
            };

            Items.Add(settings);
            Items.Add(about);
            Items.Add(pluginsFolder);

            ItemClicked += (o, e) =>
            {
                if (e.ClickedItem == settings)
                {
                    coreSettingsDelegate();
                }
                if (e.ClickedItem == about)
                {
                    FormsManager.OpenAboutForm(coreAboutInfo);
                }
                if (e.ClickedItem == pluginsFolder)
                {
                    pluginsFolderDelegate();
                }
            };
        }

    }
}
