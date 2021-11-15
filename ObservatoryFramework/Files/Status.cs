using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files
{
    /// <summary>
    /// Elite Dangerous status.json file.
    /// </summary>
    public class Status : Journal.JournalBase
    {
        /// <summary>
        /// Set of flags representing current player state.
        /// </summary>
        public StatusFlags Flags { get; init; }
        /// <summary>
        /// Additional set of flags representing current player state.
        /// Added in later versions of Elite Dangerous.
        /// </summary>
        public StatusFlags2 Flags2 { get; init; }
        /// <summary>
        /// Current allocation of power distribution (pips) between SYS, ENG, and WEP, in "half pip" increments.
        /// </summary>
        [JsonConverter(typeof(PipConverter))]
        public (int Sys, int Eng, int Wep) Pips { get; init; }
        /// <summary>
        /// Currently selected fire group.
        /// </summary>
        public int Firegroup { get; init; }
        /// <summary>
        /// UI component currently focused by the player.
        /// </summary>
        public FocusStatus GuiFocus { get; init; }
        /// <summary>
        /// Fuel remaining in the current ship.
        /// </summary>
        public FuelType Fuel { get; init; }
        /// <summary>
        /// Amount of cargo currently carried.
        /// </summary>
        public float Cargo { get; init; }
        /// <summary>
        /// Legal status in the current jurisdiction.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LegalStatus LegalState { get; init; }
        /// <summary>
        /// <para>Current altitude.</para>
        /// <para>Check if RadialAltitude is set in StatusFlags to determine if altitude is based on planetary radius (set) or raycast to ground (unset).</para>
        /// </summary>
        public int Altitude { get; init; }
        /// <summary>
        /// Latitude of current surface location.
        /// </summary>
        public double Latitude { get; init; }
        /// <summary>
        /// Longitude of current surface location.
        /// </summary>
        public double Longitude { get; init; }
        /// <summary>
        /// Current heading for surface direction.
        /// </summary>
        public int Heading { get; init; }
        /// <summary>
        /// Body name of current location.
        /// </summary>
        public string BodyName { get; init; }
        /// <summary>
        /// Radius of current planet.
        /// </summary>
        public double PlanetRadius { get; init; }
        /// <summary>
        /// Oxygen remaining on foot, range from 0.0 - 1.0.
        /// </summary>
        public float Oxygen { get; init; }
        /// <summary>
        /// Health remaining on foot, range from 0.0 - 1.0.
        /// </summary>
        public float Health { get; init; }
        /// <summary>
        /// Current environmental temperature in K while on foot.
        /// </summary>
        public float Temperature { get; init; }
        /// <summary>
        /// Name of currently selected personal weapon.
        /// </summary>
        public string SelectedWeapon { get; init; }
        /// <summary>
        /// Current strength of gravity while on foot, in g.
        /// </summary>
        public float Gravity { get; init; }
        /// <summary>
        /// Current credit balance of player.
        /// </summary>
        public long Balance { get; init; }
        /// <summary>
        /// Currently set destination.
        /// </summary>
        public Destination Destination { get; init; }
    }
}
