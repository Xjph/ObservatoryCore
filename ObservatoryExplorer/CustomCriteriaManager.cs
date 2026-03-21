using System.Diagnostics;
using System.Text;
using NLua;
using Observatory.Framework.Files.Journal;

namespace Observatory.Explorer
{
    internal class CustomCriteriaManager
    {
        private Lua LuaState;
        private readonly Dictionary<string, LuaFunction> DiscoveryFunctions = [];
        private readonly Dictionary<string, LuaFunction> BodySignalsFunctions = [];
        private readonly Dictionary<string, LuaFunction> AllBodiesFunctions = [];
        private readonly Dictionary<string, LuaFunction> JumpFunctions = [];
        private readonly Dictionary<string, LuaFunction> CriteriaFunctions = [];
        private readonly Dictionary<string, string> CustomFunctionsErrors = [];
        private ExplorerSettings _settings;
        private readonly Action<Exception, String> ErrorLogger;
        private readonly Action<string, string, string, string, int?> NotificationMethod;
        private uint ScanCount;
        private string eventTime = string.Empty;
        private bool _readAllMode = true;
        private string _criteriaPath = string.Empty;

        public CustomCriteriaManager(
            ExplorerSettings settings,
            Action<Exception, String> errorLogger,
            Action<string, string, string, string, int?> notificationMethod
        )
        {
            ErrorLogger = errorLogger;
            ScanCount = 0;
            NotificationMethod = notificationMethod;
            _readAllMode = false;
            Settings = settings;
        }

        public void SendNotification(string title, string detail, string extendedDetail) =>
            NotificationMethod(eventTime, title, detail, extendedDetail, null);

        public void SendNotificationForBody(
            string title,
            string detail,
            string extendedDetail,
            int bodyId
        ) => NotificationMethod(eventTime, title, detail, extendedDetail, bodyId);

        public ExplorerSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                CriteriaPath = value.CustomCriteriaFile;
            }
        }

        public bool ReadAllMode
        {
            get => _readAllMode;
            set
            {
                if (value != _readAllMode)
                {
                    _readAllMode = value;
                    MaybeRefreshCriteria(true);
                }
            }
        }

        public string CriteriaPath
        {
            get => _criteriaPath;
            set
            {
                if (string.IsNullOrWhiteSpace(value) && value != _criteriaPath)
                {
                    _criteriaPath = value;
                    try
                    {
                        MaybeRefreshCriteria();
                    }
                    catch (CriteriaLoadException ex)
                    {
                        ErrorLogger(ex, $"Failed to load custom criteria from {value}");
                    }
                }
            }
        }

        private DateTime CriteriaLastModified = DateTime.MinValue;

        private bool CriteriaFileNeedsRefresh()
        {
            string criteriaFilePath = Settings.CustomCriteriaFile;

            if (string.IsNullOrWhiteSpace(CriteriaPath) && !CriteriaPath.Equals(criteriaFilePath))
            {
                // Different file detected.
                CriteriaLastModified = DateTime.MinValue;
                _criteriaPath = criteriaFilePath; // Don't use property setter here to avoid side-effects.
            }

            if (File.Exists(criteriaFilePath))
            {
                DateTime fileModified = new FileInfo(criteriaFilePath).LastWriteTime;

                return (fileModified != CriteriaLastModified);
            }
            return false;
        }

        private void MaybeRefreshCriteria(bool isReadAllModeTransition = false)
        {
            if ((ReadAllMode || !CriteriaFileNeedsRefresh()) && !isReadAllModeTransition)
                return;

            LuaState = new();

            LuaState.UseTraceback = !ReadAllMode;
            LuaState.State.Encoding = Encoding.UTF8;
            LuaState.LoadCLRPackage();

            #region Iterators

            // Empty function for nil iterators
            LuaState.DoString("function nil_iterator() end");

            //Materials and AtmosphereComposition
            LuaState.DoString(
                @"
                function materials (material_list)
                    if material_list then
                        local i = 0
                        local count = material_list.Count
                        return function ()
                            i = i + 1
                            if i <= count then
                                return { name = material_list[i - 1].Name, percent = material_list[i - 1].Percent }
                            end
                        end
                    else
                        return nil_iterator
                    end
                end"
            );

            //Rings - internal filterable iterator
            LuaState.DoString(
                @"
                function _ringsFiltered (ring_list, filter_by)
                    if ring_list then
                        local i = 0
                        local count = ring_list.Count
                        return function ()
                            i = i + 1
                            while i <= count do
                                local ring = ring_list[i - 1]
                                if (filter_by == nil or string.find(ring.Name, filter_by)) then
                                    return { name = ring.Name, ringclass = ring.RingClass, massmt = ring.MassMT, innerrad = ring.InnerRad, outerrad = ring.OuterRad }
                                else
                                    i = i + 1
                                end
                            end
                        end
                    else
                        return nil_iterator
                    end
                end"
            );

            //Rings - internal filterable hasX check
            LuaState.DoString(
                @"
                function _hasRingsFiltered (ring_list, filter_by)
                    if ring_list then
                        local i = 0
                        local count = ring_list.Count
                        while i < count do
                            if string.find(ring_list[i].Name, filter_by) then
                                return true
                            end
                            i = i + 1
                        end
                    end
                    return false
                end"
            );

            //Rings - iterate all - nil filter
            LuaState.DoString(
                @"
                function rings (ring_list)
                    return _ringsFiltered(ring_list, nil)
                end"
            );

            //Rings - iterate proper rings only
            LuaState.DoString(
                @"
                function ringsOnly (ring_list)
                    return _ringsFiltered(ring_list, 'Ring')
                end"
            );

            //Rings - iterate belts only
            LuaState.DoString(
                @"
                function beltsOnly (ring_list)
                    return _ringsFiltered(ring_list, 'Belt')
                end"
            );

            //Bodies in system
            LuaState.DoString(
                @"
                function bodies (system_list)
                    if system_list then
                        local i = 0
                        local count = system_list.Count
                        return function ()
                            i = i + 1
                            if i <= count then
                                return system_list[i - 1]
                            end
                        end
                    else
                        return nil_iterator
                    end
                end"
            );

            //Parent bodies
            LuaState.DoString(
                @"
                function allparents (parent_list)
                    if parent_list then
                        local i = 0
                        local count
                        if parent_list then count = parent_list.Count else count = 0 end
                        return function ()
                            i = i + 1
                            if i <= count then
                                return { parenttype = parent_list[i - 1].ParentType, body = parent_list[i - 1].Body, scan = parent_list[i - 1].Scan }
                            end
                        end
                    else
                        return nil_iterator
                    end
                end"
            );

            #endregion

            #region Convenience Functions

            LuaState.RegisterFunction(
                "notify",
                this,
                typeof(CustomCriteriaManager).GetMethod("SendNotification")
            );
            LuaState.RegisterFunction(
                "notifyForBody",
                this,
                typeof(CustomCriteriaManager).GetMethod("SendNotificationForBody")
            );

            // Body type related functions and tests

            //Rings - has > 0 belts
            LuaState.DoString(
                @"
                function hasBelts (ring_list)
                    return _hasRingsFiltered(ring_list, 'Belt')
                end"
            );

            //Body name represents a belt
            LuaState.DoString(
                @"
                function isBelt (body_name)
                    return body_name ~= nil and string.find(body_name, 'Belt')
                end"
            );

            //Rings - has > 0 proper rings
            LuaState.DoString(
                @"
                function hasRings (ring_list)
                    return _hasRingsFiltered(ring_list, 'Ring')
                end"
            );

            //Body name represents a ring
            LuaState.DoString(
                @"
                function isRing (body_name)
                    return body_name ~= nil and string.find(body_name, 'Ring')
                end"
            );

            LuaState.DoString(
                @"
                function isStar (scan)
                    return scan.StarType and scan.StarType ~= ''
                end"
            );

            LuaState.DoString(
                @"
                function isPlanet (scan)
                    return scan.PlanetClass ~= nil and scan.PlanetClass ~= 'Barycentre'
                end"
            );

            LuaState.DoString(
                @"
                function isBarycentre (scan)
                    return scan.PlanetClass ~= nil and scan.PlanetClass == 'Barycentre'
                end"
            );

            // Atmosphere checks

            LuaState.DoString(
                @"
                function hasAtmosphere (scan)
                    return scan.AtmosphereComposition ~= nil
                end"
            );

            LuaState.DoString(
                @"
                function hasLandableAtmosphere (scan)
                    return scan.Landable and scan.AtmosphereComposition ~= nil
                end"
            );

            // Common unit conversion functions and related constants

            // Since LUA has no 'const' keyword, using a metatable as recommended by this:
            // https://andrejs-cainikovs.blogspot.com/2009/05/lua-constants.html
            LuaState.DoString(
                @"
                function protect(tbl)
                    return setmetatable({}, {
                        __index = tbl,
                        __newindex = function(t, key, value)
                            error(""attempting to change constant "" ..
                                   tostring(key) .. "" to "" .. tostring(value), 2)
                        end
                    })
                end

                const = {
                    SPEED_OF_LIGHT_mps = 299792458,
                    GRAVITY_mps2 = 9.81,
                    ATM_PRESSURE_Pa = 101325,
                    DAY_s = 86400,
                    HOUR_s = 3600,
                }
                const = protect(const)
            "
            );

            LuaState.DoString(
                @"
                function distanceAsLs (value_in_m)
                    return value_in_m / const.SPEED_OF_LIGHT_mps
                end"
            );

            LuaState.DoString(
                @"
                function distanceAsKm (value_in_m)
                    return value_in_m / 1000
                end"
            );

            LuaState.DoString(
                @"
                function gravityAsG (value_in_mps2)
                    return value_in_mps2 / const.GRAVITY_mps2
                end"
            );

            LuaState.DoString(
                @"
                function pressureAsAtm (value_in_pa)
                    return value_in_pa / const.ATM_PRESSURE_Pa
                end"
            );

            LuaState.DoString(
                @"
                function periodAsDay (value_in_s)
                    return value_in_s / const.DAY_s
                end"
            );

            LuaState.DoString(
                @"
                function periodAsHour (value_in_s)
                    return value_in_s / const.HOUR_s
                end"
            );

            #endregion

            #region Custom Function loading
            ClearCustomLuaFunctions();

            var criteria = File.Exists(CriteriaPath)
                ? File.ReadAllLines(CriteriaPath)
                : Array.Empty<string>();
            StringBuilder global = new();
            StringBuilder script = new();

            try
            {
                for (int i = 0; i < criteria.Length; i++)
                {
                    if (GetCriteriaAnnotation(criteria[i], out Annotation annotation))
                    {
                        switch (annotation.type)
                        {
                            case AnnotationType.Global:
                                i++;
                                do
                                {
                                    // Stuff this in a separate stringbuilder dedicated for global script content.
                                    global.AppendLine(criteria[i]);
                                    i++;
                                } while (!IsEndAnnotation(criteria[i]));

                                // Insert the global script last.
                                break;
                            case AnnotationType.Complex:
                                i = ParseCustomFunction(
                                    i,
                                    criteria,
                                    script,
                                    annotation,
                                    CriteriaFunctions,
                                    "scan, parents, system, biosignals, geosignals"
                                );
                                break;
                            case AnnotationType.AllBodies:
                                i = ParseCustomFunction(
                                    i,
                                    criteria,
                                    script,
                                    annotation,
                                    AllBodiesFunctions,
                                    "allBodies, system, parentsTable"
                                );
                                break;
                            case AnnotationType.Jump:
                                i = ParseCustomFunction(
                                    i,
                                    criteria,
                                    script,
                                    annotation,
                                    JumpFunctions,
                                    "jump"
                                );
                                break;
                            case AnnotationType.BodySignals:
                                i = ParseCustomFunction(
                                    i,
                                    criteria,
                                    script,
                                    annotation,
                                    BodySignalsFunctions,
                                    "bodySignals"
                                );
                                break;
                            case AnnotationType.Discovery:
                                i = ParseCustomFunction(
                                    i,
                                    criteria,
                                    script,
                                    annotation,
                                    DiscoveryFunctions,
                                    "discovery"
                                );
                                break;
                            default:
                                i++;

                                string functionName = $"Criteria{i}";

                                script.AppendLine(
                                    $"function {functionName} (scan, parents, system, biosignals, geosignals)"
                                );
                                script.AppendLine($"    local result = {criteria[i]}");
                                script.AppendLine("    local detail = ''");

                                if (
                                    criteria.Length > i + 1
                                    && GetCriteriaAnnotation(
                                        criteria[i + 1],
                                        out Annotation detailAnnotation
                                    )
                                    && detailAnnotation.type == AnnotationType.Detail
                                )
                                {
                                    i++;
                                    i++;
                                    // Gate detail evaluation on result to allow safe use of criteria-checked values in detail string.
                                    script.AppendLine("    if result then");
                                    script.AppendLine($"        detail = {criteria[i]}");
                                    script.AppendLine("    end");
                                }

                                script.AppendLine(
                                    $"    return result, '{annotation.value}', detail"
                                );
                                script.AppendLine("end");

                                LuaState.DoString(script.ToString());
                                CriteriaFunctions.Add(
                                    GetUniqueDescription(functionName, annotation.value),
                                    LuaState[functionName] as LuaFunction
                                );
                                script.Clear();
                                break;
                        }
                    }
                }

                // Stuff the global content in.
                script = global; // for error handling; just in case this fail.
                LuaState.DoString(global.ToString());

                // We succeeded. Update to latest modified time to gate further refreshes.
                CriteriaLastModified = new FileInfo(CriteriaPath).LastWriteTime;
            }
            catch (Exception e)
            {
                string originalScript = script.ToString().Trim();

                originalScript = originalScript.Remove(
                    originalScript.LastIndexOf(Environment.NewLine)
                );
                originalScript = originalScript[
                    (originalScript.IndexOf(Environment.NewLine) + Environment.NewLine.Length)..
                ];
                originalScript = originalScript.Replace('\t', ' ');

                StringBuilder errorDetail = new();
                errorDetail
                    .AppendLine("Error Reading Custom Criteria File:")
                    .AppendLine(originalScript)
                    .AppendLine(
                        "To correct this problem, make changes to the Lua source file, save it and either re-run read-all or scan another body. It will be automatically reloaded."
                    );
                ErrorLogger(e, errorDetail.ToString());
                ClearCustomLuaFunctions(); // Don't use partial parse.
                throw new CriteriaLoadException(e.Message, originalScript);
            }
            #endregion
        }

        private int ParseCustomFunction(
            int i,
            string[] criteria,
            StringBuilder script,
            Annotation annotation,
            Dictionary<string, LuaFunction> funcs,
            string paramList
        )
        {
            i++;
            string functionName = $"Observatory{annotation.type}Handler{i}";
            script.AppendLine($"function {functionName} ({paramList})");
            do
            {
                if (i >= criteria.Length)
                    throw new Exception(
                        $"Unterminated {annotation.type} handler.\r\nAre you missing an End annotation?"
                    );

                script.AppendLine(criteria[i]);
                i++;
            } while (!IsEndAnnotation(criteria[i]));
            script.AppendLine("end");

            LuaState.DoString(script.ToString());
            funcs.Add(
                GetUniqueDescription(functionName, annotation.value),
                LuaState[functionName] as LuaFunction
            );
            script.Clear();
            return i;
        }

        private void ClearCustomLuaFunctions()
        {
            CriteriaFunctions.Clear();
            CustomFunctionsErrors.Clear();
            AllBodiesFunctions.Clear();
            BodySignalsFunctions.Clear();
            DiscoveryFunctions.Clear();
            JumpFunctions.Clear();
        }

        public List<(string, string, bool)> CheckInterest(
            Scan scan,
            Dictionary<ulong, Dictionary<int, Scan>> scanHistory,
            Dictionary<ulong, Dictionary<int, FSSBodySignals>> signalHistory,
            ExplorerSettings settings
        )
        {
            MaybeRefreshCriteria();

            List<(string, string, bool)> results = new();
            ScanCount++;

            foreach (var criteriaFunction in CriteriaFunctions)
            {
                // Skip criteria which have previously thrown an error. We can't remove them from the dictionary while iterating it.
                if (CustomFunctionsErrors.ContainsKey(criteriaFunction.Key))
                    continue;

                var scanList = scanHistory[scan.SystemAddress].Values.ToList();

                int bioSignals;
                int geoSignals;

                if (
                    signalHistory.ContainsKey(scan.SystemAddress)
                    && signalHistory[scan.SystemAddress].ContainsKey(scan.BodyID)
                )
                {
                    bioSignals = signalHistory[scan.SystemAddress]
                        [scan.BodyID]
                        .Signals.Where(s => s.Type == "$SAA_SignalType_Biological;")
                        .Select(s => s.Count)
                        .FirstOrDefault();
                    geoSignals = signalHistory[scan.SystemAddress]
                        [scan.BodyID]
                        .Signals.Where(s => s.Type == "$SAA_SignalType_Geological;")
                        .Select(s => s.Count)
                        .FirstOrDefault();
                }
                else
                {
                    bioSignals = 0;
                    geoSignals = 0;
                }

                List<Parent> parents = MakeParentsList(scan, scanHistory[scan.SystemAddress]);

                try
                {
                    var result = criteriaFunction.Value.Call(
                        scan,
                        parents,
                        scanList,
                        bioSignals,
                        geoSignals
                    );
                    if (result.Length == 3 && ((bool?)result[0]).GetValueOrDefault(false))
                    {
                        results.Add((result[1].ToString(), result[2].ToString(), false));
                    }
                    else if (result.Length == 2)
                    {
                        results.Add((result[0].ToString(), result[1].ToString(), false));
                    }
                }
                catch (NLua.Exceptions.LuaScriptException e)
                {
                    results.Add((e.Message, scan.Json, false));

                    StringBuilder errorDetail = new();
                    errorDetail
                        .AppendLine(
                            $"while processing custom criteria '{criteriaFunction.Key}' on scan:"
                        )
                        .AppendLine(scan.Json)
                        .AppendLine(
                            "To correct this problem, make changes to the Lua source file, save it and either re-run read-all or scan another body. It will be automatically reloaded."
                        );
                    ErrorLogger(e, errorDetail.ToString());
                    CustomFunctionsErrors.Add(
                        criteriaFunction.Key,
                        e.Message + Environment.NewLine + errorDetail.ToString()
                    );
                }
            }

            MaybeRemoveFailingFunctions(CriteriaFunctions);

            if (ScanCount > 99)
            {
                ScanCount = 0;
                LuaGC();
            }

            return results;
        }

        public void RunCustomFunctions<T>(
            T journal,
            Dictionary<string, LuaFunction> customFunctions,
            Dictionary<ulong, Dictionary<int, Scan>> scanHistory
        )
            where T : JournalBase
        {
            MaybeRefreshCriteria();

            StoreTimeString(journal);
            foreach (var customFunc in customFunctions)
            {
                if (CustomFunctionsErrors.ContainsKey(customFunc.Key))
                    continue; // This has failed previously.

                try
                {
                    switch (journal)
                    {
                        case FSSAllBodiesFound allBodies:
                            Debug.Assert(
                                scanHistory is not null,
                                "FSSAllBodiesFound requires scanHistory to be provided"
                            );
                            var systemScans = scanHistory[allBodies.SystemAddress];
                            var scanList = systemScans.Values.ToList();
                            Dictionary<int, List<Parent>> parentsPerBodyId = scanList
                                .Where(s => s.Parent is not null)
                                .ToDictionary(s => s.BodyID, s => MakeParentsList(s, systemScans));

                            customFunc.Value.Call(journal, scanList, parentsPerBodyId);
                            break;
                        default:
                            customFunc.Value.Call(journal);
                            break;
                    }
                }
                catch (NLua.Exceptions.LuaScriptException e)
                {
                    StringBuilder errorDetail = new();
                    errorDetail
                        .AppendLine(
                            $"while processing custom criteria '{customFunc.Key}' on {journal.Event}:"
                        )
                        .AppendLine(journal.Json)
                        .AppendLine(
                            "To correct this problem, make changes to the Lua source file, save it and either re-run read-all or scan another body. It will be automatically reloaded."
                        );
                    ErrorLogger(e, errorDetail.ToString());
                    CustomFunctionsErrors.Add(
                        customFunc.Key,
                        e.Message + Environment.NewLine + errorDetail.ToString()
                    );
                }
            }

            MaybeRemoveFailingFunctions(customFunctions);
        }

        public void CustomDiscovery(
            FSSDiscoveryScan scan,
            Dictionary<ulong, Dictionary<int, Scan>> scanHistory
        )
        {
            RunCustomFunctions(scan, DiscoveryFunctions, scanHistory);
        }

        public void CustomAllBodies(
            FSSAllBodiesFound allBodies,
            Dictionary<ulong, Dictionary<int, Scan>> scanHistory
        )
        {
            RunCustomFunctions(allBodies, AllBodiesFunctions, scanHistory);
        }

        public void CustomJump(FSDJump jump, Dictionary<ulong, Dictionary<int, Scan>> scanHistory)
        {
            RunCustomFunctions(jump, JumpFunctions, scanHistory);
        }

        public void CustomSignals(
            SAASignalsFound signalsFound,
            Dictionary<ulong, Dictionary<int, Scan>> scanHistory
        )
        {
            RunCustomFunctions(signalsFound, BodySignalsFunctions, scanHistory);
        }

        private void StoreTimeString(JournalBase journal)
        {
            eventTime = journal.TimestampDateTime.ToString("s").Replace('T', ' ');
        }

        private class Annotation
        {
            public AnnotationType type { get; init; }
            public string value { get; init; }
        }

        private bool GetCriteriaAnnotation(string line, out Annotation annotation)
        {
            line = line.Trim();

            if (line.StartsWith("::") || line.StartsWith("---@"))
            {
                string annotationRaw = line.Replace("::", string.Empty)
                    .Replace("---@", string.Empty);
                string directive = annotationRaw.Split(' ')[0].Split('=')[0].ToLower(); // Gross, but handles both formats
                string debugLabel;
                if (annotationRaw.ToLower().StartsWith($"{directive}="))
                {
                    debugLabel = annotationRaw[(directive.Length + 1)..];
                }
                else if (annotationRaw.Contains(' '))
                {
                    debugLabel = string.Join(' ', annotationRaw.Split(' ')[1..]);
                }
                else
                {
                    debugLabel = string.Empty;
                }
                switch (directive)
                {
                    case "end":
                        annotation = new() { type = AnnotationType.End, value = string.Empty };
                        return true;
                    case "global":
                        annotation = new() { type = AnnotationType.Global, value = string.Empty };
                        return true;
                    case "detail":
                        annotation = new() { type = AnnotationType.Detail, value = string.Empty };
                        return true;
                    case "criteria":
                    case "complex":
                        annotation = new() { type = AnnotationType.Complex, value = debugLabel };
                        return true;
                    case "allbodies":
                        annotation = new() { type = AnnotationType.AllBodies, value = debugLabel };
                        return true;
                    case "jump":
                        annotation = new() { type = AnnotationType.Jump, value = debugLabel };
                        return true;
                    case "bodysignals":
                        annotation = new()
                        {
                            type = AnnotationType.BodySignals,
                            value = debugLabel,
                        };
                        return true;
                    case "discovery":
                        annotation = new() { type = AnnotationType.Discovery, value = debugLabel };
                        return true;
                    default:
                        annotation = new() { type = AnnotationType.Simple, value = debugLabel };
                        return true;
                }
            }

            annotation = new() { type = AnnotationType.None, value = string.Empty };
            return false;
        }

        private bool IsEndAnnotation(string line)
        {
            return GetCriteriaAnnotation(line, out Annotation endAnnotation)
                && endAnnotation.type == AnnotationType.End;
        }

        private enum AnnotationType
        {
            Simple,
            Complex,
            Detail,
            Global,
            End,
            None,
            AllBodies,
            Jump,
            BodySignals,
            Discovery,
        }

        private string GetUniqueDescription(string functionName, string scriptDescription)
        {
            string uniqueDescription = functionName;
            if (!string.IsNullOrWhiteSpace(scriptDescription) && scriptDescription != functionName)
            {
                uniqueDescription += '=' + scriptDescription;
            }
            return uniqueDescription;
        }

        private void LuaGC()
        {
            LuaState?.DoString("collectgarbage()");
        }

        // Remove any erroring criteria. They will be repopulated next time the file is parsed.
        private void MaybeRemoveFailingFunctions(Dictionary<string, LuaFunction> funcList)
        {
            if (CustomFunctionsErrors.Count > 0)
            {
                foreach (var criteriaKey in CustomFunctionsErrors.Keys)
                {
                    funcList.Remove(criteriaKey);
                }
            }
        }

        private static List<Parent> MakeParentsList(Scan scan, Dictionary<int, Scan> scanList)
        {
            if (scan.Parent is null)
                return null;

            List<Parent> parents = [];
            foreach (var parent in scan.Parent)
            {
                var parentScan = scanList.GetValueOrDefault(parent.Body, null);

                parents.Add(
                    new Parent()
                    {
                        ParentType = parent.ParentType.ToString(),
                        Body = parent.Body,
                        Scan = parentScan,
                    }
                );
            }
            return parents;
        }

        internal class Parent
        {
            public string ParentType;
            public int Body;
            public Scan Scan;
        }
    }
}
