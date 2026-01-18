using System.Collections.ObjectModel;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using Observatory.Framework;

namespace Observatory.Explorer
{
    public class ExplorerWorker : IObservatoryWorker
    {
        public ExplorerWorker()
        {
            settings = new()
            {
                CloseBinary = true,
                CloseOrbit = true,
                CloseRing = true,
                CollidingBinary = true,
                FastRotation = true,
                HighEccentricity = true,
                LandableAtmosphere = true,
                LandableHighG = true,
                LandableLarge = true,
                LandableRing = true,
                LandableTerraformable = true,
                Nested = true,
                ShepherdMoon = true,
                SmallObject = true,
                WideRing = true
            };
            resultsGrid = new();
        }

        private Explorer explorer;
        private ObservableCollection<object> resultsGrid;
        private IObservatoryCore Core;

        private bool recordProcessedSinceBatchStart = false;
        private AboutInfo _aboutInfo = new()
        {
            FullName = "Observatory Explorer",
            ShortName = "Explorer",
            Description = "Explorer is a core plugin for Observatory, designed to point out any astronomical points of interest that might be encountered.",
            AuthorName = "Vithigar",
            Links = new()
            {
                new AboutLink("github", "https://github.com/Xjph/ObservatoryCore"),
                new AboutLink("Documentation", "https://observatory.xjph.net/usage/plugins/explorer"),
            }
        };

        public AboutInfo AboutInfo => _aboutInfo;
 
        public static Guid Guid => new ("B11E8725-3F71-4445-9B09-682F9EAB8B59");

        public string Version => typeof(ExplorerWorker).Assembly.GetName().Version.ToString();

        private PluginUI pluginUI;

        public PluginUI PluginUI => pluginUI;

        public void Load(IObservatoryCore observatoryCore)
        {
            explorer = new Explorer(this, observatoryCore, resultsGrid);
            resultsGrid.Add(new ExplorerUIResults());
            pluginUI = new PluginUI(resultsGrid);
            Core = observatoryCore;
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case Scan scan:
                    explorer.ProcessScan(scan, Core.IsLogMonitorBatchReading && recordProcessedSinceBatchStart);
                    // Set this *after* the first scan processes so that we get the current custom criteria file.
                    if (Core.IsLogMonitorBatchReading) recordProcessedSinceBatchStart = true;
                    break;
                case FSSBodySignals signals:
                    explorer.RecordSignal(signals);
                    explorer.ProcessSignalScan(signals);
                    break;
                case ScanBaryCentre barycentre:
                    explorer.RecordBarycentre(barycentre);
                    break;
                case FSDJump fsdjump:
                    if (fsdjump is CarrierJump && !((CarrierJump)fsdjump).Docked)
                        break;
                    explorer.SetSystem(fsdjump.StarSystem);
                    explorer.ProcessJump(fsdjump);
                    break;
                case Location location:
                    explorer.SetSystem(location.StarSystem);
                    break;
                case FSSDiscoveryScan discoveryScan:
                    explorer.ProcessDiscovery(discoveryScan);
                    break;
                case SAASignalsFound signalsFound:
                    explorer.ProcessSignalScan(signalsFound);
                    break;
                case FSSAllBodiesFound allBodiesFound:
                    explorer.ProcessAllBodies(allBodiesFound);
                    break;
                case CodexEntry codexEntry:
                    explorer.ProcessCodexEntry(codexEntry);
                    break;
            }
            
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            if (LogMonitorStateChangedEventArgs.IsBatchRead(args.NewState))
            {
                // Beginning a batch read. Clear grid.
                recordProcessedSinceBatchStart = false;
                Core.ClearGrid(this, new ExplorerUIResults());
                explorer.Clear();
            }
        }
        
        public object Settings
        {
            get => settings;
            set => settings = (ExplorerSettings)value;
        }

        internal ExplorerSettings settings;
    }
}
