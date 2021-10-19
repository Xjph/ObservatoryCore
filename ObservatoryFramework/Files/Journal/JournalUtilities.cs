using System;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace Observatory.Framework.Files
{
    public static class JournalUtilities
    {
        public static string GetEventType(string line)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(line));
            string result = string.Empty;

            try
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "event")
                    {
                        reader.Read();
                        result = reader.GetString();
                    }
                }
            }
            catch
            {
                
                result = "InvalidJson";
            }


            return result;
        }

        public static string CleanScanEvent(string line)
        {
            return line.Replace("\"RotationPeriod\":inf,", "");
        }

        public const string ObsoleteMessage = "Unused in Elite Dangerous 3.7+, may appear in legacy journal data.";

        public const string UnusedMessage = "Documented by Frontier, but no occurances of this value ever found in real journal data.";
     
    }
}
