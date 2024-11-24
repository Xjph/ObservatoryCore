using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.UI
{
    internal class FormsManager
    {
        public static CoreForm? FindCoreForm()
        {
            foreach (var f in Application.OpenForms)
            {
                if (f is CoreForm form)
                {
                    return form;
                }
            }
            return null;
        }

        public static Form? GetFormByTitle(string title)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Text == title)
                    return form;
            }
            return null;
        }

        public static void OpenPluginAboutForm(IObservatoryPlugin plugin)
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

        public static void OpenPluginSettingsForm(IObservatoryPlugin plugin)
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

        public static void OpenPluginPopoutForm(IObservatoryPlugin plugin, TabPage? pluginTab = null)
        {
            var form = GetFormByTitle(plugin.Name);
            if (form != null)
            {
                form.Activate();
            }
            else
            {
                var popoutForm = new PopoutForm(pluginTab, plugin);
                ThemeManager.GetInstance.RegisterControl(popoutForm);
                popoutForm.Show();
            }
        }

        public static void FocusPluginTabOrWindow(string pluginName)
        {
            // Check first if the plugin is popped out and activate/raise that window.
            Form? pluginPopout = FormsManager.GetFormByTitle(pluginName);
            if (pluginPopout != null)
            {
                pluginPopout.Activate();
            }
            else
            {
                // Otherwise, switch the main window to that tab.
                FindCoreForm()?.FocusPlugin(pluginName);
            }
        }
    }
}
