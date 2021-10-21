using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class MissionAccepted : JournalBase
    {
        public string Name { get; init; }
        public string LocalisedName { get; init; }
        public string Faction { get; init; }
        public long MissionID { get; init; }
        [JsonConverter(typeof(MissionEffectConverter))]
        public MissionEffect Influence { get; init; }
        [JsonConverter(typeof(MissionEffectConverter))]
        public MissionEffect Reputation { get; init; }
        public long Reward { get; init; }
        public string Commodity { get; init; }
        public string Commodity_Localised { get; init; }
        public int Count { get; init; }
        public string Donation { get; init; }
        public int Donated { get; init; }
        public string Target { get; init; }
        public string Target_Localised { get; init; }
        public string TargetType { get; init; }
        public string TargetType_Localised { get; init; }
        public string TargetFaction { get; init; }
        public int KillCount { get; init; }
        public string Expiry { get; init; }
        public DateTime ExpiryDateTime
        {
            get
            {
                if (DateTime.TryParseExact(Expiry, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime expiryDateTime))
                {
                    return expiryDateTime;
                }
                else
                {
                    return new DateTime();
                }
            }
        }
        public string DestinationSystem { get; init; }
        public string DestinationStation { get; init; }
        public string NewDestinationSystem { get; init; }
        public string NewDestinationStation { get; init; }
        public int PassengerCount { get; init; }
        public bool PassengerVIPs { get; init; }
        public bool PassengerWanted { get; init; }
        public string PassengerType { get; init; }
        public bool Wing { get; init; }
    }
}
