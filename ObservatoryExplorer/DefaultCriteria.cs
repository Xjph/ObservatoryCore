﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Explorer
{
    internal static class DefaultCriteria
    {
        public static List<(string Description, string Detail)> CheckInterest(Scan scan, Dictionary<ulong, Dictionary<int, Scan>> scanHistory, Dictionary<ulong, Dictionary<int, FSSBodySignals>> signalHistory, ExplorerSettings settings)
        {
            List<(string, string)> results = new();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            bool isRing = scan.BodyName.Contains("Ring");

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

            #region Parent Relative Checks

            if (scan.SystemAddress != 0 && scan.SemiMajorAxis != 0 &&
                scanHistory[scan.SystemAddress].ContainsKey(scan.Parent[0].Body))
            {
                Scan parent = scanHistory[scan.SystemAddress][scan.Parent[0].Body];

                if (settings.CloseOrbit && !isRing && parent.Radius * 3 > scan.SemiMajorAxis)
                {
                    results.Add("Close Orbit", $"Orbital Radius: {scan.SemiMajorAxis / 1000:N0}km, Parent Radius: {parent.Radius / 1000:N0}km");
                }

                if (settings.ShepherdMoon && !isRing && parent.Rings?.Last().OuterRad > scan.SemiMajorAxis && !parent.Rings.Last().Name.Contains("Belt"))
                {
                    results.Add("Shepherd Moon", $"Orbit: {scan.SemiMajorAxis / 1000:N0}km, Ring Radius: {parent.Rings.Last().OuterRad / 1000:N0}km");
                }

                if (settings.CloseRing && parent.Rings?.Count > 0)
                {
                    foreach (var ring in parent.Rings)
                    {
                        var separation = Math.Min(Math.Abs(scan.SemiMajorAxis - ring.OuterRad), Math.Abs(ring.InnerRad - scan.SemiMajorAxis));
                        if (separation < scan.Radius * 10)
                        {
                            results.Add("Close Ring Proximity", $"Orbit: {scan.SemiMajorAxis / 1000:N0}km, Radius: {scan.Radius / 1000:N0}km, Distance from ring: {separation / 1000:N0}km");
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
                        results.Add("Wide Ring", $"Width: {ringWidth / 299792458:N2}Ls / {ringWidth / 1000:N0}km, Parent Radius: {scan.Radius / 1000:N0}km");
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

            if (settings.FastRotation && scan.RotationPeriod != 0 && !scan.TidalLock && Math.Abs(scan.RotationPeriod) < 28800 && !isRing)
            {
                results.Add("Non-locked Body with Fast Rotation", $"Period: {scan.RotationPeriod/3600:N1} hours");
            }

            if (settings.FastOrbit && scan.OrbitalPeriod != 0 && Math.Abs(scan.OrbitalPeriod) < 28800 && !isRing)
            {
                results.Add("Fast Orbit", $"Orbital Period: {Math.Abs(scan.OrbitalPeriod / 3600):N1} hours");
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

                boostMaterials.RemoveMatchedMaterials(scan);

                if (boostMaterials.Count == 1)
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

                var systemBodies = scanHistory[scan.SystemAddress];

                bool notifyGreen = false;

                foreach (var body in systemBodies.Values)
                {
                    if (settings.GreenSystem && body.Materials != null)
                    {
                        if (!boostMaterials.RemoveMatchedMaterials(body) && boostMaterials.Count == 0)
                        {
                            //If the list has been emptied but we're still checking more bodies this notification has already fired and we can abort.
                            notifyGreen = false;
                            break;
                        }

                        if (boostMaterials.Count == 0)
                            notifyGreen = true;
                            
                    }
                }

                if (notifyGreen)
                    results.Add("All Premium Boost Materials In System");
            }

            return results;
        }

        private static bool RemoveMatchedMaterials(this List<string> materials, Scan body)
        {
            foreach (var material in body.Materials)
            {
                var matchedMaterial = materials.Find(mat => mat.Equals(material.Name, StringComparison.OrdinalIgnoreCase));
                if (matchedMaterial != null)
                {
                    materials.Remove(matchedMaterial);
                }
            }
            return false;
        }

        private static void Add(this List<(string, string)> results, string description, string detail = "")
        {
            results.Add((description, detail));
        }
    }
}
