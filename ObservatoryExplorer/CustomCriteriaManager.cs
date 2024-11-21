using System.Text;
using Observatory.Framework.Files.Journal;
using NLua;

namespace Observatory.Explorer
{
    internal class CustomCriteriaManager
    {
        private Lua LuaState;
        private LuaFunction DiscoveryFunction;
        private LuaFunction BodySignalsFunction;
        private LuaFunction AllBodiesFunction;
        private LuaFunction JumpFunction;
        private bool hasAllBodiesFunc = false;
        private bool hasDiscoveryFunc = false;
        private bool hasJumpFunc = false;
        private bool hasBodySignalsFunc = false;
        private Dictionary<String,LuaFunction> CriteriaFunctions;
        private Dictionary<string, string> CriteriaWithErrors = new();
        Action<Exception, String> ErrorLogger;
        private Action<string, string, string, string> NotificationMethod;
        private uint ScanCount;
        private string eventTime = string.Empty;

        public CustomCriteriaManager(Action<Exception, String> errorLogger, Action<string, string, string, string> notificationMethod)
        {
            ErrorLogger = errorLogger;
            CriteriaFunctions = new();
            ScanCount = 0;
            NotificationMethod = notificationMethod;
        }

        public void SendNotification(string title, string detail, string extendedDetail) => NotificationMethod(eventTime, title, detail, extendedDetail);

        public void RefreshCriteria(string criteriaPath)
        {
            LuaState = new();
            LuaState.State.Encoding = Encoding.UTF8;
            LuaState.LoadCLRPackage();

            #region Iterators

            // Empty function for nil iterators
            LuaState.DoString("function nil_iterator() end");

            //Materials and AtmosphereComposition
            LuaState.DoString(@"
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
                end");

            //Rings - internal filterable iterator
            LuaState.DoString(@"
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
                end");

            //Rings - internal filterable hasX check
            LuaState.DoString(@"
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
                end");

            //Rings - iterate all - nil filter
            LuaState.DoString(@"
                function rings (ring_list)
                    return _ringsFiltered(ring_list, nil)
                end");

            //Rings - iterate proper rings only
            LuaState.DoString(@"
                function ringsOnly (ring_list)
                    return _ringsFiltered(ring_list, 'Ring')
                end");

            //Rings - iterate belts only
            LuaState.DoString(@"
                function beltsOnly (ring_list)
                    return _ringsFiltered(ring_list, 'Belt')
                end");

            //Bodies in system
            LuaState.DoString(@"
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
                end");

            //Parent bodies
            LuaState.DoString(@"
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
                end");

            #endregion

            #region Convenience Functions

            LuaState.RegisterFunction("notify", this, typeof(CustomCriteriaManager).GetMethod("SendNotification"));

            // Body type related functions and tests

            //Rings - has > 0 belts
            LuaState.DoString(@"
                function hasBelts (ring_list)
                    return _hasRingsFiltered(ring_list, 'Belt')
                end");

            //Body name represents a belt
            LuaState.DoString(@"
                function isBelt (body_name)
                    return body_name ~= nil and string.find(body_name, 'Belt')
                end");

            //Rings - has > 0 proper rings
            LuaState.DoString(@"
                function hasRings (ring_list)
                    return _hasRingsFiltered(ring_list, 'Ring')
                end");

            //Body name represents a ring
            LuaState.DoString(@"
                function isRing (body_name)
                    return body_name ~= nil and string.find(body_name, 'Ring')
                end");

            LuaState.DoString(@"
                function isStar (scan)
                    return scan.StarType and scan.StarType ~= ''
                end");

            LuaState.DoString(@"
                function isPlanet (scan)
                    return scan.PlanetClass ~= nil and scan.PlanetClass ~= 'Barycentre'
                end");

            LuaState.DoString(@"
                function isBarycentre (scan)
                    return scan.PlanetClass ~= nil and scan.PlanetClass == 'Barycentre'
                end");

            // Atmosphere checks

            LuaState.DoString(@"
                function hasAtmosphere (scan)
                    return scan.AtmosphereComposition ~= nil
                end");

            LuaState.DoString(@"
                function hasLandableAtmosphere (scan)
                    return scan.Landable and scan.AtmosphereComposition ~= nil
                end");

            // Common unit conversion functions and related constants

            // Since LUA has no 'const' keyword, using a metatable as recommended by this:
            // https://andrejs-cainikovs.blogspot.com/2009/05/lua-constants.html
            LuaState.DoString(@"
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
            ");

            LuaState.DoString(@"
                function distanceAsLs (value_in_m)
                    return value_in_m / const.SPEED_OF_LIGHT_mps
                end");

            LuaState.DoString(@"
                function distanceAsKm (value_in_m)
                    return value_in_m / 1000
                end");

            LuaState.DoString(@"
                function gravityAsG (value_in_mps2)
                    return value_in_mps2 / const.GRAVITY_mps2
                end");

            LuaState.DoString(@"
                function pressureAsAtm (value_in_pa)
                    return value_in_pa / const.ATM_PRESSURE_Pa
                end");

            LuaState.DoString(@"
                function periodAsDay (value_in_s)
                    return value_in_s / const.DAY_s
                end");

            LuaState.DoString(@"
                function periodAsHour (value_in_s)
                    return value_in_s / const.HOUR_s
                end");

            #endregion

            CriteriaFunctions.Clear();
            CriteriaWithErrors.Clear();
            var criteria = File.Exists(criteriaPath) ? File.ReadAllLines(criteriaPath) : Array.Empty<string>();
            StringBuilder script = new();

            try
            {
                var IsEndAnnotation = (string line) => 
                GetCriteriaAnnotation(line, out Annotation endAnnotation) && endAnnotation.type == AnnotationType.End;

                for (int i = 0; i < criteria.Length; i++)
                {
                    if (GetCriteriaAnnotation(criteria[i], out Annotation annotation))
                    {
                        if (annotation.type == AnnotationType.Complex)
                        {
                            string functionName = $"Criteria{i}";
                            script.AppendLine($"function {functionName} (scan, parents, system, biosignals, geosignals)");
                            i++;
                            do
                            {
                                if (i >= criteria.Length)
                                    throw new Exception("Unterminated multi-line criteria.\r\nAre you missing an End annotation?");

                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!IsEndAnnotation(criteria[i]));
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            CriteriaFunctions.Add(GetUniqueDescription(functionName, annotation.value), LuaState[functionName] as LuaFunction);
                            script.Clear();
                        }
                        else if (annotation.type == AnnotationType.Global)
                        {
                            i++;
                            do
                            {
                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!IsEndAnnotation(criteria[i]));
                            LuaState.DoString(script.ToString());
                            script.Clear();
                        }
                        else if (annotation.type == AnnotationType.AllBodies)
                        {
                            if (hasAllBodiesFunc) throw new CriteriaLoadException("Multiple AllBodies annotations found.");
                            hasAllBodiesFunc = true;
                            
                            script.AppendLine($"function ObservatoryAllBodiesHandler (allBodies)");
                            i++;
                            do
                            {
                                if (i >= criteria.Length)
                                    throw new Exception("Unterminated AllBodies handler.\r\nAre you missing an End annotation?");

                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!IsEndAnnotation(criteria[i]));
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            AllBodiesFunction = LuaState["ObservatoryAllBodiesHandler"] as LuaFunction;
                            script.Clear();
                        }
                        else if (annotation.type == AnnotationType.Jump)
                        {
                            if (hasJumpFunc) throw new CriteriaLoadException("Multiple Jump annotations found.");
                            hasJumpFunc = true;

                            script.AppendLine($"function ObservatoryJumpHandler (jump)");
                            i++;
                            do
                            {
                                if (i >= criteria.Length)
                                    throw new Exception("Unterminated Jump handler.\r\nAre you missing an End annotation?");

                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!IsEndAnnotation(criteria[i]));
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            JumpFunction = LuaState["ObservatoryJumpHandler"] as LuaFunction;
                            script.Clear();
                        }
                        else if (annotation.type == AnnotationType.BodySignals)
                        {
                            if (hasBodySignalsFunc) throw new CriteriaLoadException("Multiple BodySignals annotations found.");
                            hasBodySignalsFunc = true;

                            script.AppendLine($"function ObservatoryBodySignalsHandler (bodySignals)");
                            i++;
                            do
                            {
                                if (i >= criteria.Length)
                                    throw new Exception("Unterminated BodySignals handler.\r\nAre you missing an End annotation?");

                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!IsEndAnnotation(criteria[i]));
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            BodySignalsFunction = LuaState["ObservatoryBodySignalsHandler"] as LuaFunction;
                            script.Clear();
                        }
                        else if (annotation.type == AnnotationType.Discovery)
                        {
                            if (hasDiscoveryFunc) throw new CriteriaLoadException("Multiple Discovery annotations found.");
                            hasDiscoveryFunc = true;

                            script.AppendLine($"function ObservatoryDiscoveryHandler (discovery)");
                            i++;
                            do
                            {
                                if (i >= criteria.Length)
                                    throw new Exception("Unterminated Discovery handler.\r\nAre you missing an End annotation?");

                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!IsEndAnnotation(criteria[i]));
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            DiscoveryFunction = LuaState["ObservatoryDiscoveryHandler"] as LuaFunction;
                            script.Clear();
                        }
                        else
                        {
                            i++;

                            string functionName = $"Criteria{i}";

                            script.AppendLine($"function {functionName} (scan, parents, system, biosignals, geosignals)");
                            script.AppendLine($"    local result = {criteria[i]}");
                            script.AppendLine("    local detail = ''");

                            if (criteria.Length > i + 1
                                && GetCriteriaAnnotation(criteria[i + 1], out Annotation detailAnnotation) 
                                && detailAnnotation.type == AnnotationType.Detail)
                            {
                                i++; i++;
                                // Gate detail evaluation on result to allow safe use of criteria-checked values in detail string.
                                script.AppendLine("    if result then");
                                script.AppendLine($"        detail = {criteria[i]}");
                                script.AppendLine("    end");
                            }

                            script.AppendLine($"    return result, '{annotation.value}', detail");
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            CriteriaFunctions.Add(GetUniqueDescription(functionName, annotation.value), LuaState[functionName] as LuaFunction);
                            script.Clear();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string originalScript = script.ToString().Trim();

                originalScript = originalScript.Remove(originalScript.LastIndexOf(Environment.NewLine));
                originalScript = originalScript[(originalScript.IndexOf(Environment.NewLine) + Environment.NewLine.Length)..];
                originalScript = originalScript.Replace('\t', ' ');

                StringBuilder errorDetail = new();
                errorDetail.AppendLine("Error Reading Custom Criteria File:")
                    .AppendLine(originalScript)
                    .AppendLine("To correct this problem, make changes to the Lua source file, save it and either re-run read-all or scan another body. It will be automatically reloaded."); ErrorLogger(e, errorDetail.ToString());
                CriteriaFunctions.Clear(); // Don't use partial parse.
                throw new CriteriaLoadException(e.Message, originalScript);
            }
        }

        public List<(string, string, bool)> CheckInterest(Scan scan, Dictionary<ulong, Dictionary<int, Scan>> scanHistory, Dictionary<ulong, Dictionary<int, FSSBodySignals>> signalHistory, ExplorerSettings settings)
        {
            List<(string, string, bool)> results = new();
            ScanCount++;

            foreach (var criteriaFunction in CriteriaFunctions)
            {
                // Skip criteria which have previously thrown an error. We can't remove them from the dictionary while iterating it. 
                if (CriteriaWithErrors.ContainsKey(criteriaFunction.Key)) continue;

                var scanList = scanHistory[scan.SystemAddress].Values.ToList();

                int bioSignals;
                int geoSignals;

                if (signalHistory.ContainsKey(scan.SystemAddress) && signalHistory[scan.SystemAddress].ContainsKey(scan.BodyID))
                {
                    bioSignals = signalHistory[scan.SystemAddress][scan.BodyID].Signals.Where(s => s.Type == "$SAA_SignalType_Biological;").Select(s => s.Count).FirstOrDefault();
                    geoSignals = signalHistory[scan.SystemAddress][scan.BodyID].Signals.Where(s => s.Type == "$SAA_SignalType_Geological;").Select(s => s.Count).FirstOrDefault();
                }
                else
                {
                    bioSignals = 0;
                    geoSignals = 0;
                }
                    

                List<Parent> parents;

                if (scan.Parent != null)
                {
                    parents = new();
                    foreach (var parent in scan.Parent)
                    {
                        var parentScan = scanList.Where(s => s.BodyID == parent.Body);

                        parents.Add(new Parent() 
                        { 
                            ParentType = parent.ParentType.ToString(), 
                            Body = parent.Body, 
                            Scan = parentScan.Any() ? parentScan.First() : null
                        });
                    }
                }
                else
                {
                    parents = null;
                }

                try
                {
                    var result = criteriaFunction.Value.Call(scan, parents, scanList, bioSignals, geoSignals);
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
                    errorDetail.AppendLine($"while processing custom criteria '{criteriaFunction.Key}' on scan:")
                        .AppendLine(scan.Json)
                        .AppendLine("To correct this problem, make changes to the Lua source file, save it and either re-run read-all or scan another body. It will be automatically reloaded.");
                    ErrorLogger(e, errorDetail.ToString());
                    CriteriaWithErrors.Add(criteriaFunction.Key, e.Message + Environment.NewLine + errorDetail.ToString());
                }
            }

            // Remove any erroring criteria. They will be repopulated next time the file is parsed.
            if (CriteriaWithErrors.Count > 0)
            {
                foreach (var criteriaKey in CriteriaWithErrors.Keys)
                {
                    if (CriteriaFunctions.ContainsKey(criteriaKey)) CriteriaFunctions.Remove(criteriaKey);
                }
            }

            if (ScanCount > 99)
            {
                ScanCount = 0;
                LuaGC();
            }

            return results;
        }

        public void CustomDiscovery(FSSDiscoveryScan scan) 
        {
            if (hasDiscoveryFunc)
            {
                StoreTimeString(scan);
                DiscoveryFunction.Call(scan);
            }
        }
        
        public void CustomAllBodies(FSSAllBodiesFound allBodies)
        {
            if (hasAllBodiesFunc)
            {
                StoreTimeString(allBodies);
                AllBodiesFunction.Call(allBodies);
            }
                
        }

        public void CustomJump(FSDJump jump)
        {
            if (hasJumpFunc)
            {
                StoreTimeString(jump);
                JumpFunction.Call(jump);
            }
        }

        public void CustomSignals(SAASignalsFound signalsFound)
        {
            if (hasBodySignalsFunc)
            {
                StoreTimeString(signalsFound);
                BodySignalsFunction.Call(signalsFound);
            }
                
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
                string annotationRaw = line.Replace("::", string.Empty).Replace("---@", string.Empty);

                switch (annotationRaw.Split(' ')[0].Split('=')[0].ToLower()) // Gross, but handles both formats
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
                        string debugLabel;
                        if (annotationRaw.ToLower().StartsWith("criteria="))
                        {
                            debugLabel = annotationRaw[9..];
                        }
                        else if (annotationRaw.Contains(' '))
                        {
                            debugLabel = string.Join(' ', annotationRaw.Split(' ')[1..]);
                        }
                        else
                        {
                            debugLabel = string.Empty;
                        }
                        annotation = new() { type = AnnotationType.Complex, value = debugLabel };
                        return true;
                    case "allbodies":
                        annotation = new() { type = AnnotationType.AllBodies, value = string.Empty };
                        return true;
                    case "jump":
                        annotation = new() { type = AnnotationType.Jump, value = string.Empty };
                        return true;
                    case "bodysignals":
                        annotation = new() { type = AnnotationType.BodySignals, value = string.Empty };
                        return true;
                    case "discovery":
                        annotation = new() { type = AnnotationType.Discovery, value = string.Empty };
                        return true;
                    default:
                        string simpleDescription;
                        if (annotationRaw.ToLower().StartsWith("simple "))
                        {
                            simpleDescription = annotationRaw[7..];
                        }
                        else
                        {
                            simpleDescription = annotationRaw;
                        }
                        annotation = new() { type = AnnotationType.Simple, value = simpleDescription };
                        return true;
                }
            }

            annotation = new() { type = AnnotationType.None, value = string.Empty };
            return false;
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
            Discovery
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

        internal class Parent
        {
            public string ParentType;
            public int Body;
            public Scan Scan;
        }

    }
}
