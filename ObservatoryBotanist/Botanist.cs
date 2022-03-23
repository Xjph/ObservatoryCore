using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;

namespace Observatory.Botanist
{
    public class Botanist : IObservatoryWorker
    {
        private IObservatoryCore Core;
        private bool OdysseyLoaded = false;
        private Dictionary<BodyAddress, BioPlanetDetail> BioPlanets;

        // Note: This only explicitly contains Odyssey bios. Everything else is assumed to be 100.
        // (This is partly because names for the genus for legacy/Horizons bios are somewhat inconsistent.)
        private readonly Dictionary<String, int> ColonyDistancesByGenus = new() {
            { "Aleoida", 150 },
            { "Bacterium", 500 },
            { "Cactoida", 300 },
            { "Clypeus", 150 },
            { "Concha", 150 },
            { "Electricae", 1000 },
            { "Fonticulua", 500 },
            { "Frutexa", 150 },
            { "Fumerola", 100 },
            { "Fungoida", 300 },
            { "Osseus", 800 },
            { "Recepta", 150 },
            { "Stratum", 500 },
            { "Tubus", 800 },
            { "Tussock", 200 },
        };
        private const int DEFAULT_COLONY_DISTANCE = 100;

        ObservableCollection<object> GridCollection;
        private PluginUI pluginUI;
        private Guid? samplerStatusNotification = null;
        private BotanistSettings botanistSettings = new()
        {
            OverlayEnabled = true,
        };
        public string Name => "Observatory Botanist";

        public string ShortName => "Botanist";

        public string Version => typeof(Botanist).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings { get => botanistSettings; set { botanistSettings = (BotanistSettings)value;  } }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case LoadGame loadGame:
                    OdysseyLoaded = loadGame.Odyssey;
                    break;
                case SAASignalsFound signalsFound:
                    {
                        BodyAddress systemBodyId = new()
                        {
                            SystemAddress = signalsFound.SystemAddress,
                            BodyID = signalsFound.BodyID
                        };
                        if (OdysseyLoaded && !BioPlanets.ContainsKey(systemBodyId))
                        {
                            var bioSignals = from signal in signalsFound.Signals
                                             where signal.Type == "$SAA_SignalType_Biological;"
                                             select signal;

                            if (bioSignals.Any())
                            {
                                BioPlanets.Add(
                                    systemBodyId,
                                    new() {
                                        BodyName = signalsFound.BodyName,
                                        BioTotal = bioSignals.First().Count,
                                        SpeciesFound = new()
                                    }
                                );
                            }
                        }
                    }
                    break;
                case ScanOrganic scanOrganic:
                    {
                        BodyAddress systemBodyId = new()
                        {
                            SystemAddress = scanOrganic.SystemAddress,
                            BodyID = scanOrganic.Body
                        };
                        if (!BioPlanets.ContainsKey(systemBodyId))
                        {
                            // Unlikely to ever end up in here, but just in case create a new planet entry.
                            Dictionary<string, BioSampleDetail> bioSampleDetails = new();
                            bioSampleDetails.Add(scanOrganic.Species_Localised, new()
                                {
                                    Genus = scanOrganic.Genus_Localised,
                                    Analysed = false
                                });

                            BioPlanets.Add(systemBodyId, new() {
                                BodyName = string.Empty,
                                BioTotal = 0,
                                SpeciesFound = bioSampleDetails
                            });
                        }
                        else
                        {
                            var bioPlanet = BioPlanets[systemBodyId];
                            
                            switch (scanOrganic.ScanType)
                            {
                                case ScanOrganicType.Log:
                                case ScanOrganicType.Sample:
                                    if (!Core.IsLogMonitorBatchReading && botanistSettings.OverlayEnabled)
                                    {
                                        var colonyDistance = GetColonyDistance(scanOrganic);
                                        var sampleNum = scanOrganic.ScanType == ScanOrganicType.Log ? 1 : 2;
                                        NotificationArgs args = new()
                                        {
                                            Title = scanOrganic.Species_Localised,
                                            Detail = $"Sample {sampleNum} of 3{Environment.NewLine}Colony distance: {colonyDistance} m",
                                            Rendering = NotificationRendering.NativeVisual,
                                            Timeout = 0,
                                        };
                                        if (samplerStatusNotification == null)
                                        {
                                            samplerStatusNotification = Core.SendNotification(args);
                                        }
                                        else
                                        {
                                            Core.UpdateNotification(samplerStatusNotification.Value, args);
                                        }
                                    }

                                    if (!bioPlanet.SpeciesFound.ContainsKey(scanOrganic.Species_Localised))
                                    {
                                        bioPlanet.SpeciesFound.Add(scanOrganic.Species_Localised, new()
                                        {
                                            Genus = scanOrganic.Genus_Localised,
                                            Analysed = false
                                        });
                                    }
                                    break;
                                case ScanOrganicType.Analyse:
                                    if (!bioPlanet.SpeciesFound[scanOrganic.Species_Localised].Analysed)
                                    {
                                        bioPlanet.SpeciesFound[scanOrganic.Species_Localised].Analysed = true;
                                    }
                                    MaybeCloseSamplerStatusNotification();
                                    break;
                            }
                        }
                        UpdateUIGrid();
                    }
                    break;
                case LeaveBody:
                case FSDJump:
                case Shutdown:
                    // These are all good reasons to kill any open notification. Note that SupercruiseEntry is NOT a
                    // suitable reason to close the notification as the player hopping out only to double check the
                    // DSS map for another location. Note that a game client crash will not close the status notification.
                    MaybeCloseSamplerStatusNotification();
                    break;
            }
        }

        private object GetColonyDistance(ScanOrganic scan)
        {
            // Try find a genus name.
            string genusName = null;
            if (scan.Genus_Localised != null)
            {
                genusName = scan.Genus_Localised;
            }
            else if (scan.Genus == "$Codex_Ent_Clepeus_Genus_Name;")
            {
                // Odyssey had a bug until Update 9 where Clypeus ScanOrganic entries were missing the Genus_Localised property.
                genusName = "Clypeus";
            }

            return (genusName != null && ColonyDistancesByGenus.ContainsKey(genusName)) ? ColonyDistancesByGenus[genusName] : DEFAULT_COLONY_DISTANCE;
        }

        private void MaybeCloseSamplerStatusNotification()
        {
            if (samplerStatusNotification != null)
            {
                Core.CancelNotification(samplerStatusNotification.Value);
                samplerStatusNotification = null;
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            GridCollection = new();
            BotanistGrid uiObject = new();

            GridCollection.Add(uiObject);
            pluginUI = new PluginUI(GridCollection);

            BioPlanets = new();
            
            Core = observatoryCore;
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            if (LogMonitorStateChangedEventArgs.IsBatchRead(args.NewState))
            {
                // Beginning a batch read. Clear grid.
                Core.ClearGrid(this, new BotanistGrid());
            }
            else if (LogMonitorStateChangedEventArgs.IsBatchRead(args.PreviousState))
            {
                // Batch read is complete. Show data.
                UpdateUIGrid();
            }
        }

        private void UpdateUIGrid()
        {
            // Suppress repainting the entire contents of the grid on every ScanOrganic record we read.
            if (Core.IsLogMonitorBatchReading) return;

            BotanistGrid uiObject = new();
            Core.ClearGrid(this, uiObject);
            foreach (var bioPlanet in BioPlanets.Values)
            {
                if (bioPlanet.SpeciesFound.Count == 0)
                {
                    var planetRow = new BotanistGrid()
                    {
                        Body = bioPlanet.BodyName,
                        BioTotal = bioPlanet.BioTotal.ToString(),
                        Species = "(NO SAMPLES TAKEN)",
                        Analysed = string.Empty,
                        ColonyDistance = string.Empty,
                    };
                    Core.AddGridItem(this, planetRow);
                }

                bool firstRow = true;
                foreach (var entry in bioPlanet.SpeciesFound)
                {
                    int colonyDistance = DEFAULT_COLONY_DISTANCE;
                    if (entry.Value.Genus != null && ColonyDistancesByGenus.ContainsKey(entry.Value.Genus))
                    {
                        colonyDistance = ColonyDistancesByGenus[entry.Value.Genus];
                    }
                    var speciesRow = new BotanistGrid()
                    {
                        Body = firstRow ? bioPlanet.BodyName : string.Empty,
                        BioTotal = firstRow ? bioPlanet.BioTotal.ToString() : string.Empty,
                        Species = entry.Key,
                        Analysed = entry.Value.Analysed ? "✓" : "",
                        ColonyDistance = $"{colonyDistance}m",
                    };
                    Core.AddGridItem(this, speciesRow);
                    firstRow = false;
                }
            }
        }
    }

    class BodyAddress
    {
        public ulong SystemAddress { get; set; }
        public int BodyID { get; set;  }
        
        public override bool Equals(object obj)
        {
            // We want value equality here.

            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            BodyAddress other = (BodyAddress)obj;
            return other.SystemAddress == SystemAddress
                && other.BodyID == BodyID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SystemAddress, BodyID);
        }
    }

    class BioPlanetDetail
    {
        public string BodyName { get; set; }
        public int BioTotal { get; set; }
        public Dictionary<string, BioSampleDetail> SpeciesFound { get; set; }
    }

    class BioSampleDetail
    {
        public string Genus { get; set; }
        public bool Analysed { get; set; }
    }

    public class BotanistGrid
    {
        public string Body { get; set; }
        public string BioTotal { get; set; }
        public string Species { get; set; }
        public string Analysed { get; set; }
        public string ColonyDistance { get; set; }
    }
}
