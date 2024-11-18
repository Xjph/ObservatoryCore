using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Utils
{
    internal static class NativeSpeechHelper
    {
        private const string PROP_VOICE_SYNTHESIZER = "VoiceSynthesizer";
        private const string FIELD_INSTALLED_VOICES = "_installedVoices";

        private const string ONE_CORE_VOICES_REGISTRY = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech_OneCore\Voices";

        private static readonly Type ObjectTokenCategoryType = typeof(SpeechSynthesizer).Assembly
            .GetType("System.Speech.Internal.ObjectTokens.ObjectTokenCategory")!;

        private static readonly Type VoiceInfoType = typeof(SpeechSynthesizer).Assembly
            .GetType("System.Speech.Synthesis.VoiceInfo")!;

        private static readonly Type InstalledVoiceType = typeof(SpeechSynthesizer).Assembly
            .GetType("System.Speech.Synthesis.InstalledVoice")!;


        public static void InjectOneCoreVoices(this SpeechSynthesizer synthesizer)
        {
            var voiceSynthesizer = GetProperty(synthesizer, PROP_VOICE_SYNTHESIZER);
            if (voiceSynthesizer == null) throw new NotSupportedException($"Property not found: {PROP_VOICE_SYNTHESIZER}");

            var installedVoices = GetField(voiceSynthesizer, FIELD_INSTALLED_VOICES) as IList;
            if (installedVoices == null)
                throw new NotSupportedException($"Field not found or null: {FIELD_INSTALLED_VOICES}");

            if (ObjectTokenCategoryType
                    .GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic)?
                    .Invoke(null, new object?[] { ONE_CORE_VOICES_REGISTRY }) is not IDisposable otc)
                throw new NotSupportedException($"Failed to call Create on {ObjectTokenCategoryType} instance");

            using (otc)
            {
                if (ObjectTokenCategoryType
                        .GetMethod("FindMatchingTokens", BindingFlags.Instance | BindingFlags.NonPublic)?
                        .Invoke(otc, new object?[] { null, null }) is not IList tokens)
                    throw new NotSupportedException($"Failed to list matching tokens");

                foreach (var token in tokens)
                {
                    if (token == null || GetProperty(token, "Attributes") == null) continue;

                    var voiceInfo =
                        typeof(SpeechSynthesizer).Assembly
                            .CreateInstance(VoiceInfoType.FullName!, true,
                                BindingFlags.Instance | BindingFlags.NonPublic, null,
                                new object[] { token }, null, null);

                    if (voiceInfo == null)
                        throw new NotSupportedException($"Failed to instantiate {VoiceInfoType}");

                    var installedVoice =
                        typeof(SpeechSynthesizer).Assembly
                            .CreateInstance(InstalledVoiceType.FullName!, true,
                                BindingFlags.Instance | BindingFlags.NonPublic, null,
                                new object[] { voiceSynthesizer, voiceInfo }, null, null);

                    if (installedVoice == null)
                        throw new NotSupportedException($"Failed to instantiate {InstalledVoiceType}");

                    installedVoices.Add(installedVoice);
                }
            }
        }

        private static object? GetProperty(object target, string propName)
        {
            return target.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(target);
        }

        private static object? GetField(object target, string propName)
        {
            return target.GetType().GetField(propName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(target);
        }
    }
}
