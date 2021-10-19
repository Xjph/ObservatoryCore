namespace Observatory.Framework.Files.ParameterTypes
{
    public class CarrierFinance
    {
        public long CarrierBalance { get; init; }
        public long ReserveBalance { get; init; }
        public long AvailableBalance { get; init; }
        public int ReservePercent { get; init; }
        public int TaxRate { get; init; }
    }
}
