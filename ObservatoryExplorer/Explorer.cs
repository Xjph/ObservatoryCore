using System.Collections.ObjectModel;
using System.Text;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;

namespace Observatory.Explorer
{
    internal class Explorer
    {
        private IObservatoryCore ObservatoryCore;
        private ObservableCollection<object> Results;
        private ExplorerWorker ExplorerWorker;
        private Dictionary<ulong, Dictionary<int, Scan>> SystemBodyHistory;
        private Dictionary<ulong, Dictionary<int, FSSBodySignals>> BodySignalHistory;
        private Dictionary<ulong, Dictionary<int, ScanBaryCentre>> BarycentreHistory;
        private CustomCriteriaManager CustomCriteriaManager;
        private DateTime CriteriaLastModified;
        private string currentSystem = string.Empty;

        internal Explorer(ExplorerWorker explorerWorker, IObservatoryCore core, ObservableCollection<object> results)
        {
            SystemBodyHistory = new();
            BodySignalHistory = new();
            BarycentreHistory = new();
            ExplorerWorker = explorerWorker;
            ObservatoryCore = core;
            Results = results;
            CustomCriteriaManager = new(core.GetPluginErrorLogger(explorerWorker), HandleCustomNotification);
            CriteriaLastModified = new DateTime(0);
        }

        public void Clear()
        {
            SystemBodyHistory.Clear();
            BodySignalHistory.Clear();
            BarycentreHistory.Clear();
        }

        public void RecordSignal(FSSBodySignals bodySignals)
        {
            if (!BodySignalHistory.ContainsKey(bodySignals.SystemAddress))
            {
                BodySignalHistory.Add(bodySignals.SystemAddress, new Dictionary<int, FSSBodySignals>());
            }

            if (!BodySignalHistory[bodySignals.SystemAddress].ContainsKey(bodySignals.BodyID))
            {
                BodySignalHistory[bodySignals.SystemAddress].Add(bodySignals.BodyID, bodySignals);
            }
        }

        public void RecordBarycentre(ScanBaryCentre scan)
        {
            if (!BarycentreHistory.ContainsKey(scan.SystemAddress))
            {
                BarycentreHistory.Add(scan.SystemAddress, new Dictionary<int, ScanBaryCentre>());
            }

            if (!BarycentreHistory[scan.SystemAddress].ContainsKey(scan.BodyID))
            {
                BarycentreHistory[scan.SystemAddress].Add(scan.BodyID, scan);
            }
        }

        private static string IncrementOrdinal(string ordinal)
        {
            char ordChar = ordinal.ToCharArray().Last();

            if (new char[] { 'Z', '9' }.Contains(ordChar))
            {
                ordinal = IncrementOrdinal(ordinal.Length == 1 ? " " : String.Empty + ordinal[..^1]);
                ordChar = (char)(ordChar - 10);
            }

            return ordinal[..^1] + (char)(ordChar + 1);
        }

        private static string DecrementOrdinal(string ordinal)
        {
            char ordChar = ordinal.ToCharArray().Last();

            if (new char[] { 'A', '0' }.Contains(ordChar))
            {
                ordinal = DecrementOrdinal(ordinal.Length == 1 ? " " : String.Empty + ordinal[..^1]);
                ordChar = (char)(ordChar + 10);
            }

            return ordinal[..^1] + (char)(ordChar - 1);
        }

        public Scan ConvertBarycentre(ScanBaryCentre barycentre, Scan childScan)
        {
            string childAffix = childScan.BodyName
                .Replace(childScan.StarSystem, string.Empty).Trim();

            string baryName;

            if (!string.IsNullOrEmpty(childAffix))
            {
                char childOrdinal = childAffix.ToCharArray().Last();

                // If the ID is one higher than the barycentre than this is the "first" child body
                bool lowChild = childScan.BodyID - barycentre.BodyID == 1;

                string baryAffix;

                // Barycentre ordinal always labelled as low before high, e.g. "AB"
                if (lowChild)
                {
                    baryAffix = childAffix + "-" + IncrementOrdinal(childAffix);
                }
                else
                {
                    baryAffix = DecrementOrdinal(childAffix) + "-" + childAffix;
                }

                baryName = barycentre.StarSystem + " " + baryAffix;
            }
            else
            {
                // Without ordinals it's complicated to determine what the ordinal *should* be.
                // Just name the barycentre after the child object.
                baryName = childScan.BodyName + " Barycentre";
            }

            Scan barycentreScan = new()
            {
                Timestamp = barycentre.Timestamp,
                BodyName = baryName,
                Parents = childScan.Parents.RemoveAt(0),
                PlanetClass = "Barycentre",
                StarSystem = barycentre.StarSystem,
                SystemAddress = barycentre.SystemAddress,
                BodyID = barycentre.BodyID,
                SemiMajorAxis = barycentre.SemiMajorAxis,
                Eccentricity = barycentre.Eccentricity,
                OrbitalInclination = barycentre.OrbitalInclination,
                Periapsis = barycentre.Periapsis,
                OrbitalPeriod = barycentre.OrbitalPeriod,
                AscendingNode = barycentre.AscendingNode,
                MeanAnomaly = barycentre.MeanAnomaly,
                Json = barycentre.Json
            };

            return barycentreScan;
        }
        public void SetSystem(string potentialNewSystem)
        {
            if (string.IsNullOrEmpty(currentSystem) || currentSystem != potentialNewSystem)
            {
                currentSystem = potentialNewSystem;
                if (ExplorerWorker.settings.OnlyShowCurrentSystem && !ObservatoryCore.IsLogMonitorBatchReading)
                {
                    ObservatoryCore.ClearGrid(ExplorerWorker, new ExplorerUIResults());
                    Clear();
                }
            }
        }

        public void ProcessScan(Scan scanEvent, bool readAll)
        {
            if (!readAll)
            {
                string criteriaFilePath = ExplorerWorker.settings.CustomCriteriaFile;

                if (File.Exists(criteriaFilePath))
                {
                    DateTime fileModified = new FileInfo(criteriaFilePath).LastWriteTime;

                    if (fileModified != CriteriaLastModified)
                    {
                        try
                        {
                            CustomCriteriaManager.RefreshCriteria(criteriaFilePath);
                        }
                        catch (CriteriaLoadException e)
                        {
                            var exceptionResult = new ExplorerUIResults()
                            {
                                BodyName = "Error Reading Custom Criteria File",
                                Time = DateTime.Now.ToString("s").Replace('T', ' '),
                                Description = e.Message,
                                Details = e.OriginalScript
                            };
                            ObservatoryCore.AddGridItem(ExplorerWorker, exceptionResult);
                        }

                        CriteriaLastModified = fileModified;
                    }
                }
            }

            Dictionary<int, Scan> systemBodies;
            if (SystemBodyHistory.ContainsKey(scanEvent.SystemAddress))
            {
                systemBodies = SystemBodyHistory[scanEvent.SystemAddress];
                if (systemBodies.ContainsKey(scanEvent.BodyID))
                {
                    if (scanEvent.SystemAddress != 0)
                    {
                        //We've already checked this object.
                        return;
                    }
                }
            }
            else
            {
                systemBodies = new();
                SystemBodyHistory.Add(scanEvent.SystemAddress, systemBodies);
            }

            if (SystemBodyHistory.Count > 1000)
            {
                foreach (var entry in SystemBodyHistory.Where(entry => entry.Key != scanEvent.SystemAddress).ToList())
                {
                    SystemBodyHistory.Remove(entry.Key);
                }
                SystemBodyHistory.TrimExcess();
            }

            if (scanEvent.SystemAddress != 0 && !systemBodies.ContainsKey(scanEvent.BodyID))
                systemBodies.Add(scanEvent.BodyID, scanEvent);

            var results = DefaultCriteria.CheckInterest(scanEvent, SystemBodyHistory, BodySignalHistory, ExplorerWorker.settings);

            if (BarycentreHistory.ContainsKey(scanEvent.SystemAddress) && scanEvent.Parent != null && BarycentreHistory[scanEvent.SystemAddress].ContainsKey(scanEvent.Parent[0].Body))
            {
                ProcessScan(ConvertBarycentre(BarycentreHistory[scanEvent.SystemAddress][scanEvent.Parent[0].Body], scanEvent), readAll);
            }

            if (ExplorerWorker.settings.EnableCustomCriteria)
                results.AddRange(CustomCriteriaManager.CheckInterest(scanEvent, SystemBodyHistory, BodySignalHistory, ExplorerWorker.settings));

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    if (result.Description.Contains('\n') || result.Detail.Contains('\n'))
                    {
                        var descriptionLines = result.Description.Split('\n');
                        var detailLines = result.Detail.Split('\n');
                        List<ExplorerUIResults> explorerUIResults = [];
                        var lineOne = new ExplorerUIResults()
                        {
                            BodyName = result.SystemWide ? scanEvent.StarSystem : scanEvent.BodyName,
                            Time = scanEvent.TimestampDateTime.ToString("s").Replace('T', ' '),
                            Description = descriptionLines[0],
                            Details = detailLines[0]
                        };
                        explorerUIResults.Add(lineOne);

                        for (int i = 1; i < Math.Max(descriptionLines.Length, detailLines.Length); i++)
                        {
                            explorerUIResults.Add(new()
                            {
                                Description = i < descriptionLines.Length ? descriptionLines[i] : string.Empty,
                                Details = i < detailLines.Length ? detailLines[i] : string.Empty
                            });
                        }

                        ObservatoryCore.AddGridItems(ExplorerWorker, explorerUIResults);
                    }
                    else
                    {
                        var scanResult = new ExplorerUIResults()
                        {
                            BodyName = result.SystemWide ? scanEvent.StarSystem : scanEvent.BodyName,
                            Time = scanEvent.TimestampDateTime.ToString("s").Replace('T', ' '),
                            Description = result.Description,
                            Details = result.Detail
                        };
                        ObservatoryCore.AddGridItem(ExplorerWorker, scanResult);
                    }

                    SendNotification(scanEvent, result.Description, result.Detail);
                }
            }
        }

        public void ProcessDiscovery(FSSDiscoveryScan discoveryScan)
        {
            if (ExplorerWorker.settings.EnableCustomCriteria)
                CustomCriteriaManager.CustomDiscovery(discoveryScan);
        }


        public void ProcessAllBodies(FSSAllBodiesFound allBodies)
        { 
            if (ExplorerWorker.settings.EnableCustomCriteria)
                CustomCriteriaManager.CustomAllBodies(allBodies); 
        }

        public void ProcessSignalScan(SAASignalsFound signalsFound)
        {
            if (ExplorerWorker.settings.EnableCustomCriteria)
                CustomCriteriaManager.CustomSignals(signalsFound);
        }

        public void ProcessJump(FSDJump jump)
        {
            if (ExplorerWorker.settings.EnableCustomCriteria)
                CustomCriteriaManager.CustomJump(jump);
        }

        private void SendNotification(Scan scanEvent, string detail, string extendedDetail)
        {
            string bodyAffix;

            if (scanEvent.StarSystem != null && scanEvent.BodyName.StartsWith(scanEvent.StarSystem))
            {
                bodyAffix = scanEvent.BodyName.Replace(scanEvent.StarSystem, string.Empty);
            }
            else
            {
                // Use the body name un-modified -- probably an overridden name (ie. Earth in Sol)
                bodyAffix = " " + scanEvent.BodyName;
            }

            string bodyLabel = System.Security.SecurityElement.Escape(scanEvent.PlanetClass == "Barycentre" ? "Barycentre" : "Body");

            string spokenAffix;

            if (bodyAffix.Length > 0)
            {
                if (bodyAffix.Contains("Ring"))
                {
                    int ringIndex = bodyAffix.Length - 6;
                    spokenAffix =
                        "<say-as interpret-as=\"spell-out\">" + bodyAffix[..ringIndex]
                        + "</say-as><break strength=\"weak\"/>" + SplitOrdinalForSsml(bodyAffix.Substring(ringIndex, 1))
                        + bodyAffix[(ringIndex + 1)..];
                }
                else if (bodyAffix.Contains(scanEvent.BodyName))
                {
                    // This contains the entire body name (ie. an override); Speak it out as-is.
                    spokenAffix = scanEvent.BodyName;
                }
                else
                {
                    spokenAffix = SplitOrdinalForSsml(bodyAffix);
                }
            }
            else
            {
                bodyLabel = "Primary Star";
                spokenAffix = string.Empty;
            }

            NotificationArgs args = new()
            {
                Title = bodyLabel + bodyAffix,
                TitleSsml = $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">{bodyLabel} {spokenAffix}</voice></speak>",
                Detail = detail,
                Sender = ExplorerWorker.AboutInfo.ShortName,
                ExtendedDetails = extendedDetail,
                CoalescingId = scanEvent.BodyID,
            };

            ObservatoryCore.SendNotification(args);
        }

        private void SendNotification(string title, string detail, string extendedDetail)
        {
            NotificationArgs args = new()
            {
                Title = title,
                Detail = detail,
                Sender = ExplorerWorker.AboutInfo.ShortName,
                ExtendedDetails = extendedDetail,
                CoalescingId = -1,
            };

            ObservatoryCore.SendNotification(args);
        }

        private void AddGridItem(string eventTime, string title, string detail, string extendedDetail)
        {
            ExplorerUIResults results = new()
            {
                BodyName = title,
                Time = eventTime,
                Description = detail,
                Details = extendedDetail
            };

            ObservatoryCore.AddGridItem(ExplorerWorker, results);
        }

        private void HandleCustomNotification(string eventTime, string title, string detail, string extendedDetail)
        {
            SendNotification(title, detail, extendedDetail);
            AddGridItem(eventTime, title, detail, extendedDetail);
        }

        private static string SplitOrdinalForSsml(string ordinalString)
        {
            var ordinalSegments = ordinalString.Split(' ');
            StringBuilder affix = new();
            foreach (var ordinalSegment in ordinalSegments)
            {
                if (ordinalSegment.All(Char.IsDigit))
                    affix.Append(" " + ordinalSegment);
                else
                    affix.Append("<say-as interpret-as=\"spell-out\">" + ordinalSegment + "</say-as>");
            }
            return affix.ToString();
        }
    }
}
