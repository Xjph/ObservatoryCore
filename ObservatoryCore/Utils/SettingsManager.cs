#if PORTABLE
using System.Reflection;
using System.Text.Json;
#endif

namespace Observatory.Utils
{
    internal static class SettingsManager
    {
        internal static void Save()
        {
#if DEBUG || RELEASE
            Properties.Core.Default.Save();
#elif PORTABLE

            Dictionary<string, object?> settings = new();

            foreach (PropertyInfo property in Properties.Core.Default.GetType().GetProperties())
            {
                if (property.CanRead && property.CanWrite && !property.GetIndexParameters().Any())
                    settings.Add(
                        property.Name,
                        property.GetValue(Properties.Core.Default)
                        );
            }

            string serializedSettings = JsonSerializer.Serialize(settings, new JsonSerializerOptions()
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                
            });
            File.WriteAllText("Observatory.config", serializedSettings);
#endif
        }

        internal static void Load()
        {
#if PORTABLE
            if (File.Exists("Observatory.config"))
            {
                string savedSettings = File.ReadAllText("Observatory.config");
                Dictionary<string, object?>? settings = JsonSerializer.Deserialize<Dictionary<string, object?>>(savedSettings);
                if (settings != null)
                {
                    var properties = Properties.Core.Default.GetType().GetProperties();
                    
                    foreach (var savedProperty in settings)
                    {

                        var currentProperty = properties.Where(p => p.Name == savedProperty.Key);
                        if (currentProperty.Any())
                        {
                            JsonElement? value = (JsonElement?)savedProperty.Value;
                            var deserializedValue = value?.Deserialize(currentProperty.First().PropertyType);
                            currentProperty.First().SetValue(Properties.Core.Default, deserializedValue);
                        }

                    }
                }
            }
#endif
        }
    }
}
