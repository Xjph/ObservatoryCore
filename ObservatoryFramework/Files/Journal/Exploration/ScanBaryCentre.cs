namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Barycentre orbital properties, automatically recorded when any member of a multiple-body orbital arrangement is first scanned.
    /// </summary>
    public class ScanBaryCentre : JournalBase
    {
        /// <summary>
        /// Name of star system containing scanned body.
        /// </summary>
        public string StarSystem { get; init; }
        /// <summary>
        /// 64-bit unique identifier for the current star system. Also known as the system's "ID64".
        /// </summary>
        public ulong SystemAddress { get; init; }
        /// <summary>
        /// Id number of body within a system.
        /// </summary>
        public int BodyID { get; init; }
        /// <summary>
        /// Orbital semi-major axis in metres.<br/>Distance from the body's centre of gravity to the parent's centre of gravity at the most distant point in the orbit.
        /// </summary>
        public float SemiMajorAxis { get; init; }
        /// <summary>
        /// Orbital eccentricity.<br/>0: perfectly circular, 0 &gt; x &gt; 1: eccentric, 1: parabolic (escape) trajectory.<br/>(You should not ever see 1 or 0.)
        /// </summary>
        public float Eccentricity { get; init; }
        /// <summary>
        /// Orbital inclination in degrees.
        /// </summary>
        public float OrbitalInclination { get; init; }
        /// <summary>
        /// Argument of periapsis in degrees.
        /// </summary>
        public float Periapsis { get; init; }
        /// <summary>
        /// Orbital period in seconds.
        /// </summary>
        public float OrbitalPeriod { get; init; }
        /// <summary>
        /// Longitude of the ascending node in degrees.
        /// </summary>
        public float AscendingNode { get; init; }
        /// <summary>
        /// Mean anomaly in degrees.
        /// </summary>
        public float MeanAnomaly { get; init; }

    }
}
