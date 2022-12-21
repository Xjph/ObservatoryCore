using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Observatory.Framework.Files.Journal;
using NLua;
using System.Linq;

namespace Observatory.Explorer
{
    internal class CustomCriteriaManager
    {
        private Lua LuaState;
        private Dictionary<String,LuaFunction> CriteriaFunctions;
        Action<Exception, String> ErrorLogger;
        private uint ScanCount;

        public CustomCriteriaManager(Action<Exception, String> errorLogger)
        {
            ErrorLogger = errorLogger;
            CriteriaFunctions = new();
            ScanCount = 0;
        }

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

            //Rings - has > 0 belts
            LuaState.DoString(@"
                function hasBelts (ring_list)
                    return _hasRingsFiltered(ring_list, 'Belt')
                end");

            //Rings - has > 0 proper rings
            LuaState.DoString(@"
                function hasRings (ring_list)
                    return _hasRingsFiltered(ring_list, 'Ring')
                end");

            LuaState.DoString(@"
                function isStar (scan)
                    return scan.StarType and scan.StarType ~= ''
                end");

            LuaState.DoString(@"
                function isPlanet (scan)
                    return scan.PlanetClass ~= nil
                end");

            LuaState.DoString(@"
                function hasAtmosphere (scan)
                    return scan.AtmosphereComposition ~= nil
                end");

            LuaState.DoString(@"
                function hasLandableAtmosphere (scan)
                    return scan.Landable and scan.AtmosphereComposition ~= nil
                end");

            #endregion

            CriteriaFunctions.Clear();
            var criteria = File.Exists(criteriaPath) ? File.ReadAllLines(criteriaPath) : Array.Empty<string>();
            StringBuilder script = new();

            try
            {
                for (int i = 0; i < criteria.Length; i++)
                {
                    if (criteria[i].Trim().StartsWith("::"))
                    {
                        string scriptDescription = criteria[i].Trim().Replace("::", string.Empty);
                        if (scriptDescription.ToLower() == "criteria" || scriptDescription.ToLower().StartsWith("criteria="))
                        {
                            string functionName = $"Criteria{i}";
                            script.AppendLine($"function {functionName} (scan, parents, system, biosignals, geosignals)");
                            i++;
                            do
                            {
                                if (i >= criteria.Length)
                                    throw new Exception("Unterminated multi-line criteria.\r\nAre you missing an ::End::?");

                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!criteria[i].Trim().ToLower().StartsWith("::end::"));
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            CriteriaFunctions.Add(GetUniqueDescription(functionName, scriptDescription), LuaState[functionName] as LuaFunction);
                            script.Clear();
                        }
                        else if (scriptDescription.ToLower() == "global")
                        {
                            i++;
                            do
                            {
                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!criteria[i].Trim().ToLower().StartsWith("::end::"));
                            LuaState.DoString(script.ToString());
                            script.Clear();
                        }
                        else
                        {
                            i++;

                            string functionName = $"Criteria{i}";

                            script.AppendLine($"function {functionName} (scan, parents, system, biosignals, geosignals)");
                            script.AppendLine($"    local result = {criteria[i]}");
                            script.AppendLine("    local detail = ''");

                            if (criteria.Length > i + 1 && criteria[i + 1].Trim().ToLower() == "::detail::")
                            {
                                i++; i++;
                                // Gate detail evaluation on result to allow safe use of criteria-checked values in detail string.
                                script.AppendLine("    if result then");
                                script.AppendLine($"        detail = {criteria[i]}");
                                script.AppendLine("    end");
                            }

                            script.AppendLine($"    return result, '{scriptDescription}', detail");
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            CriteriaFunctions.Add(GetUniqueDescription(functionName, scriptDescription), LuaState[functionName] as LuaFunction);
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
                    .AppendLine("NOTE: Custom criteria processing has been disabled to prevent further errors.");
                ErrorLogger(e, errorDetail.ToString());
                throw new CriteriaLoadException(e.Message, originalScript);
            }
        }

        public List<(string, string, bool)> CheckInterest(Scan scan, Dictionary<ulong, Dictionary<int, Scan>> scanHistory, Dictionary<ulong, Dictionary<int, FSSBodySignals>> signalHistory, ExplorerSettings settings)
        {
            List<(string, string, bool)> results = new();
            ScanCount++;

            foreach (var criteriaFunction in CriteriaFunctions)
            {
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
                    settings.EnableCustomCriteria = false;
                    results.Add((e.Message, scan.Json, false));

                    StringBuilder errorDetail = new();
                    errorDetail.AppendLine($"while processing custom criteria '{criteriaFunction.Key}' on scan:")
                        .AppendLine(scan.Json)
                        .AppendLine("NOTE: Custom criteria processing has been disabled to prevent further errors.");
                    ErrorLogger(e, errorDetail.ToString());
                    break;
                }
            }

            if (ScanCount > 99)
            {
                ScanCount = 0;
                LuaGC();
            }

            return results;
        }

        private string GetUniqueDescription(string functionName, string scriptDescription)
        {
            string uniqueDescription = functionName;
            if (scriptDescription.ToLower().StartsWith("criteria="))
            {
                uniqueDescription += scriptDescription.Replace("criteria=", "=", StringComparison.CurrentCultureIgnoreCase);
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
