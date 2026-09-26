using System.Collections.ObjectModel;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;

namespace Observatory.Sandbox
{
    /// <summary>
    /// This "plugin" is a simple dev sandbox just to provide
    /// a convenient place to test arbitrary events from the plugin side.
    /// </summary>
    public class Sandbox : IObservatoryWorker
    {
        private IObservatoryCore Core;

        ObservableCollection<object> GridCollection;
        private PluginUI pluginUI;
        private NotificationArgs currentNotificationArgs = null;

        public static Guid Guid => new("EEE256DC-99CB-475F-8363-3B45A3121554");

        public string Version => typeof(Sandbox).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        private SandboxSettings _settings = new SandboxSettings();

        public object Settings
        {
            get => _settings;
            set { _settings = (SandboxSettings)value; }
        }

        public AboutInfo AboutInfo =>
            new()
            {
                ShortName = "Sandbox",
                Description = "A debug plugin for testing and development.",
                AuthorName = "Vithigar",
            };

        public void JournalEvent<TJournal>(TJournal journal)
            where TJournal : JournalBase
        {
            switch (journal)
            {
                case Cargo journalCargo:
                    Core.AddGridItem(
                        this,
                        new SandboxGrid
                        {
                            Data =
                                $"Cargo Event: {journalCargo.Vessel} - {journalCargo.Count} items",
                        }
                    );
                    var manifest = Core.GetCargo();
                    manifest.Inventory?.ForEach(item =>
                    {
                        Core.AddGridItem(
                            this,
                            new SandboxGrid
                            {
                                Data = $"Cargo Item: {item.Name} - {item.Count} units",
                            }
                        );
                    });
                    break;
            }
        }

        public void StatusChange(Status status) { }

        public void CargoChange(CargoFile cargo)
        {
            Core.AddGridItem(
                this,
                new SandboxGrid { Data = $"Cargo Change: {cargo.Vessel} - {cargo.Count} items" }
            );
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            GridCollection = new();
            SandboxGrid uiObject = new();

            GridCollection.Add(uiObject);
            pluginUI = new PluginUI(GridCollection);

            Core = observatoryCore;
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args) { }
    }

    public class SandboxGrid
    {
        [ColumnSuggestedWidth(300)]
        public string Data { get; set; }
    }
}
