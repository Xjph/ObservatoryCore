using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework
{
    /// <summary>
    /// Represents the sender of a message.
    /// </summary>
    public class MessageSender
    {
        /// <summary>
        /// ID of the sending plugin, or Guid.Empty if the sender does not provide an ID.
        /// </summary>
        public Guid Guid { get; init; }
        /// <summary>
        /// Full name of the sending plugin.
        /// </summary>
        public string FullName { get; init; }
        /// <summary>
        /// Short name of the sending plugin.
        /// </summary>
        public string ShortName { get; init; }
        /// <summary>
        /// Version of the sending plugin.
        /// </summary>
        public string Version { get; init; }
    }
}
