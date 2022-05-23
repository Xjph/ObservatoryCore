﻿using System.Collections.ObjectModel;
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
            switch (journal)
            {
                case Scan scan:
                    explorer.ProcessScan(scan, Core.IsLogMonitorBatchReading && recordProcessedSinceBatchStart);
                    // Set this *after* the first scan processes so that we get the current custom criteria file.
                    if (Core.IsLogMonitorBatchReading) recordProcessedSinceBatchStart = true;
                    break;
                case FSSBodySignals signals:
                    explorer.RecordSignal(signals);
                    break;
                case ScanBaryCentre barycentre:
                    explorer.RecordBarycentre(barycentre);
                    break;
                case FSDJump fsdjump:
                    if (fsdjump is CarrierJump && !((CarrierJump)fsdjump).Docked)
                        break;
                    explorer.SetSystem(fsdjump.StarSystem);
                    break;
                case Location location:
                    explorer.SetSystem(location.StarSystem);
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
