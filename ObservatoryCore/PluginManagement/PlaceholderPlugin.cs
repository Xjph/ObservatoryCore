using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.PluginManagement
{
    public class PlaceholderPlugin : IObservatoryNotifier
    {
        public PlaceholderPlugin(string name)
        {
            this.name = name;
        }

        public string Name => name;

        private string name;

        public string ShortName => name;

        public string Version => string.Empty;

        public PluginUI PluginUI => new PluginUI(PluginUI.UIType.None, null);

        public object Settings { get => null; set { } }

        public void Load(IObservatoryCore observatoryCore)
        { }

        public void OnNotificationEvent(NotificationArgs notificationArgs)
        { }
    }
}
