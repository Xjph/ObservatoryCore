using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.ObjectModel;
using Observatory.Framework.Sorters;
using System.Collections.Generic;

namespace Observatory.Botanist
{
    public class Botanist : IObservatoryWorker
    {
        private IObservatoryCore Core;
        private bool OdysseyLoaded = false;
        private Dictionary<BodyAddress, BioPlanetDetail> BioPlanets;

        // To make this journal locale agnostic, use the genus identifier and map to English names used in notifications.
        // Note: Values here are also used in the lookup for colony distance, so we also use this to resolve misspellings and Frontier bugs.
        private readonly Dictionary<String, String> EnglishGenusByIdentifier = new() {
            { "$Codex_Ent_Aleoids_Genus_Name;", "Aleoida" },
            { "$Codex_Ent_Bacterial_Genus_Name;", "Bacterium" },
            { "$Codex_Ent_Cactoid_Genus_Name;", "Cactoida" },
            { "$Codex_Ent_Clepeus_Genus_Name;;", "Clypeus" }, // Fun misspelling of the identifier discovered in the journals
            { "$Codex_Ent_Clypeus_Genus_Name;", "Clypeus" },
            { "$Codex_Ent_Conchas_Genus_Name;", "Concha" },
            { "$Codex_Ent_Electricae_Genus_Name;", "Electricae" },
            { "$Codex_Ent_Fonticulus_Genus_Name;", "Fonticulua" },
            { "$Codex_Ent_Shrubs_Genus_Name;", "Frutexa" },
            { "$Codex_Ent_Fumerolas_Genus_Name;", "Fumerola" },
            { "$Codex_Ent_Fungoids_Genus_Name;", "Fungoida" },
            { "$Codex_Ent_Osseus_Genus_Name;", "Osseus" },
            { "$Codex_Ent_Recepta_Genus_Name;", "Recepta" },
            { "$Codex_Ent_Stratum_Genus_Name;", "Stratum" },
            { "$Codex_Ent_Tubus_Genus_Name;", "Tubus" },
            { "$Codex_Ent_Tussocks_Genus_Name;", "Tussock" },
            { "$Codex_Ent_Ground_Struct_Ice_Name;", "Crystalline Shards" },
            { "$Codex_Ent_Brancae_Name;", "Brain Trees" },
            { "$Codex_Ent_Seed_Name;", "Brain Tree" }, // Misspelling? :shrug: 'Seed' also seems to refer to peduncle things.
            { "$Codex_Ent_Sphere_Name;", "Anemone" },
            { "$Codex_Ent_Tube_Name;", "Sinuous Tubers" },
            { "$Codex_Ent_Vents_Name;", "Amphora Plant" },
            { "$Codex_Ent_Cone_Name;", "Bark Mounds" },
        };

        // Note: Some Horizons bios may be missing, but they'll get localized genus name and default colony distance
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
            { "Crystalline Shards", DEFAULT_COLONY_DISTANCE },
            { "Brain Tree", DEFAULT_COLONY_DISTANCE },
            { "Anemone", DEFAULT_COLONY_DISTANCE },
            { "Sinuous Tubers", DEFAULT_COLONY_DISTANCE },
            { "Amphora Plant", DEFAULT_COLONY_DISTANCE },
            { "Bark Mounds", DEFAULT_COLONY_DISTANCE },
        };
        private const int DEFAULT_COLONY_DISTANCE = 100;

        ObservableCollection<object> GridCollection;
        private PluginUI pluginUI;
        private NotificationArgs currentNotificationArgs = null;
        private (double lat, double lon) firstScanLocation = INVALID_LOCATION;
        private (double lat, double lon) secondScanLocation = INVALID_LOCATION;
        private static (double lat, double lon) INVALID_LOCATION = (200, 200);
        private double currentBodyRadius = 0;
        private BotanistSettings botanistSettings = new()
        {
            OverlayEnabled = true,
            OverlayIsSticky = true,
        };
        private AboutInfo _aboutInfo = new()
        {
            FullName = "Observatory Botanist",
            ShortName = "Botanist",
            Description = "Botanist is a core plugin for Observatory, designed to provide simple output for biological signals.",
            AuthorName = "Vithigar",
            Links = new()
            {
                new AboutLink("github", "https://github.com/Xjph/ObservatoryCore"),
                new AboutLink("Documentation", "https://observatory.xjph.net/usage/plugins/botanist"),
            }
        };

        public AboutInfo AboutInfo => _aboutInfo;

        public string Version => typeof(Botanist).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings { get => botanistSettings; set { botanistSettings = (BotanistSettings)value;  } }

        // public IObservatoryComparer ColumnSorter => new NoOpColumnSorter();

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
                        UpdateUIGrid();
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
                                    Genus = EnglishGenusByIdentifier.GetValueOrDefault(scanOrganic.Genus, scanOrganic.Genus_Localised),
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

                            // If this is null don't bother.
                            if (scanOrganic.Species_Localised != null)
                            {
                                switch (scanOrganic.ScanType)
                                {
                                    case ScanOrganicType.Log:
                                    case ScanOrganicType.Sample:
                                        if (!Core.IsLogMonitorBatchReading && botanistSettings.OverlayEnabled)
                                        {
                                            var colonyDistance = GetColonyDistance(scanOrganic);
                                            var sampleNum = scanOrganic.ScanType == ScanOrganicType.Log ? 1 : 2;
                                            var status = Core.GetStatus() ?? new()
                                            {
                                                Latitude = 200,
                                                Longitude = 200,
                                                PlanetRadius = 0
                                            }; // INVALID_LOCATION

                                            if (sampleNum == 1)
                                            {
                                                firstScanLocation = (status.Latitude, status.Longitude);
                                                secondScanLocation = INVALID_LOCATION;
                                            }
                                            else if (sampleNum == 2)
                                                secondScanLocation = (status.Latitude, status.Longitude);

                                            currentBodyRadius = status.PlanetRadius;
                                            if (scanOrganic.ScanType == ScanOrganicType.Log)
                                                MaybeCloseSamplerStatusNotification(); // Force a new notification.

                                            var notificationGuid = currentNotificationArgs?.Guid ?? Guid.NewGuid();
                                            currentNotificationArgs = new()
                                            {
                                                Title = scanOrganic.Species_Localised,
                                                Detail = $"Sample {sampleNum} of 3{Environment.NewLine}Colony distance: {colonyDistance}m{Environment.NewLine}{GetDistanceText(status)}",
                                                Rendering = NotificationRendering.NativeVisual,
                                                Timeout = botanistSettings.OverlayIsSticky ? 0 : -1,
                                                Sender = AboutInfo.ShortName,
                                                Guid = notificationGuid,
                                            };
                                            if (!botanistSettings.OverlayIsSticky || scanOrganic.ScanType == ScanOrganicType.Log)
                                            {
                                                Core.SendNotification(currentNotificationArgs);
                                            }
                                            else
                                            {
                                                Core.UpdateNotification(currentNotificationArgs);
                                            }
                                        }

                                        if (!bioPlanet.SpeciesFound.ContainsKey(scanOrganic.Species_Localised))
                                        {
                                            bioPlanet.SpeciesFound.Add(scanOrganic.Species_Localised, new()
                                            {
                                                Genus = EnglishGenusByIdentifier.GetValueOrDefault(scanOrganic.Genus, scanOrganic.Genus_Localised),
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
                                        firstScanLocation = INVALID_LOCATION; secondScanLocation = INVALID_LOCATION;
                                        break;
                                }
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

        public void StatusChange(Status status)
        {
            if (currentNotificationArgs != null)
            {
                var currentNotificationDetail = currentNotificationArgs.Detail.Split(Environment.NewLine);

                currentNotificationDetail[2] = GetDistanceText(status);
                
                currentNotificationArgs.Detail = string.Join(Environment.NewLine, currentNotificationDetail);
                Core.UpdateNotification(currentNotificationArgs);
            }
        }

        private string GetDistanceText(Status status)
        {
            double? firstDistance = null;
            double? secondDistance = null;

            if (firstScanLocation != INVALID_LOCATION)
                firstDistance = CalculateGreatCircleDistance(firstScanLocation, (status.Latitude, status.Longitude), currentBodyRadius);

            if (secondScanLocation != INVALID_LOCATION)
                secondDistance = CalculateGreatCircleDistance(secondScanLocation, (status.Latitude, status.Longitude), currentBodyRadius);
            var bothDistances = firstDistance != null && secondDistance != null;
            return $"Current distance{(bothDistances ? 's' : string.Empty)}: "
                    + (firstDistance != null ? $"{firstDistance:N0}m" : string.Empty)
                    + (bothDistances ? " - " : string.Empty)
                    + (secondDistance != null ? $"{secondDistance:N0}m" : string.Empty);
        }

        private static double CalculateGreatCircleDistance((double lat, double lon) location1, (double lat, double lon) location2, double radius)
        {
            var latDeltaSin = Math.Sin(ToRadians(location1.lat - location2.lat) / 2);
            var longDeltaSin = Math.Sin(ToRadians(location1.lon - location2.lon) / 2);
            
            var hSin = latDeltaSin * latDeltaSin + Math.Cos(ToRadians(location1.lat)) * Math.Cos(ToRadians(location1.lat)) * longDeltaSin * longDeltaSin;
            return 2 * radius * Math.Asin(Math.Sqrt(hSin));
        }

        private static double ToRadians(double degrees) => degrees * 0.0174533;

        private object GetColonyDistance(ScanOrganic scan)
        {
            // Map the Genus to a Genus name then lookup colony distance.
            return ColonyDistancesByGenus.GetValueOrDefault(EnglishGenusByIdentifier.GetValueOrDefault(scan.Genus, String.Empty), DEFAULT_COLONY_DISTANCE);
        }

        private void MaybeCloseSamplerStatusNotification()
        {
            if (currentNotificationArgs != null)
            {
                Core.CancelNotification(currentNotificationArgs.Guid.Value);
                currentNotificationArgs = null;
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
                List<BotanistGrid> planetItems = [];
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
                    planetItems.Add(planetRow);
                    // Core.AddGridItem(this, planetRow);
                }

                bool firstRow = true;
                foreach (var entry in bioPlanet.SpeciesFound)
                {
                    int colonyDistance = ColonyDistancesByGenus.GetValueOrDefault(entry.Value.Genus ?? "", DEFAULT_COLONY_DISTANCE);
                    var speciesRow = new BotanistGrid()
                    {
                        Body = firstRow ? bioPlanet.BodyName : string.Empty,
                        BioTotal = firstRow ? bioPlanet.BioTotal.ToString() : string.Empty,
                        Species = entry.Key,
                        Analysed = entry.Value.Analysed ? "✓" : "",
                        ColonyDistance = $"{colonyDistance}m",
                    };
                    // Core.AddGridItem(this, speciesRow);
                    planetItems.Add(speciesRow);
                    firstRow = false;
                }
                Core.AddGridItems(this, planetItems);
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
        [ColumnSuggestedWidth(300)]
        public string Body { get; set; }
        [ColumnSuggestedWidth(100)]
        public string BioTotal { get; set; }
        [ColumnSuggestedWidth(300)]
        public string Species { get; set; }
        [ColumnSuggestedWidth(100)]
        public string Analysed { get; set; }
        [ColumnSuggestedWidth(100)]
        public string ColonyDistance { get; set; }
    }
}
