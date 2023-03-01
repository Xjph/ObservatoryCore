using System;
using System.Net.Http;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;

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
        [Obsolete] // Replaced by LogMonitorStateChanged 
        public void ReadAllStarted()
        { }

        /// <summary>
        /// Method called when "Read All" journal processing completes.<br/>
        /// Used to track if a "Read All" operation is in progress or not to avoid unnecessary processing or notifications.<br/>
        /// Can be omitted for plugins which do not require the distinction.
        /// </summary>
        [Obsolete] // Replaced by LogMonitorStateChanged
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
        public Action<Exception, String> GetPluginErrorLogger (IObservatoryPlugin plugin);

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
    }
}
