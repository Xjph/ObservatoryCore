using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class SAAScanComplete : JournalBase
    {
        public ulong SystemAddress { get; init; }
        public string BodyName { get; init; }
        public int BodyID { get; init; }
        /// <summary>
        /// This property is indicated with strikethrough in Frontier's documentation and may be deprecated.
        /// </summary>
        public ImmutableList<string> Discoverers { get; init; }
        /// <summary>
        /// This property is indicated with strikethrough in Frontier's documentation and may be deprecated.
        /// </summary>
        public ImmutableList<string> Mappers { get; init; }
        public int ProbesUsed { get; init; }
        public int EfficiencyTarget { get; init; }
    }
}
