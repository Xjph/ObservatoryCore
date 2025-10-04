using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Files.Converters;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Journal "Scan" event generated when directly FSS scanning, from automatic proximity scans, or nav beacon data.
    /// </summary>
    public class Scan : ScanBaryCentre
    {
        /// <summary>
        /// Type of scan which generated the event. Possible options include "Detailed", "AutoScan", and "NavBeaconDetail" (non-exhaustive).
        /// </summary>
        public string ScanType { get; init; }
        /// <summary>
        /// Name of scanned body.
        /// </summary>
        public string BodyName { get; init; }
        /// <summary>
        /// List which reflects Frontier's JSON structure for the "Parents" object. Use of Parent property is recommended instead.
        /// </summary>
        public ImmutableList<Parent> Parents { 
            get => _Parents; 
            init
            {
                _Parents = value;
                var ParentList = new System.Collections.Generic.List<(ParentType ParentType, int Body)>();
                if (value != null)
                {
                    foreach (var parent in value)
                    {
                        if (parent.Null != null)
                        {
                            ParentList.Add((ParentType.Null, parent.Null.GetValueOrDefault(0)));
                        }
                        else if (parent.Planet != null)
                        {
                            ParentList.Add((ParentType.Planet, parent.Planet.GetValueOrDefault(0)));
                        }
                        else if (parent.Star != null)
                        {
                            ParentList.Add((ParentType.Star, parent.Star.GetValueOrDefault(0)));
                        }
                    }
                }
                Parent = ParentList.ToImmutableList();
            }
        }
        /// <summary>
        /// "Parents" object rearranged into more intuitive structure for ease of use.
        /// </summary>
        [JsonIgnore]
        public ImmutableList<(ParentType ParentType, int Body)> Parent { get; init; }
        private ImmutableList<Parent> _Parents;
        /// <summary>
        /// Body distance from system arrival point in light-seconds.
        /// </summary>
        public double DistanceFromArrivalLS { get; init; }
        /// <summary>
        /// Indicates if body is tidally locked to another body (parent, child, or binary partner).
        /// </summary>
        public bool TidalLock { get; init; }
        /// <summary>
        /// Whether the planet can be or has been terraformed. Options include "Terraformable", "Terraformed", or "" (non-terraformable or naturally earth-like).
        /// </summary>
        public string TerraformState { get; init; }
        /// <summary>
        /// Class of planet. Consult your preferred source of journal documentation for all possible values.
        /// </summary>
        public string PlanetClass { get; init; }
        /// <summary>
        /// Descriptive string for body atmosphere, e.g. "hot thick sulfur dioxide atmosphere".
        /// </summary>
        public string Atmosphere { get; init; }
        /// <summary>
        /// Simple string indicating dominant atmosphere type, e.g. "SulfurDioxide".
        /// </summary>
        public string AtmosphereType { get; init; }
        /// <summary>
        /// List containing full breakdown of atmospheric components and their relative percentages.
        /// </summary>
        [JsonConverter(typeof(MaterialCompositionConverter))]
        public ImmutableList<MaterialComposition> AtmosphereComposition { get; init; }
        /// <summary>
        /// Descriptive string for type of volcanism present, or an empty string for none, e.g. "major silicate vapour geysers volcanism".
        /// </summary>
        public string Volcanism { get; init; }
        /// <summary>
        /// Mass of body in multiples of Earth's mass (5.972e24 kg).
        /// </summary>
        public float MassEM { get; init; }
        /// <summary>
        /// Radius of body in metres.
        /// </summary>
        public float Radius { get; init; }
        /// <summary>
        /// Surface gravity in m/s².
        /// </summary>
        public float SurfaceGravity { get; init; }
        /// <summary>
        /// Average surface temperature in Kelvin.
        /// </summary>
        public float SurfaceTemperature { get; init; }
        /// <summary>
        /// Average surface pressure in Pascals.
        /// </summary>
        public float SurfacePressure { get; init; }
        /// <summary>
        /// Whether the body in landable in the player's current version of Elite Dangerous.
        /// </summary>
        public bool Landable { get; init; }
        /// <summary>
        /// List containing full breakdown of prospectable surface materials and their relative percentages.
        /// </summary>
        [JsonConverter(typeof(MaterialCompositionConverter))]
        public ImmutableList<MaterialComposition> Materials { get; init; }
        /// <summary>
        /// Overall composition of body, expressed as percentages of ice, rock, and metal.
        /// </summary>
        public Composition Composition { get; init; }
        /// <summary>
        /// Rotation period of body in seconds.
        /// </summary>
        public float RotationPeriod { get; init; }
        /// <summary>
        /// Axial tilt of body in radians.
        /// </summary>
        public float AxialTilt { get; init; }
        /// <summary>
        /// List of all planetary or stellar ring systems around the body.
        /// </summary>
        public ImmutableList<Ring> Rings { get; init; }
        /// <summary>
        /// Description of the minable material abundance.<br/>Possible values inclue "PristineResources", "MajorResources", "CommonResources", "LowResources", and "DepletedResources". 
        /// </summary>
        public string ReserveLevel { get; init; }
        /// <summary>
        /// Type of star. Consult your preferred source of journal documentation for all possible values.
        /// </summary>
        public string StarType { get; init; }
        /// <summary>
        /// Subclass of star. Consult your preferred source of journal documentation for all possible values.
        /// </summary>
        public int Subclass { get; init; }
        /// <summary>
        /// Mass of star in multiples of The Sun's mass (1.989e30 kg).
        /// </summary>
        public float StellarMass { get; init; }
        /// <summary>
        /// Absolute magnitude of star.
        /// </summary>
        public float AbsoluteMagnitude { get; init; }
        /// <summary>
        /// Age of body in millions of years.
        /// </summary>
        public int Age_MY { get; init; }
        /// <summary>
        /// Yerkes luminosity class of star.
        /// </summary>
        public string Luminosity { get; init; }
        /// <summary>
        /// Whether the body has been previously discovered by a player.
        /// </summary>
        public bool WasDiscovered { get; init; }
        /// <summary>
        /// Whether the body has been previously mapped by a player.
        /// </summary>
        public bool WasMapped { get; init; }
        /// <summary>
        /// Whether the body has been previously walked on by a player.
        /// </summary>
        public bool WasFootfalled { get; init; }
    }
}
