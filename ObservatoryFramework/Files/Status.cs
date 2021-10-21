using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files
{
    public class Status : Journal.JournalBase
    {
        public StatusFlags Flags { get; init; }
        public StatusFlags2 Flags2 { get; init; }
        [JsonConverter(typeof(PipConverter))]
        public (int Sys, int Eng, int Wep) Pips { get; init; }
        public int Firegroup { get; init; }
        public FocusStatus GuiFocus { get; init; }
        public FuelType Fuel { get; init; }
        public float Cargo { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LegalStatus LegalState { get; init; }
        public int Altitude { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int Heading { get; init; }
        public string BodyName { get; init; }
        public double PlanetRadius { get; init; }
        public float Oxygen { get; init; }
        public float Health { get; init; }
        public float Temperature { get; init; }
        public string SelectedWeapon { get; init; }
        public float Gravity { get; init; }
        public long Balance { get; init; }
        public Destination Destination { get; init; }
    }
}
