using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

namespace Observatory.Framework.Files.Converters
{
    class ThargoidWarRemainingTimeConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString();
                
                int dayCount = Int32.TryParse(value.Split(' ')[0], out int days)
                    ? days
                    : 0;

                return dayCount;
            }                
            else
                return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString() + " Days");
        }
    }
}
