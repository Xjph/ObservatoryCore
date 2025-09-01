using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierDecommission : JournalBase
    {
        public ulong CarrierID { get; init; }
        public CarrierType CarrierType { get; init; }
        public long ScrapRefund { get; init; }
        public long ScrapTime { get; init; }
        public System.DateTime ScrapTimeUTC
        {
            get 
            {
                return System.DateTimeOffset.FromUnixTimeSeconds(ScrapTime).UtcDateTime; 
            }
        }
    }
}
