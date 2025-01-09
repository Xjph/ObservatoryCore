using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class JournalBase
    {
        [JsonPropertyName("timestamp")]
        [JsonPropertyOrder(-2)]
        public string Timestamp { get; init; }

        [JsonIgnore]
        public DateTime TimestampDateTime
        {
            get => ParseDateTime(Timestamp);
        }

        [JsonPropertyName("event")]
        [JsonPropertyOrder(-1)]
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

        // For use by Journal object classes for .*DateTime properties, like TimestampeDateTime, above.
        internal static DateTime ParseDateTime(string value)
        {
            if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTimeValue))
            {
                return dateTimeValue;
            }
            else
            {
                return new DateTime();
            }
        }
    }
}
