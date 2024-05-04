using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Explorer
{
    internal static class DefaultCriteria
    {
        public static List<(string Description, string Detail, bool SystemWide)> CheckInterest(Scan scan, Dictionary<ulong, Dictionary<int, Scan>> scanHistory, Dictionary<ulong, Dictionary<int, FSSBodySignals>> signalHistory, ExplorerSettings settings)
        {
            List<(string, string, bool)> results = new();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            bool isRing = scan.BodyName.Contains("Ring");

#if DEBUG
            // results.Add("Test Scan Event", "Test Detail");
#endif

            #region Landable Checks
            if (scan.Landable)
            {
                if (settings.LandableTerraformable && scan.TerraformState?.Length > 0)
                {
                    results.Add($"Landable and {scan.TerraformState}");
                }

                if (settings.LandableRing && scan.Rings?.Count > 0)
                {
                    results.Add("Ringed Landable Body");
                }

                if (settings.LandableAtmosphere && scan.Atmosphere.Length > 0)
                {
                    results.Add("Landable with Atmosphere", textInfo.ToTitleCase(scan.Atmosphere));
                }

                if (settings.LandableHighG && scan.SurfaceGravity > 29.4)
                {
                    results.Add("Landable with High Gravity", $"Surface gravity: {scan.SurfaceGravity / 9.81:0.00}g");
                }

                if (settings.LandableLarge && scan.Radius > 18000000)
                {
                    results.Add("Landable Large Planet", $"Radius: {scan.Radius / 1000:0}km");
                }
            }
            #endregion

            #region Value Checks
            if (settings.HighValueMappable)
            {
                IList<string> HighValueNonTerraformablePlanetClasses = new string[] {
                    "Earthlike body",
                    "Ammonia world",
                    "Water world",
                };

                if (HighValueNonTerraformablePlanetClasses.Contains(scan.PlanetClass) || scan.TerraformState?.Length > 0)
                {
                    var info = new System.Text.StringBuilder();
                                        
                    if (!scan.WasMapped)
                    {
                        if (!scan.WasDiscovered)
                            info.Append("Undiscovered ");
                        else
                            info.Append("Unmapped ");
                    }

                    if (scan.TerraformState?.Length > 0)
                        info.Append("Terraformable ");

                    results.Add("High-Value Body", $"{info.ToString()}{textInfo.ToTitleCase(scan.PlanetClass)}, {scan.DistanceFromArrivalLS:N0}Ls");
                }
            }
            #endregion
            
            #region Parent Relative Checks

            if (scan.SystemAddress != 0 && scan.SemiMajorAxis != 0 &&
                scanHistory[scan.SystemAddress].ContainsKey(scan.Parent[0].Body))
            {
                Scan parent = scanHistory[scan.SystemAddress][scan.Parent[0].Body];

                if (settings.CloseOrbit && !isRing && parent.Radius * 3 > scan.SemiMajorAxis)
                {
                    results.Add("Close Orbit", $"Orbital Radius: {scan.SemiMajorAxis / 1000:N0}km, Parent Radius: {parent.Radius / 1000:N0}km");
                }

                if (settings.ShepherdMoon && !isRing && parent.Rings?.Any() == true && parent.Rings.Last().OuterRad > scan.SemiMajorAxis && !parent.Rings.Last().Name.Contains("Belt"))
                {
                    results.Add("Shepherd Moon", $"Orbit: {scan.SemiMajorAxis / 1000:N0}km, Ring Radius: {parent.Rings.Last().OuterRad / 1000:N0}km");
                }

                if (settings.CloseRing && parent.Rings?.Count > 0)
                {
                    foreach (var ring in parent.Rings)
                    {
                        var separation = Math.Min(Math.Abs(scan.SemiMajorAxis - ring.OuterRad), Math.Abs(ring.InnerRad - scan.SemiMajorAxis)) - scan.Radius;
                        if (separation < scan.Radius * 10)
                        {
                            var ringTypeName = ring.Name.Contains("Belt") ? "Belt" : "Ring";
                            var isLandable = scan.Landable ? ", Landable" : "";
                            results.Add($"Close {ringTypeName} Proximity",
                                $"Orbit: {scan.SemiMajorAxis / 1000:N0}km, Radius: {scan.Radius / 1000:N0}km, Distance from {ringTypeName.ToLower()}: {separation / 1000:N0}km{isLandable}");
                        }
                    }
                }
            }

            #endregion

            if (settings.DiverseLife && signalHistory.ContainsKey(scan.SystemAddress) && signalHistory[scan.SystemAddress].ContainsKey(scan.BodyID))
            {
                var bioSignals = signalHistory[scan.SystemAddress][scan.BodyID].Signals.Where(s => s.Type == "$SAA_SignalType_Biological;");

                if (bioSignals.Count() > 0 && bioSignals.First().Count > 7)
                {
                    results.Add("Diverse Life", $"Biological Signals: {bioSignals.First().Count}");
                }
            }

            if (settings.WideRing && scan.Rings?.Count > 0)
            {
                foreach (var ring in scan.Rings.Where(r => !r.Name.Contains("Belt")))
                {
                    var ringWidth = ring.OuterRad - ring.InnerRad;
                    if (ringWidth > scan.Radius * 5)
                    {
                        var ringName = ring.Name.Replace(scan.BodyName, "").Trim();
                        results.Add("Wide Ring", $"{ringName}: Width: {ringWidth / 299792458:N2}Ls / {ringWidth / 1000:N0}km, Parent Radius: {scan.Radius / 1000:N0}km");
                    }
                }
            }

            if (settings.SmallObject && scan.StarType == null && scan.PlanetClass != null && scan.PlanetClass != "Barycentre" && scan.Radius < 300000)
            {
                results.Add("Small Object", $"Radius: {scan.Radius / 1000:N0}km");
            }

            if (settings.HighEccentricity && scan.Eccentricity > 0.9)
            {
                results.Add("Highly Eccentric Orbit", $"Eccentricity: {Math.Round(scan.Eccentricity, 4)}");
            }

            if (settings.Nested && !isRing && scan.Parent?.Count > 1 && scan.Parent[0].ParentType == ParentType.Planet && scan.Parent[1].ParentType == ParentType.Planet)
            {
                results.Add("Nested Moon");
            }

            if (settings.FastRotation && scan.RotationPeriod != 0 && !scan.TidalLock && Math.Abs(scan.RotationPeriod) < 28800 && !isRing && string.IsNullOrEmpty(scan.StarType))
            {
                results.Add("Non-locked Body with Fast Rotation", $"Period: {scan.RotationPeriod/3600:N1} hours");
            }

            if (settings.FastOrbit && scan.OrbitalPeriod != 0 && Math.Abs(scan.OrbitalPeriod) < 28800 && !isRing)
            {
                results.Add("Fast Orbit", $"Orbital Period: {Math.Abs(scan.OrbitalPeriod / 3600):N1} hours");
            }

            // Close binary pair
            if ((settings.CloseBinary || settings.CollidingBinary) && scan.Parent?[0].ParentType == ParentType.Null && scan.Radius / scan.SemiMajorAxis > 0.4)
            {
                var binaryPartner = scanHistory[scan.SystemAddress].Where(priorScan => priorScan.Value.Parent?[0].Body == scan.Parent?[0].Body && scan.BodyID != priorScan.Key);

                if (binaryPartner.Count() == 1)
                {
                    if (binaryPartner.First().Value.Radius / binaryPartner.First().Value.SemiMajorAxis > 0.4)
                    {
                        if (settings.CollidingBinary && binaryPartner.First().Value.Radius + scan.Radius >= binaryPartner.First().Value.SemiMajorAxis * (1 - binaryPartner.First().Value.Eccentricity) + scan.SemiMajorAxis * (1 - scan.Eccentricity))
                        {
                            results.Add("COLLIDING binary", $"Orbit: {Math.Truncate((double)scan.SemiMajorAxis / 1000):N0}km, Radius: {Math.Truncate((double)scan.Radius / 1000):N0}km, Partner: {binaryPartner.First().Value.BodyName}");
                        }
                        else if (settings.CloseBinary)
                        {
                            results.Add("Close binary relative to body size", $"Orbit: {Math.Truncate((double)scan.SemiMajorAxis / 1000):N0}km, Radius: {Math.Truncate((double)scan.Radius / 1000):N0}km, Partner: {binaryPartner.First().Value.BodyName}");
                        }
                    }
                }
            }

            if (settings.GoodFSDBody && scan.Landable)
            {
                List<string> boostMaterials = new() 
                { 
                    "Carbon",
                    "Germanium",
                    "Arsenic",
                    "Niobium",
                    "Yttrium",
                    "Polonium"
                };

                if (boostMaterials.RemoveMatchedMaterials(scan) == 1)
                {
                    results.Add("5 of 6 Premium Boost Materials", $"Missing material: {boostMaterials[0]}");
                }
            }

            if ((settings.GreenSystem || settings.GoldSystem) && scan.Materials != null)
            {
                List<string> boostMaterials = new()
                {
                    "Carbon",
                    "Germanium",
                    "Arsenic",
                    "Niobium",
                    "Yttrium",
                    "Polonium"
                };

                List<string> allSurfaceMaterials = new()
                {
                    "Antimony", "Arsenic", "Cadmium", "Carbon",
                    "Chromium", "Germanium", "Iron", "Manganese",
                    "Mercury", "Molybdenum", "Nickel", "Niobium",
                    "Phosphorus", "Polonium", "Ruthenium", "Selenium",
                    "Sulphur", "Technetium", "Tellurium", "Tin",
                    "Tungsten", "Vanadium", "Yttrium", "Zinc",
                    "Zirconium"
                };

                var systemBodies = scanHistory[scan.SystemAddress];

                bool notifyGreen = false;
                bool notifyGold = false;

                foreach (var body in systemBodies.Values)
                {

                    // If we enter either check and the count is already zero then a
                    // previous body in system triggered the check, suppress notification.
                    if (settings.GreenSystem && body.Materials != null)
                    {
                        if (boostMaterials.Count == 0)
                            notifyGreen = false;
                        else if (boostMaterials.RemoveMatchedMaterials(body) == 0)
                            notifyGreen = true;
                    }

                    if (settings.GoldSystem && body.Materials != null)
                    {
                        if (allSurfaceMaterials.Count == 0)
                            notifyGold = false;
                        else if (allSurfaceMaterials.RemoveMatchedMaterials(body) == 0)
                            notifyGold = true;
                    }
                }

                if (notifyGreen)
                    results.Add("All Premium Boost Materials In System", string.Empty, true);

                if (notifyGold)
                    results.Add("All Surface Materials In System", string.Empty, true);
            }

            if (settings.UncommonSecondary && scan.BodyID > 0 && !string.IsNullOrWhiteSpace(scan.StarType) && scan.DistanceFromArrivalLS > 10)
            {
                var excludeTypes = new List<string>() { "O", "B", "A", "F", "G", "K", "M", "L", "T", "Y", "TTS" };
                if (!excludeTypes.Contains(scan.StarType.ToUpper()))
                {
                    results.Add("Uncommon Secondary Star Type", $"{GetUncommonStarTypeName(scan.StarType)}, Distance: {scan.DistanceFromArrivalLS:N0}Ls");
                }
            }

            return results;
        }

        private static string GetUncommonStarTypeName(string starType)
        {
            string name;

            switch (starType.ToLower())
            {
                case "b_bluewhitesupergiant":
                    name = "B Blue-White Supergiant";
                    break;
                case "a_bluewhitesupergiant":
                    name = "A Blue-White Supergiant";
                    break;
                case "f_whitesupergiant":
                    name = "F White Supergiant";
                    break;
                case "g_whitesupergiant":
                    name = "G White Supergiant";
                    break;
                case "k_orangegiant":
                    name = "K Orange Giant";
                    break;
                case "m_redgiant":
                    name = "M Red Giant";
                    break;
                case "m_redsupergiant":
                    name = "M Red Supergiant";
                    break;
                case "aebe":
                    name = "Herbig Ae/Be";
                    break;
                case "w":
                case "wn":
                case "wnc":
                case "wc":
                case "wo":
                    name = "Wolf-Rayet";
                    break;
                case "c":
                case "cs":
                case "cn":
                case "cj":
                case "ch":
                case "chd":
                    name = "Carbon Star";
                    break;
                case "s":
                    name = "S-Type Star";
                    break;
                case "ms":
                    name = "MS-Type Star";
                    break;
                case "d":
                case "da":
                case "dab":
                case "dao":
                case "daz":
                case "dav":
                case "db":
                case "dbz":
                case "dbv":
                case "do":
                case "dov":
                case "dq":
                case "dc":
                case "dcv":
                case "dx":
                    name = "White Dwarf";
                    break;
                case "n":
                    name = "Neutron Star";
                    break;
                case "h":
                    name = "Black Hole";
                    break;
                case "supermassiveblackhole":
                    name = "Supermassive Black Hole";
                    break;
                case "x":
                    name = "Exotic Star";
                    break;
                case "rogueplanet":
                    name = "Rogue Planet";
                    break;
                case "tts":
                case "t":
                    name = "T Tauri Type";
                    break;
                default:
                    name = starType + "-Type Star";
                    break;
            }

            return name;
        }

        /// <summary>
        /// Removes materials from the list if found on the specified body.
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="body"></param>
        /// <returns>Count of materials remaining in list.</returns>
        private static int RemoveMatchedMaterials(this List<string> materials, Scan body)
        {
            foreach (var material in body.Materials)
            {
                var matchedMaterial = materials.Find(mat => mat.Equals(material.Name, StringComparison.OrdinalIgnoreCase));
                if (matchedMaterial != null)
                {
                    materials.Remove(matchedMaterial);
                }
            }
            return materials.Count;
        }

        private static void Add(this List<(string, string, bool)> results, string description, string detail = "", bool systemWide = false)
        {
            results.Add((description, detail, systemWide));
        }
    }
}
