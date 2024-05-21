using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using System.Drawing;

namespace Observatory.Framework.Interfaces
{
    /// <summary>
    /// <para>Base plugin interface containing methods common to both notifiers and workers.</para>
    /// <para>Note: Not intended to be implemented on its own and will not define a functional plugin. Use IObservatoryWorker, IObservatoryNotifier, or both, as appropriate.</para>
    /// </summary>
    public interface IObservatoryPlugin
    {
        /// <summary>
        /// <para>This method will be called on startup by Observatory Core when a plugin is first loaded.</para>
        /// <para>Passes the Core interface to the plugin.</para>
        /// </summary>
        /// <param name="observatoryCore">Object implementing Observatory Core's main interface. A reference to this object should be maintained by the plugin for communication back to Core.</param>
        public void Load(IObservatoryCore observatoryCore);

        /// <summary>
        /// Full name of the plugin. Displayed in the Core settings tab's plugin list.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Short name of the plugin. Used as the tab title for the plugin UI.<br/>
        /// Can be omitted, in which case the full Name will be used.
        /// </summary>
        public string ShortName { get => Name; }

        /// <summary>
        /// Version string displayed in the Core settings tab's plugin list.<br/>
        /// Potentially used for automated version checking. (Not yet implemented)
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Reference to plugin UI to display within its tab.
        /// </summary>
        public PluginUI PluginUI { get; }

        /// <summary>
        /// <para>Accessors for plugin settings object. Should be initialized with a default state during the plugin constructor.</para>
        /// <para>Saving and loading of settings is handled by Observatory Core, and any previously saved settings will be set after plugin instantiation, but before Load() is called.</para>
        /// <para>A plugin's settings class is expected to consist of properties with public getters and setters. The settings UI will be automatically generated based on each property type.<br/>
        /// The [SettingDisplayName(string name)] attribute can be used to specify a display name, otherwise the name of the property will be used.<br/>
        /// Private or internal properties and methods are ignored and can be used for backing values or any other purpose.<br/>
        /// If a public property is necessary but not required to be user accessible the [SettingIgnore] property will suppress display.</para>
        /// </summary>
        public object Settings { get; set; }

        /// <summary>
        /// <para>Plugin-specific object implementing the IComparer interface which is used to sort columns in the basic UI datagrid.</para>
        /// <para>If omitted a natural sort order is used.</para>
        /// </summary>
        public IObservatoryComparer ColumnSorter
        { get => null; }

        /// <summary>
        /// Receives data sent by other plugins.
        /// </summary>
        public void HandlePluginMessage(string sourceName, string sourceVersion, object messageArgs)
        { }

        /// <summary>
        /// <para>Plugin specific data export implementation. Omit or return null to use Observatory's own export process.</para>
        /// <para>While default behaviour is expected to be a delimited text file (i.e., .csv), a plugin may create a file in any format.</para>
        /// </summary>
        /// <param name="delimiter">Column delimiter for csv export.</param>
        /// <param name="filetype">File extension to use for file. Change this when returning a file format other than delimited text.</param>
        /// <returns>File content as a byte array.</returns>
        public byte[] ExportContent(string delimiter, ref string filetype) 
        {
            return null;
        }

        /// <summary>
        /// Called when Observatory finishes loading and the UI is ready.
        /// </summary>
        public void ObservatoryReady()
        { }
    }

    /// <summary>
    /// <para>Interface for worker plugins which process journal data to update their UI or send notifications.</para>
    /// <para>Work required on plugin startup — for example object instantiation — can be done in the constructor or Load() method.<br/>
    /// Be aware that saved settings will not be available until Load() is called.</para>
    /// </summary>
    public interface IObservatoryWorker : IObservatoryPlugin
    {
        /// <summary>
        /// Method called when new journal data is processed. Most work done by worker plugins will occur here.
        /// </summary>
        /// <typeparam name="TJournal">Specific type of journal entry being received.</typeparam>
        /// <param name="journal"><para>Elite Dangerous journal event, deserialized into a .NET object.</para>
        /// <para>Unhandled json values within a journal entry type will be contained in member property:<br/>Dictionary&lt;string, object&gt; AdditionalProperties.</para>
        /// <para>Unhandled journal event types will be type JournalBase with all values contained in AdditionalProperties.</para></param>
        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase;

        /// <summary>
        /// Method called when status.json content is updated.<br/>
        /// Can be omitted for plugins which do not use this data.
        /// </summary>
        /// <param name="status">Player status.json content, deserialized into a .NET object.</param>
        public void StatusChange(Status status)
        { }

        /// <summary>
        /// Called when the LogMonitor changes state. Useful for suppressing output in certain situations
        /// such as batch reads (ie. "Read all") or responding to other state transitions.
        /// </summary>
        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs eventArgs)
        { }

        /// <summary>
        /// Method called when the user begins "Read All" journal processing, before any journal events are sent.<br/>
        /// Used to track if a "Read All" operation is in progress or not to avoid unnecessary processing or notifications.<br/>
        /// Can be omitted for plugins which do not require the distinction.
        /// </summary>
        [Obsolete("Deprecated in favour of LogMonitorStateChanged")]
        public void ReadAllStarted()
        { }

        /// <summary>
        /// Method called when "Read All" journal processing completes.<br/>
        /// Used to track if a "Read All" operation is in progress or not to avoid unnecessary processing or notifications.<br/>
        /// Can be omitted for plugins which do not require the distinction.
        /// </summary>
        [Obsolete("Deprecated in favour of LogMonitorStateChanged")]
        public void ReadAllFinished()
        { }
    }

    /// <summary>
    /// <para>Interface for notifier plugins which receive notification events from other plugins for any purpose.</para>
    /// <para>Work required on plugin startup — for example object instantiation — can be done in the constructor or Load() method.<br/>
    /// Be aware that saved settings will not be available until Load() is called.</para>
    /// </summary>
    public interface IObservatoryNotifier : IObservatoryPlugin
    {
        /// <summary>
        /// Method called when other plugins send notification events to Observatory Core.
        /// </summary>
        /// <param name="notificationEventArgs">Details of the notification as sent from the originating worker plugin.</param>
        public void OnNotificationEvent(NotificationArgs notificationEventArgs);

        /// <summary>
        /// Property set by notification plugins to indicate to Core 
        /// that native audio notifications should be disabled/suppressed.
        /// </summary>
        public bool OverrideAudioNotifications
        { get => false; }

        /// <summary>
        /// Property set by notification plugins to indicate to Core 
        /// that native popup notifications should be disabled/suppressed.
        /// </summary>
        public bool OverridePopupNotifications
        { get => false; }
    }

    /// <summary>
    /// Interface passed by Observatory Core to plugins. Primarily used for sending notifications and UI updates back to Core.
    /// </summary>
    public interface IObservatoryCore
    {
        /// <summary>
        /// Send a notification out to all native notifiers and any plugins implementing IObservatoryNotifier.
        /// </summary>
        /// <param name="title">Title text for notification.</param>
        /// <param name="detail">Detail/body text for notificaiton.</param>
        /// <returns>Guid associated with the notification during its lifetime. Used as an argument with CancelNotification and UpdateNotification.</returns>
        public Guid SendNotification(string title, string detail);

        /// <summary>
        /// Send a notification with arguments out to all native notifiers and any plugins implementing IObservatoryNotifier.
        /// </summary>
        /// <param name="notificationEventArgs">NotificationArgs object specifying notification content and behaviour.</param>
        /// <returns>Guid associated with the notification during its lifetime. Used as an argument with CancelNotification and UpdateNotification.</returns>
        public Guid SendNotification(NotificationArgs notificationEventArgs);

        /// <summary>
        /// Cancel or close an active notification.
        /// </summary>
        /// <param name="notificationId">Guid of notification to be cancelled.</param>
        public void CancelNotification(Guid notificationId);

        /// <summary>
        /// Update an active notification with a new set of NotificationsArgs. Timeout values are reset and begin counting again from zero if specified.
        /// </summary>
        /// <param name="notificationId">Guid of notification to be updated.</param>
        /// <param name="notificationEventArgs">NotificationArgs object specifying updated notification content and behaviour.</param>
        public void UpdateNotification(Guid notificationId, NotificationArgs notificationEventArgs);

        /// <summary>
        /// Add an item to the bottom of the basic UI grid.
        /// </summary>
        /// <param name="worker">Reference to the calling plugin's worker interface.</param>
        /// <param name="item">Grid item to be added. Object type should match original template item used to create the grid.</param>
        public void AddGridItem(IObservatoryWorker worker, object item);

        /// <summary>
        /// Add multiple items to the bottom of the basic UI grid.
        /// </summary>
        /// <param name="worker">Reference to the calling plugin's worker interface.</param>
        /// <param name="items">Grid items to be added. Object types should match original template item used to create the grid.</param>
        public void AddGridItems(IObservatoryWorker worker, IEnumerable<object> items);

        /// <summary>
        /// Replace the contents of the grid with the provided items.
        /// </summary>
        /// <param name="worker">Reference to the calling plugin's worker interface.</param>
        /// <param name="items">Grid items to be added. Object types should match original template item used to create the grid.</param>
        public void SetGridItems(IObservatoryWorker worker, IEnumerable<object> items);

        /// <summary>
        /// Clears basic UI grid, removing all items.
        /// </summary>
        /// <param name="worker">Reference to the calling plugin's worker interface.</param>
        /// <param name="templateItem">Template item used to re-initialise the grid.</param>
        public void ClearGrid(IObservatoryWorker worker, object templateItem);

        /// <summary>
        /// Requests current Elite Dangerous status.json content.
        /// </summary>
        /// <returns>Status object reflecting current Elite Dangerous player status.</returns>
        public Status GetStatus();

        /// <summary>
        /// Version string of Observatory Core.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Returns a delegate for logging an error for the calling plugin. A plugin can wrap this method
        /// or pass it along to its collaborators.
        /// </summary>
        /// <param name="plugin">The calling plugin</param>
        public Action<Exception, string> GetPluginErrorLogger(IObservatoryPlugin plugin);

        /// <summary>
        /// Perform an action on the current Avalonia UI thread.
        /// </summary>
        /// <param name="action"></param>
        public void ExecuteOnUIThread(Action action);

        /// <summary>
        /// Shared application HttpClient object. Provided so that plugins can adhere to .NET recommended behaviour of a single HttpClient object per application.
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// Returns the current LogMonitor state.
        /// </summary>
        public LogMonitorState CurrentLogMonitorState { get; }

        /// <summary>
        /// Returns true if the current LogMonitor state represents a batch-read mode.
        /// </summary>
        public bool IsLogMonitorBatchReading { get; }

        /// <summary>
        /// Retrieves and ensures creation of a location which can be used by the plugin to store persistent data.
        /// </summary>
        public string PluginStorageFolder { get; }

        /// <summary>
        /// Plays audio file using default audio device.
        /// </summary>
        /// <param name="filePath">Absolute path to audio file.</param>
        public Task PlayAudioFile(string filePath);

        /// <summary>
        /// Sends arbitrary data to all other plugins. The full name and version of the sending plugin will be used to identify the sender to any recipients.
        /// </summary>
        public void SendPluginMessage(IObservatoryPlugin plugin, object message);

        /// <summary>
        /// Register a UI control for themeing.
        /// </summary>
        /// <param name="control">UI Control object or ToolStripMenuItem</param>
        public void RegisterControl(object control);

        /// <summary>
        /// Remove a UI control from themeing.
        /// </summary>
        /// <param name="control">UI Control object or ToolStripMenuItem</param>
        public void UnregisterControl(object control);

        /// <summary>
        /// Retrieves the name of the currently selected UI theme.
        /// </summary>
        /// <returns>Name of the theme as a string.</returns>
        public string GetCurrentThemeName();

        /// <summary>
        /// Retrieves the details of the currently selected UI theme.
        /// </summary>
        /// <returns>A dictionary keyed by the type of control as a string and the associated colour, e.g. { "Button.BackColor", Color.DimGrey } </returns>
        public Dictionary<string, Color> GetCurrentThemeDetails();

        /// <summary>
        /// Request that Observatory save plugin settings to preserve changes made outside the settings UI.
        /// </summary>
        public void SaveSettings(IObservatoryPlugin plugin);

        /// <summary>
        /// Request that Observatory open the setting form for the current plugin
        /// </summary>
        public void OpenSettings(IObservatoryPlugin plugin);

        /// <summary>
        /// Deserializes a journal event from JSON into a journal object.
        /// </summary>
        /// <param name="json">JSON string representing a journal event</param>
        /// <param name="replay">(Optional) Replay this event as a current journal entry to all plugins</param>
        /// <returns>Journal object of the json passed in</returns>
        public JournalEventArgs DeserializeEvent(string json, bool replay = false);

        /// <summary>
        /// Switches focus to the named plugin (if found).
        /// </summary>
        /// <param name="pluginName">The short name of the plugin which should be focused.</param>
        public void FocusPlugin(string pluginName);
    }

    /// <summary>
    /// Extends the base IComparer interface with exposed values for the column ID and sort order to use.
    /// </summary>
    public interface IObservatoryComparer : System.Collections.IComparer
    {
        /// <summary>
        /// Column ID to be currently sorted by.
        /// </summary>
        public int SortColumn { get; set; }

        /// <summary>
        /// Current order of sorting. Ascending = 1, Descending = -1, No sorting = 0.
        /// </summary>
        public int Order { get; set; }
    }

}
