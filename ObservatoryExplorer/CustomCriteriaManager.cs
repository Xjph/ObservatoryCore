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
        private List<LuaFunction> CriteriaFunctions;
        
        public CustomCriteriaManager()
        {
            CriteriaFunctions = new();
        }

        public void RefreshCriteria(string criteriaPath)
        {
            LuaState = new();
            LuaState.State.Encoding = Encoding.UTF8;
            LuaState.LoadCLRPackage();
            
            #region Iterators

            //Materials and AtmosphereComposition
            LuaState.DoString(@"
                function materials (material_list)
                    local i = 0
                    local count = material_list.Count
                    return function ()
                        i = i + 1
                        if i <= count then
                            return { name = material_list[i - 1].Name, percent = material_list[i - 1].Percent }
                        end
                    end
                end");

            //Rings
            LuaState.DoString(@"
                function rings (ring_list)
                    local i = 0
                    local count = ring_list.Count
                    return function ()
                        i = i + 1
                        if i <= count then
                            local ring = ring_list[i - 1]
                            return { name = ring.Name, ringclass = ring.RingClass, massmt = ring.MassMT, innerrad = ring.InnerRad, outerrad = ring.OuterRad }
                        end
                    end
                end");

            //Bodies in system
            LuaState.DoString(@"
                function bodies (system_list)
                    local i = 0
                    local count = system_list.Count
                    return function ()
                        i = i + 1
                        if i <= count then
                            return system_list[i - 1]
                        end
                    end
                end");

            //Parent bodies
            LuaState.DoString(@"
                function allparents (parent_list)
                    local i = 0
                    local count
                    if parent_list then count = parent_list.Count else count = 0 end
                    return function ()
                        i = i + 1
                        if i <= count then
                            return { parenttype = parent_list[i - 1].ParentType, body = parent_list[i - 1].Body, scan = parent_list[i - 1].Scan }
                        end
                    end
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
                        if (scriptDescription.ToLower() == "criteria")
                        {
                            string functionName = $"Criteria{i}";
                            script.AppendLine($"function {functionName} (scan, parents, system, biosignals, geosignals)");
                            i++;
                            do
                            {
                                script.AppendLine(criteria[i]);
                                i++;
                            } while (!criteria[i].Trim().ToLower().StartsWith("::end::"));
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            CriteriaFunctions.Add(LuaState[functionName] as LuaFunction);
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

                            if (criteria.Length > i + 1 && criteria[i + 1].Trim().ToLower() == "::detail::")
                            {
                                i++; i++;
                                script.AppendLine($"    local detail = {criteria[i]}");
                            }
                            else
                            {
                                script.AppendLine("    local detail = ''");
                            }

                            script.AppendLine($"    return result, '{scriptDescription}', detail");
                            script.AppendLine("end");

                            LuaState.DoString(script.ToString());
                            CriteriaFunctions.Add(LuaState[functionName] as LuaFunction);
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
                
                throw new CriteriaLoadException(e.Message, originalScript.Replace('\t', ' '));
            }
        }

        public List<(string, string, bool)> CheckInterest(Scan scan, Dictionary<ulong, Dictionary<int, Scan>> scanHistory, Dictionary<ulong, Dictionary<int, FSSBodySignals>> signalHistory, ExplorerSettings settings)
        {
            List<(string, string, bool)> results = new();
           
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
                    var result = criteriaFunction.Call(scan, parents, scanList, bioSignals, geoSignals);
                    if (result.Length > 0 && ((bool?)result[0]).GetValueOrDefault(false))
                    {
                        results.Add((result[1].ToString(), result[2].ToString(), false));
                    }
                }
                catch (NLua.Exceptions.LuaScriptException e)
                {
                    settings.EnableCustomCriteria = false;
                    results.Add((e.Message, scan.Json, false));
                    break;
                }
            }

            return results;
        }

        internal class Parent
        {
            public string ParentType;
            public int Body;
            public Scan Scan;
        }

    }
}
