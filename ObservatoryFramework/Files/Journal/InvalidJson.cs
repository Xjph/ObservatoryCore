using System;
using System.Collections.Immutable;
using System.Text;

namespace Observatory.Framework.Files.Journal
{
    public class InvalidJson : JournalBase
    {
        public string OriginalEvent { get; init; }
    }
}
