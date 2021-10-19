using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class RedeemVoucher : JournalBase
    {
        [JsonConverter(typeof(VoucherTypeConverter))]
        public VoucherType Type { get; init; }
        public long Amount { get; init; }
        public string Faction { get; init; }
        public float BrokerPercentage { get; init; }
        public ImmutableList<VoucherFaction> Factions { get; init; }

    }
}
