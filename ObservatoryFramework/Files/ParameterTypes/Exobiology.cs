using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Exobiology
    {
        [JsonPropertyName("First_Logged")]
        public int FirstLogged { get; init; }

        [JsonPropertyName("First_Logged_Profits")]
        public long FirstLoggedProfits { get; init; }

        [JsonPropertyName("Organic_Data")]
        public int OrganicData { get; init; }

        [JsonPropertyName("Organic_Data_Profits")]
        public long OrganicDataProfits { get; init; }

        [JsonPropertyName("Organic_Genus")]
        public int OrganicGenus { get; init; }

        [JsonPropertyName("Organic_Genus_Encountered")]
        public int OrganicGenusEncountered { get; init; }

        [JsonPropertyName("Organic_Planets")]
        public int OrganicPlanets { get; init; }

        [JsonPropertyName("Organic_Species")]
        public int OrganicSpecies { get; init; }

        [JsonPropertyName("Organic_Species_Encountered")]
        public int OrganicSpeciesEncountered { get; init; }

        [JsonPropertyName("Organic_Systems")]
        public int OrganicSystems { get; init; }

        [JsonPropertyName("Organic_Variant_Encountered")]
        public int OrganicVariantEncountered { get; init; }
    }
}
