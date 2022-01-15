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
        private Dictionary
            <
                (
                    ulong systemAddress, 
                    int bodyID
                ), 
                (
                    string bodyName, 
                    int bioTotal,
                    List<string> speciesFound, 
                    List<string> speciesAnalysed
                )
            > BioPlanets;
        ObservableCollection<object> GridCollection;
        private PluginUI pluginUI;
        private bool readAllInProgress = false;
        private Guid? samplerStatusNotification = null;

        public string Name => "Observatory Botanist";

        public string ShortName => "Botanist";

        public string Version => typeof(Botanist).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings { get => null; set { } }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case LoadGame loadGame:
                    OdysseyLoaded = loadGame.Odyssey;
                    break;
                case SAASignalsFound signalsFound:
                    {
                        var systemBodyId = (signalsFound.SystemAddress, signalsFound.BodyID);
                        if (OdysseyLoaded && !BioPlanets.ContainsKey(systemBodyId))
                        {
                            var bioSignals = from signal in signalsFound.Signals
                                             where signal.Type == "$SAA_SignalType_Biological;"
                                             select signal;

                            if (bioSignals.Any())
                            {
                                if (!BioPlanets.ContainsKey(systemBodyId))
                                {
                                    BioPlanets.Add(
                                        systemBodyId,
                                        (signalsFound.BodyName, bioSignals.First().Count, new List<string>(), new List<string>())
                                    );
                                }
                                else
                                {
                                    var bioPlanet = BioPlanets[systemBodyId];
                                    bioPlanet.bodyName = signalsFound.BodyName;
                                    bioPlanet.bioTotal = bioSignals.First().Count;
                                }

                            }
                        }
                    }
                    break;
                case ScanOrganic scanOrganic:
                    {
                        var systemBodyId = (scanOrganic.SystemAddress, scanOrganic.Body);
                        if (!BioPlanets.ContainsKey(systemBodyId))
                        {
                            //Unlikely to ever end up in here, but just in case create a new planet entry.
                            List<string> genus = new();
                            List<string> species = new();
                            genus.Add(scanOrganic.Genus_Localised);
                            species.Add(scanOrganic.Species_Localised);
                            var bioPlanet = (string.Empty, 0, genus, species);
                            BioPlanets.Add(systemBodyId, bioPlanet);
                        }
                        else
                        {
                            var bioPlanet = BioPlanets[systemBodyId];
                            
                            switch (scanOrganic.ScanType)
                            {
                                case ScanOrganicType.Log:
                                case ScanOrganicType.Sample:
                                    if (!readAllInProgress)
                                    {
                                        NotificationArgs args = new()
                                        {
                                            Title = scanOrganic.Species_Localised,
                                            Detail = string.Format("Sample {0} of 3", scanOrganic.ScanType == ScanOrganicType.Log ? 1 : 2),
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

                                    if (!bioPlanet.speciesFound.Contains(scanOrganic.Species_Localised))
                                    {
                                        bioPlanet.speciesFound.Add(scanOrganic.Species_Localised);
                                    }
                                    break;
                                case ScanOrganicType.Analyse:
                                    if (!bioPlanet.speciesAnalysed.Contains(scanOrganic.Species_Localised))
                                    {
                                        bioPlanet.speciesAnalysed.Add(scanOrganic.Species_Localised);
                                    }

                                    if (samplerStatusNotification != null)
                                    {
                                        Core.CancelNotification(samplerStatusNotification.Value);
                                        samplerStatusNotification = null;
                                    }
                                    break;
                            }
                        }
                        UpdateUIGrid();
                    }
                    break;
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

        public void ReadAllStarted()
        {
            readAllInProgress = true;
            Core.ClearGrid(this, new BotanistGrid());
        }

        public void ReadAllFinished()
        {
            readAllInProgress = false;
            UpdateUIGrid();
        }

        private void UpdateUIGrid()
        {
            // Suppress repainting the entire contents of the grid on every ScanOrganic record we read.
            if (readAllInProgress) return;

            BotanistGrid uiObject = new();
            Core.ClearGrid(this, uiObject);
            foreach (var bioPlanet in BioPlanets.Values)
            {
                if (bioPlanet.speciesFound.Count == 0)
                {
                    var planetRow = new BotanistGrid()
                    {
                        Body = bioPlanet.bodyName,
                        BioTotal = bioPlanet.bioTotal.ToString(),
                        Species = "(NO SAMPLES TAKEN)",
                        Analysed = string.Empty
                    };
                    Core.AddGridItem(this, planetRow);
                }

                for (int i = 0; i < bioPlanet.speciesFound.Count; i++)
                {
                    var speciesRow = new BotanistGrid()
                    {
                        Body = i == 0 ? bioPlanet.bodyName : string.Empty,
                        BioTotal = i == 0 ? bioPlanet.bioTotal.ToString() : string.Empty,
                        Species = bioPlanet.speciesFound[i],
                        Analysed = bioPlanet.speciesAnalysed.Contains(bioPlanet.speciesFound[i]) ? "✓" : ""
                    };
                    Core.AddGridItem(this, speciesRow);
                }
            }
        }
    }

    public class BotanistGrid
    {
        public string Body { get; set; }
        public string BioTotal { get; set; }
        public string Species { get; set; }
        public string Analysed { get; set; }
    }
}
