using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class FleetCarrier
    {
        [JsonPropertyName("FLEETCARRIER_EXPORT_TOTAL")]
        public int ExportTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_IMPORT_TOTAL")]
        public int ImportTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_TRADEPROFIT_TOTAL")]
        public long TradeprofitTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_TRADESPEND_TOTAL")]
        public long TradespendTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_STOLENPROFIT_TOTAL")]
        public int StolenprofitTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_STOLENSPEND_TOTAL")]
        public int StolenspendTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_DISTANCE_TRAVELLED")]
        [JsonConverter(typeof(FleetCarrierTravelConverter))]
        public float DistanceTravelled { get; init; }

        [JsonPropertyName("FLEETCARRIER_TOTAL_JUMPS")]
        public int TotalJumps { get; init; }

        [JsonPropertyName("FLEETCARRIER_SHIPYARD_SOLD")]
        public int ShipyardSold { get; init; }

        [JsonPropertyName("FLEETCARRIER_SHIPYARD_PROFIT")]
        public long ShipyardProfit { get; init; }

        [JsonPropertyName("FLEETCARRIER_OUTFITTING_SOLD")]
        public int OutfittingSold { get; init; }

        [JsonPropertyName("FLEETCARRIER_OUTFITTING_PROFIT")]
        public long OutfittingProfit { get; init; }

        [JsonPropertyName("FLEETCARRIER_REARM_TOTAL")]
        public int RearmTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_REFUEL_TOTAL")]
        public int RefuelTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_REFUEL_PROFIT")]
        public long RefuelProfit { get; init; }

        [JsonPropertyName("FLEETCARRIER_REPAIRS_TOTAL")]
        public int RepairsTotal { get; init; }

        [JsonPropertyName("FLEETCARRIER_VOUCHERS_REDEEMED")]
        public int VouchersRedeemed { get; init; }

        [JsonPropertyName("FLEETCARRIER_VOUCHERS_PROFIT")]
        public long VouchersProfit { get; init; }
    }
}
