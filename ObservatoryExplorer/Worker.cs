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

        private bool readAllStarting = false;
        private bool readAllInProgress = false;

        public string Name => "Observatory Explorer";

        public string ShortName => "Explorer";

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
            bool recordProcessed = false;
            switch (journal)
            {
                case Scan scan:
                    explorer.ProcessScan(scan, readAllInProgress);
                    recordProcessed = true;
                    break;
                case FSSBodySignals signals:
                    explorer.RecordSignal(signals);
                    break;
                case ScanBaryCentre barycentre:
                    explorer.RecordBarycentre(barycentre);
                    break;
                case DiscoveryScan discoveryScan:
                    break;
                case FSSDiscoveryScan discoveryScan:
                    break;
                case FSSSignalDiscovered signalDiscovered:
                    break;
                case NavBeaconScan beaconScan:
                    break;
                case SAAScanComplete scanComplete:
                    break;
                case SAASignalsFound signalsFound:
                    break;
            }
            
            //Set this *after* the first scan processes so that we get the current custom criteria file.
            if (readAllStarting && recordProcessed)
                readAllInProgress = true;
        }

        public void ReadAllStarted()
        {
            readAllStarting = true;
            Core.ClearGrid(this, new ExplorerUIResults());
            explorer.Clear();
        }

        public void ReadAllFinished()
        {
            readAllStarting = false;
            readAllInProgress = false;
        }

        public object Settings
        {
            get => settings;
            set => settings = (ExplorerSettings)value;
        }

        private ExplorerSettings settings;
    }
}
