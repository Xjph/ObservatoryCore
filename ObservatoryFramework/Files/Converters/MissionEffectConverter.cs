using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Converters
{
    public class MissionEffectConverter : JsonConverter<MissionEffect>
    {
        public override MissionEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string effect = reader.GetString();
            //TODO: Find out all possible values
            switch (effect)
            {
                case "+":
                    effect = "Low";
                    break;
                case "++":
                    effect = "Med";
                    break;
                case "+++++":
                    effect = "High";
                    break;
                default:
                    break;
            }

            MissionEffect missionEffect = (MissionEffect)Enum.Parse(typeof(MissionEffect), effect, true);

            return missionEffect;
        }

        public override void Write(Utf8JsonWriter writer, MissionEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
