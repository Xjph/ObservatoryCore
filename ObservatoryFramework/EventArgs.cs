using System;

namespace Observatory.Framework
{
    /// <summary>
    /// Provides data for Elite Dangerous journal events.
    /// </summary>
    public class JournalEventArgs : EventArgs
    {
        /// <summary>
        /// <para>Type of journal entry that triggered event.</para>
        /// <para>Will be a class from the Observatory.Framework.Files.Journal namespace derived from JournalBase, or JournalBase itself in the case of an unhandled journal event type.</para>
        /// </summary>
        public Type journalType;
        /// <summary>
        /// <para>Elite Dangerous journal event, deserialized into a .NET object of the type specified by JournalEventArgs.journalType.</para>
        /// <para>Unhandled json values within a journal entry type will be contained in member property:<br/>Dictionary&lt;string, object&gt; AdditionalProperties.</para>
        /// </summary>
        public object journalEvent;
    }

    /// <summary>
    /// Provides values used as arguments for Observatory notification events.
    /// </summary>
    public class NotificationArgs
    {
        /// <summary>
        /// Text typically displayed as header content.
        /// </summary>
        public string Title;
        /// <summary>
        /// SSML representation of Title text.<br/>
        /// This value is optional, if omitted the value of <c>NotificationArgs.Title</c> will be used for voice synthesis without markup.
        /// </summary>
        public string TitleSsml;
        /// <summary>
        /// Text typically displayed as body content.
        /// </summary>
        public string Detail;
        /// <summary>
        /// SSML representation of Detail text.<br/>
        /// This value is optional, if omitted the value of <c>NotificationArgs.Detail</c> will be used for voice synthesis without markup.
        /// </summary>
        public string DetailSsml;
        /// <summary>
        /// Specify window timeout in ms (overrides Core setting). Specify 0 timeout to persist until removed via IObservatoryCore.CancelNotification. Default -1 (use Core setting).
        /// </summary>
        public int Timeout = -1;
        /// <summary>
        /// Specify window X position as a percentage from upper left corner (overrides Core setting). Default -1.0 (use Core setting).
        /// </summary>
        public double XPos = -1.0;
        /// <summary>
        /// Specify window Y position as a percentage from upper left corner (overrides Core setting). Default -1.0 (use Core setting).
        /// </summary>
        public double YPos = -1.0;
        /// <summary>
        /// Specifies the desired renderings of the notification.
        /// </summary>
        public NotificationRendering Rendering = NotificationRendering.All;
    }

    [Flags]
    public enum NotificationRendering
    {
        // These need to be multiples of 2 as they're used via masking.
        NativeVisual = 1,
        NativeVocal = 2,
        PluginNotifier = 4,
        All = (NativeVisual | NativeVocal | PluginNotifier)
    }

    public enum LogMonitorState
    {
        None, // This is used only for startup.
        Idle, // Monitoring is stopped
        ReadAllBatch, // Performing a batch read of history. Formerly known as "read-all".
        PreReadBatch, // Currently pre-reading current system context, if enabled, prior to RealTime monitoring
        Realtime, // Real-time monitoring is active
    }

    /// <summary>
    /// Provides information about a LogMonitor state transition.
    /// </summary>
    public class LogMonitorStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The previous LogMonitor state.
        /// </summary>
        public LogMonitorState PreviousState;

        /// <summary>
        /// The new, current LogMonitor state.
        /// </summary>
        public LogMonitorState NewState;

        /// <summary>
        /// Determins if the given state is a batch read of any form.
        /// </summary>
        /// <param name="state">The state to evaluate</param>
        /// <returns>A boolean; True iff the state provided represents a batch-mode read.</returns>
        public static bool IsBatchRead(LogMonitorState state)
        {
            return state == LogMonitorState.ReadAllBatch || state == LogMonitorState.PreReadBatch;
        }
    }
}
