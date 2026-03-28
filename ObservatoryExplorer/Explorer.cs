using System.Text;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;

namespace Observatory.Explorer
{
    internal class Explorer
    {
        private IObservatoryCore ObservatoryCore;
        private ExplorerWorker ExplorerWorker;
        private Dictionary<ulong, Dictionary<int, Scan>> SystemBodyHistory;
        private Dictionary<ulong, Dictionary<int, FSSBodySignals>> BodySignalHistory;
        private Dictionary<ulong, Dictionary<int, ScanBaryCentre>> BarycentreHistory;
        private CustomCriteriaManager CustomCriteriaManager;
        private string currentSystem = string.Empty;
        private ulong currentSystemId64 = 0; // for indexing into above dictionaries.

        internal Explorer(
            ExplorerWorker explorerWorker,
            IObservatoryCore core
        )
        {
            SystemBodyHistory = new();
            BodySignalHistory = new();
            BarycentreHistory = new();
            ExplorerWorker = explorerWorker;
            ObservatoryCore = core;
            CustomCriteriaManager = new(
                ExplorerWorker.settings,
                core.GetPluginErrorLogger(explorerWorker),
                HandleCustomNotification
            );
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            if (LogMonitorStateChangedEventArgs.IsBatchRead(args.NewState))
            {
                // Beginning a batch read. Clear grid.
                ObservatoryCore.ClearGrid(ExplorerWorker, new ExplorerUIResults());
                Clear();

                // Entering read-all (not pre-read).
                if (
                    !args.PreviousState.HasFlag(LogMonitorState.Batch)
                    && args.NewState.HasFlag(LogMonitorState.Batch)
                )
                {
                    CustomCriteriaManager.ReadAllMode = true;
                }
            }
            // Exiting Read-all.
            else if (
                args.PreviousState.HasFlag(LogMonitorState.Batch)
                & !args.NewState.HasFlag(LogMonitorState.Batch)
            )
            {
                CustomCriteriaManager.ReadAllMode = false;
            }
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
                BodySignalHistory.Add(
                    bodySignals.SystemAddress,
                    new Dictionary<int, FSSBodySignals>()
                );
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
                ordinal = IncrementOrdinal(
                    ordinal.Length == 1 ? " " : String.Empty + ordinal[..^1]
                );
                ordChar = (char)(ordChar - 10);
            }

            return ordinal[..^1] + (char)(ordChar + 1);
        }

        private static string DecrementOrdinal(string ordinal)
        {
            char ordChar = ordinal.ToCharArray().Last();

            if (new char[] { 'A', '0' }.Contains(ordChar))
            {
                ordinal = DecrementOrdinal(
                    ordinal.Length == 1 ? " " : String.Empty + ordinal[..^1]
                );
                ordChar = (char)(ordChar + 10);
            }

            return ordinal[..^1] + (char)(ordChar - 1);
        }

        public Scan ConvertBarycentre(ScanBaryCentre barycentre, Scan childScan)
        {
            string childAffix = childScan
                .BodyName.Replace(childScan.StarSystem, string.Empty)
                .Trim();

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
                Json = barycentre.Json,
            };

            return barycentreScan;
        }

        public void SetSystem(string potentialNewSystem, ulong id64)
        {
            if (string.IsNullOrEmpty(currentSystem) || currentSystem != potentialNewSystem)
            {
                currentSystem = potentialNewSystem;
                currentSystemId64 = id64;
                if (
                    ExplorerWorker.settings.OnlyShowCurrentSystem
                    && !ObservatoryCore.IsLogMonitorBatchReading
                )
                {
                    ObservatoryCore.ClearGrid(ExplorerWorker, new ExplorerUIResults());
                    Clear();
                }
            }
        }

        public void ProcessScan(Scan scanEvent)
        {
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
                foreach (
                    var entry in SystemBodyHistory
                        .Where(entry => entry.Key != scanEvent.SystemAddress)
                        .ToList()
                )
                {
                    SystemBodyHistory.Remove(entry.Key);
                }
                SystemBodyHistory.TrimExcess();
            }

            if (scanEvent.SystemAddress != 0 && !systemBodies.ContainsKey(scanEvent.BodyID))
                systemBodies.Add(scanEvent.BodyID, scanEvent);

            var results = DefaultCriteria.CheckInterest(
                scanEvent,
                SystemBodyHistory,
                BodySignalHistory,
                ExplorerWorker.settings
            );

            if (
                BarycentreHistory.ContainsKey(scanEvent.SystemAddress)
                && scanEvent.Parent != null
                && BarycentreHistory[scanEvent.SystemAddress].ContainsKey(scanEvent.Parent[0].Body)
            )
            {
                ProcessScan(
                    ConvertBarycentre(
                        BarycentreHistory[scanEvent.SystemAddress][scanEvent.Parent[0].Body],
                        scanEvent
                    )
                );
            }

            if (ExplorerWorker.settings.EnableCustomCriteria)
                results.AddRange(
                    CustomCriteriaManager.CheckInterest(
                        scanEvent,
                        SystemBodyHistory,
                        BodySignalHistory,
                        ExplorerWorker.settings
                    )
                );

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
                            BodyName = result.SystemWide
                                ? scanEvent.StarSystem
                                : scanEvent.BodyName,
                            Time = scanEvent.TimestampDateTime.ToString("s").Replace('T', ' '),
                            Description = descriptionLines[0],
                            Details = detailLines[0],
                        };
                        explorerUIResults.Add(lineOne);

                        for (
                            int i = 1;
                            i < Math.Max(descriptionLines.Length, detailLines.Length);
                            i++
                        )
                        {
                            explorerUIResults.Add(
                                new()
                                {
                                    Description =
                                        i < descriptionLines.Length
                                            ? descriptionLines[i]
                                            : string.Empty,
                                    Details =
                                        i < detailLines.Length ? detailLines[i] : string.Empty,
                                }
                            );
                        }

                        ObservatoryCore.AddGridItems(ExplorerWorker, explorerUIResults);
                    }
                    else
                    {
                        var scanResult = new ExplorerUIResults()
                        {
                            BodyName = result.SystemWide
                                ? scanEvent.StarSystem
                                : scanEvent.BodyName,
                            Time = scanEvent.TimestampDateTime.ToString("s").Replace('T', ' '),
                            Description = result.Description,
                            Details = result.Detail,
                        };
                        ObservatoryCore.AddGridItem(ExplorerWorker, scanResult);
                    }

                    SendNotification(scanEvent, result.Description, result.Detail);
                }
            }
        }

        public void ProcessCodexEntry(CodexEntry codexEntry)
        {
            if (ExplorerWorker.settings.Codex && codexEntry.IsNewEntry)
            {
                SystemBodyHistory.TryGetValue(codexEntry.SystemAddress, out var bodiesInSystem);
#nullable enable
                Scan? bodyScan = null;
#nullable disable
                bodiesInSystem?.TryGetValue(codexEntry.BodyID, out bodyScan);
                ObservatoryCore.AddGridItem(
                    ExplorerWorker,
                    new ExplorerUIResults()
                    {
                        BodyName = bodyScan?.BodyName ?? codexEntry.System,
                        Time = codexEntry.TimestampDateTime.ToString("s").Replace('T', ' '),
                        Description = $"New {codexEntry.Category_Localised} Codex Entry",
                        Details = codexEntry.Name_Localised,
                    }
                );
                SendNotification(
                    $"New {codexEntry.Category_Localised} Codex Entry",
                    codexEntry.Name_Localised,
                    string.Empty
                );
            }
        }

        public void ProcessDiscovery(FSSDiscoveryScan discoveryScan)
        {
            if (ExplorerWorker.settings.EnableCustomCriteria)
                CustomCriteriaManager.CustomDiscovery(discoveryScan, SystemBodyHistory);
        }

        public void ProcessAllBodies(FSSAllBodiesFound allBodies)
        {
            if (ExplorerWorker.settings.EnableCustomCriteria)
                CustomCriteriaManager.CustomAllBodies(allBodies, SystemBodyHistory);
        }

        public void ProcessSignalScan(SAASignalsFound signalsFound)
        {
            if (ExplorerWorker.settings.EnableCustomCriteria)
                CustomCriteriaManager.CustomSignals(signalsFound, SystemBodyHistory);
        }

        public void ProcessJump(FSDJump jump)
        {
            if (ExplorerWorker.settings.EnableCustomCriteria)
                CustomCriteriaManager.CustomJump(jump, SystemBodyHistory);
        }

        private void SendNotification(Scan scanEvent, string detail, string extendedDetail)
        {
            string bodyAffix, bodyLabel, spokenAffix;
            MakeShortBodyTitle(scanEvent.StarSystem, scanEvent.BodyName, scanEvent.PlanetClass == "Barycentre", out bodyAffix, out bodyLabel, out spokenAffix);

            NotificationArgs args = new()
            {
                Title = bodyLabel + bodyAffix,
                TitleSsml =
                    $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">{bodyLabel} {spokenAffix}</voice></speak>",
                Detail = detail,
                Sender = ExplorerWorker.AboutInfo.ShortName,
                ExtendedDetails = extendedDetail,
                CoalescingId = scanEvent.BodyID,
            };

            ObservatoryCore.SendNotification(args);
        }

        private void SendNotification(
            string title,
            string detail,
            string extendedDetail,
            int? coalescingId = null
        )
        {
            // See if the title is a system or body name that we have scans for and if so, handle it like native explorer notifications.
            if (title.StartsWith(currentSystem) && SystemBodyHistory.ContainsKey(currentSystemId64) && coalescingId.HasValue && coalescingId.Value >= 0)
            {
                var systemBodies = SystemBodyHistory[currentSystemId64];

                ScanBaryCentre barycentreFromCoalescingId = null;
                if (BarycentreHistory.ContainsKey(currentSystemId64))
                    BarycentreHistory[currentSystemId64].TryGetValue(coalescingId.Value, out barycentreFromCoalescingId);

                string bodyAffix, bodyLabel, spokenAffix;
                MakeShortBodyTitle(currentSystem, title, barycentreFromCoalescingId is not null, out bodyAffix, out bodyLabel, out spokenAffix);

                NotificationArgs argsWithShortName = new()
                {
                    Title = bodyLabel + bodyAffix,
                    TitleSsml =
                        $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">{bodyLabel} {spokenAffix}</voice></speak>",
                    Detail = detail,
                    Sender = ExplorerWorker.AboutInfo.ShortName,
                    ExtendedDetails = extendedDetail,
                    CoalescingId = coalescingId ?? -1,
                };
                ObservatoryCore.SendNotification(argsWithShortName);
                return;
            }

            NotificationArgs args = new()
            {
                Title = title,
                Detail = detail,
                Sender = ExplorerWorker.AboutInfo.ShortName,
                ExtendedDetails = extendedDetail,
                CoalescingId = coalescingId ?? -1,
            };

            ObservatoryCore.SendNotification(args);
        }

        private static void MakeShortBodyTitle(string systemName, string bodyName, bool bodyIsBarycentre,
            out string bodyAffix, out string bodyLabel, out string spokenAffix)
        {
            if (systemName != null && bodyName.StartsWith(systemName))
            {
                bodyAffix = bodyName.Replace(systemName, string.Empty);
            }
            else
            {
                // Use the body name un-modified -- probably an overridden name (ie. Earth in Sol)
                bodyAffix = " " + bodyName;
            }

            bodyLabel = System.Security.SecurityElement.Escape(bodyIsBarycentre ? "Barycentre" : "Body");
            if (bodyAffix.Length > 0)
            {
                if (bodyAffix.Contains("Ring"))
                {
                    int ringIndex = bodyAffix.Length - 6;
                    spokenAffix =
                        "<say-as interpret-as=\"spell-out\">"
                        + bodyAffix[..ringIndex]
                        + "</say-as><break strength=\"weak\"/>"
                        + SplitOrdinalForSsml(bodyAffix.Substring(ringIndex, 1))
                        + bodyAffix[(ringIndex + 1)..];
                }
                else if (bodyAffix.Contains(bodyName))
                {
                    // This contains the entire body name (ie. an override); Speak it out as-is.
                    spokenAffix = bodyName;
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
        }

        private void AddGridItem(
            string eventTime,
            string title,
            string detail,
            string extendedDetail
        )
        {
            ExplorerUIResults results = new()
            {
                BodyName = title,
                Time = eventTime,
                Description = detail,
                Details = extendedDetail,
            };

            ObservatoryCore.AddGridItem(ExplorerWorker, results);
        }

        private void HandleCustomNotification(
            string eventTime,
            string title,
            string detail,
            string extendedDetail,
            int? coalescingId = null
        )
        {
            SendNotification(title, detail, extendedDetail, coalescingId);
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
                    affix.Append(
                        "<say-as interpret-as=\"spell-out\">" + ordinalSegment + "</say-as>"
                    );
            }
            return affix.ToString();
        }
    }
}
