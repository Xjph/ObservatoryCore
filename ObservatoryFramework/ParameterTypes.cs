using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.ParameterTypes
{
    /// <summary>
    /// A parameter class for PlayAudioFile.
    /// </summary>
    public class AudioOptions
    {
        public AudioOptions()
        {
            Instant = false;
            DeleteAfterPlay = false;
        }

        /// <summary>
        /// If set to true, the associated audio file is played immediately rather than queued. Default is false (queued).
        /// </summary>
        public bool Instant { get; set; }
        /// <summary>
        /// If set to true, the provided audio file is deleted after playback finishes. Default is false (caller is responsible
        /// for managing audio file cleanup).
        /// </summary>
        public bool DeleteAfterPlay { get; set; }
    }
}
