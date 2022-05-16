using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Explorer
{
    public class ExplorerSettings
    {
        public ExplorerSettings()
        {
            CustomCriteriaFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}{System.IO.Path.DirectorySeparatorChar}ObservatoryCriteria.lua";
        }

        [SettingDisplayName("Landable & Terraformable")]
        public bool LandableTerraformable { get; set; }

        [SettingDisplayName("Landable w/ Atmosphere")]
        public bool LandableAtmosphere { get; set; }

        [SettingDisplayName("Landable High-g")]
        public bool LandableHighG { get; set; }

        [SettingDisplayName("Landable Large")]
        public bool LandableLarge { get; set; }

        [SettingDisplayName("Close Orbit")]
        public bool CloseOrbit { get; set; }

        [SettingDisplayName("Shepherd Moon")]
        public bool ShepherdMoon { get; set; }

        [SettingDisplayName("Wide Ring")]
        public bool WideRing { get; set; }

        [SettingDisplayName("Close Binary")]
        public bool CloseBinary { get; set; }

        [SettingDisplayName("Colliding Binary")]
        public bool CollidingBinary { get; set; }

        [SettingDisplayName("Close Ring Proximity")]
        public bool CloseRing { get; set; }

        [SettingDisplayName("Codex Discoveries")]
        public bool Codex { get; set; }

        [SettingDisplayName("Uncommon Secondary Star")]
        public bool UncommonSecondary { get; set; }

        [SettingDisplayName("Landable w/ Ring")]
        public bool LandableRing { get; set; }

        [SettingDisplayName("Nested Moon")]
        public bool Nested { get; set; }

        [SettingDisplayName("Small Object")]
        public bool SmallObject { get; set; }

        [SettingDisplayName("Fast Rotation")]
        public bool FastRotation { get; set; }

        [SettingDisplayName("Fast Orbit")]
        public bool FastOrbit { get; set; }

        [SettingDisplayName("High Eccentricity")]
        public bool HighEccentricity { get; set; }

        [SettingDisplayName("Diverse Life")]
        public bool DiverseLife { get; set; }

        [SettingDisplayName("Good FSD Injection")]
        public bool GoodFSDBody { get; set; }

        [SettingDisplayName("All FSD Mats In System")]
        public bool GreenSystem { get; set; }

        [SettingDisplayName("All Surface Mats In System")]
        public bool GoldSystem { get; set; }

        [SettingDisplayName("High-Value Body")]
        public bool HighValueMappable { get; set; }

        [SettingDisplayName("Enable Custom Criteria")]
        public bool EnableCustomCriteria { get; set; }

        [SettingDisplayName("Custom Criteria File")]
        [System.Text.Json.Serialization.JsonIgnore]
        public System.IO.FileInfo CustomCriteria {get => new System.IO.FileInfo(CustomCriteriaFile); set => CustomCriteriaFile = value.FullName;}

        [SettingIgnore]
        public string CustomCriteriaFile { get; set; }
    }
}
