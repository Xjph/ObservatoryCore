namespace Observatory.Framework.Files.Journal
{
    public class Reputation : JournalBase
    {
        public float Empire { get; init; }

        public float Federation { get; init; }

        public float Independent { get; init; }

        public float Alliance { get; init; }

        public string EmpireText()
        {
            return GetReputationText(Empire);
        }

        public string FederationText()
        {
            return GetReputationText(Federation);
        }

        public string IndependentText()
        {
            return GetReputationText(Independent);
        }

        public string AllianceText()
        {
            return GetReputationText(Alliance);
        }

        private string GetReputationText(float rep)
        {
            string text = rep switch
            {
                float r when r > 90 => "allied",
                float r when r > 35 => "friendly",
                float r when r > 4 => "cordial",
                float r when r > -35 => "neutral",
                float r when r > -90 => "unfriendly",
                _ => "hostile",
            };
            return text; 
        }
    }
}
