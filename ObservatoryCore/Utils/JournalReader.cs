using Observatory.Framework.Files.Journal;
using System.Text;
using System.Text.Json;
using System.Reflection;

namespace Observatory.Utils
{
    public class JournalReader
    {
        public static TJournal ObservatoryDeserializer<TJournal>(string json) where TJournal : JournalBase
        {
            TJournal deserialized;

            if (typeof(TJournal) == typeof(InvalidJson))
            {
                InvalidJson invalidJson;
                try
                {
                    var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
                    string? eventType = string.Empty;
                    string? timestamp = string.Empty;

                    while ((eventType == string.Empty || timestamp == string.Empty) && reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            if (reader.GetString() == "event")
                            {
                                reader.Read();
                                eventType = reader.GetString();
                            }
                            else if (reader.GetString() == "timestamp")
                            {
                                reader.Read();
                                timestamp = reader.GetString();
                            }
                        }
                    }

                    invalidJson = new InvalidJson()
                    {
                        Event = "InvalidJson",
                        Timestamp = timestamp,
                        OriginalEvent = eventType
                    };
                }
                catch
                {
                    invalidJson = new InvalidJson()
                    {
                        Event = "InvalidJson",
                        Timestamp = string.Empty,
                        OriginalEvent = "Invalid"
                    };
                }

                deserialized = (TJournal)Convert.ChangeType(invalidJson, typeof(TJournal));

            }
            //Journal potentially had invalid JSON for a brief period in 2017, check for it and remove.
            //TODO: Check if this gets handled by InvalidJson now.
            else if (typeof(TJournal) == typeof(Scan) && json.Contains("\"RotationPeriod\":inf"))
            {
                deserialized = JsonSerializer.Deserialize<TJournal>(json.Replace("\"RotationPeriod\":inf,", ""));
            }
            else
            {
                deserialized = JsonSerializer.Deserialize<TJournal>(json);
            }
            deserialized.Json = json;

            return deserialized;
        }


        public static Dictionary<string, Type> PopulateEventClasses()
        {
            var eventClasses = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

            var allTypes = Assembly.GetAssembly(typeof(JournalBase))?.GetTypes();

            var journalTypes = allTypes?.Where(a => a.IsSubclassOf(typeof(JournalBase)));

            if (journalTypes != null)
            foreach (var journalType in journalTypes)
            {
                eventClasses.Add(journalType.Name, journalType);
            }

            eventClasses.Add("JournalBase", typeof(JournalBase));

            return eventClasses;
        }
    }
}
