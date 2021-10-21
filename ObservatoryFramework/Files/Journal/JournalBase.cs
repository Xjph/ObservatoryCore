using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class JournalBase
    {
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; init; }

        [JsonIgnore]
        public DateTime TimestampDateTime
        {
            get
            {
                return DateTime.ParseExact(Timestamp, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal);
            }
        }

        [JsonPropertyName("event")]
        public string Event { get;  init; }

        [JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; init; }

        [JsonIgnore]
        public string Json 
        {
            get => json; 
            set
            {
                if (json == null || string.IsNullOrWhiteSpace(json))
                {
                    json = value;
                }
                else
                {
                    throw new Exception("Journal property \"Json\" can only be set while empty.");
                }
            }
        }

        private string json;

    }
}
