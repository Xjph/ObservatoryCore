using Observatory.Framework;
using Observatory.Framework.Interfaces;

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

        public object? Settings { get => null; set { } }

        public void Load(IObservatoryCore observatoryCore)
        { }

        public void OnNotificationEvent(NotificationArgs notificationArgs)
        { }
    }
}
