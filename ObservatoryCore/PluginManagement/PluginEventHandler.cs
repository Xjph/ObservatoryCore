using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using System.Timers;
using Observatory.Utils;

namespace Observatory.PluginManagement
{
    class PluginEventHandler
    {
        private readonly IEnumerable<IObservatoryWorker> observatoryWorkers;
        private readonly IEnumerable<IObservatoryNotifier> observatoryNotifiers;
        private readonly HashSet<IObservatoryPlugin> disabledPlugins;
        private readonly List<(string error, string detail)> errorList;
        private System.Timers.Timer timer;

        public PluginEventHandler(IEnumerable<IObservatoryWorker> observatoryWorkers, IEnumerable<IObservatoryNotifier> observatoryNotifiers)
        {
            this.observatoryWorkers = observatoryWorkers;
            this.observatoryNotifiers = observatoryNotifiers;
            disabledPlugins = [];
            errorList = [];
            timer = new();

            InitializeTimer();
        }

        public HashSet<IObservatoryPlugin> DisabledPlugins {  get => disabledPlugins; }

        private void InitializeTimer()
        {
            // Use a timer to delay error reporting until incoming errors are "quiet" for one full second.
            // Should resolve issue where repeated plugin errors open hundreds of error windows.
            timer.Interval = 1000;
            timer.Elapsed += ReportErrorsIfAny;
        }

        public void OnJournalEvent(object? _, JournalEventArgs journalEventArgs)
        {
            foreach (var worker in observatoryWorkers)
            {
                if (disabledPlugins.Contains(worker)) continue;
                try
                {
                    worker.JournalEvent((JournalBase)journalEventArgs.journalEvent);
                }
                catch (PluginException ex)
                {
                    RecordError(ex);
                }
                catch (Exception ex)
                {
                    RecordError(ex, worker.Name, journalEventArgs.journalType.Name, ((JournalBase)journalEventArgs.journalEvent).Json);
                }
                ResetTimer();
            }
        }

        public void OnStatusUpdate(object? _, JournalEventArgs journalEventArgs)
        {
            foreach (var worker in observatoryWorkers)
            {
                if (disabledPlugins.Contains(worker)) continue;
                try
                {
                    worker.StatusChange((Status)journalEventArgs.journalEvent);
                }
                catch (PluginException ex)
                {
                    RecordError(ex);
                }
                catch (Exception ex)
                {
                    RecordError(ex, worker.Name, journalEventArgs.journalType.Name, ((JournalBase)journalEventArgs.journalEvent).Json);
                }
                ResetTimer();
            }
        }

        internal void OnLogMonitorStateChanged(object? _, LogMonitorStateChangedEventArgs e)
        {
            foreach (var worker in observatoryWorkers)
            {
                if (disabledPlugins.Contains(worker)) continue;
                try
                {
                    worker.LogMonitorStateChanged(e);
                }
                catch (Exception ex)
                {
                    RecordError(ex, worker.Name, "LogMonitorStateChanged event", ex.StackTrace ?? "");
                }
            }
        }

        public void OnNotificationEvent(object? _, NotificationArgs notificationArgs)
        {
            UpdateOrNotify(notificationArgs, false);
        }

        public void OnNotificationUpdate(object? _, NotificationArgs notificationArgs)
        {
            UpdateOrNotify(notificationArgs, true);
        }

        public void OnNotificationCancel(object? _, Guid guid)
        {
            foreach (var notifier in observatoryNotifiers)
            {
                notifier.CancelNotification(guid);
            }
        }

        private void UpdateOrNotify(NotificationArgs notificationArgs, bool update)
        {
            foreach (var notifier in observatoryNotifiers)
            {
                if (disabledPlugins.Contains(notifier)) continue;
                if (LogMonitorStateChangedEventArgs.IsBatchRead(LogMonitor.GetInstance.CurrentState)
                    && !notifier.OverrideAcceptNotificationsDuringBatch) continue;

                try
                {
                    // We may get notifications that are not PluginNotifier destined if we have
                    // a plugin which overrides a native handler. Only deliver native notifications if
                    // the plugin declares it's overriding.
                    if ((notifier.OverrideAudioNotifications
                            && (notificationArgs.Rendering & NotificationRendering.NativeVocal) != 0)
                        || (notifier.OverridePopupNotifications
                            && (notificationArgs.Rendering & NotificationRendering.NativeVisual) != 0)
                        || (notificationArgs.Rendering & NotificationRendering.PluginNotifier) != 0)
                    {
                        if (update)
                            notifier.UpdateNotification(notificationArgs);
                        else
                            notifier.OnNotificationEvent(notificationArgs);
                    }
                }
                catch (PluginException ex)
                {
                    RecordError(ex);
                }
                catch (Exception ex)
                {
                    RecordError(ex, notifier.Name, notificationArgs.Title, notificationArgs.Detail);
                }
                ResetTimer();
            }
        }

        private static Guid GetPluginGuid(IObservatoryPlugin plugin)
        {
            var guidProp = plugin.GetType().GetProperty("Guid");
            var pluginGuid = guidProp?.GetValue(plugin);
            return pluginGuid is Guid guid ? guid : Guid.Empty;
        }

        public void OnPluginMessageEvent(object? _, PluginMessageArgs messageArgs)
        {
            MessageSender sender = new()
            {
                Guid = messageArgs.SourceId,
                FullName = messageArgs.LongName,
                ShortName = messageArgs.ShortName,
                Version = messageArgs.SourceVersion
            };

            foreach (var plugin in observatoryNotifiers.Cast<IObservatoryPlugin>().Union(observatoryWorkers)
                .Where(p => messageArgs.TargetId == Guid.Empty || messageArgs.TargetId == GetPluginGuid(p)))
            {
                if (disabledPlugins.Contains(plugin)) continue;

                // Clone the message properties and create new message object to prevent bad actor plugins from modifying in-flight
                var clonedPayload = messageArgs.Message.MessagePayload.ToDictionary(item => item.Key, item => item.Value);
                Span<char> clonedMessageType = [];
                messageArgs.Message.MessageType.CopyTo(clonedMessageType);
                var clonedReplyId = new Guid((messageArgs.Message.InReplyTo ?? Guid.Empty).ToString());

                PluginMessage message = new(clonedMessageType.ToString(), clonedPayload, clonedReplyId);

                try
                {
                    plugin.HandlePluginMessage(sender, message);
                }
                catch (PluginException ex)
                {
                    RecordError(ex);
                }
                catch(Exception ex)
                {
                    RecordError(ex, plugin.Name, "OnPluginMessageEvent event", "");
                }
            }
        }

        public void OnLegacyPluginMessageEvent(object? _, LegacyPluginMessageArgs messageArgs)
        {
            MessageSender sender = new()
            {
                Guid = Guid.Empty,
                FullName = messageArgs.SourceName,
                ShortName = messageArgs.SourceName,
                Version = messageArgs.SourceVersion
            };

            foreach (var plugin in observatoryNotifiers.Cast<IObservatoryPlugin>().Union(observatoryWorkers)
                .Where(x => String.IsNullOrEmpty(messageArgs.TargetShortName) || x.ShortName == messageArgs.TargetShortName)
                .Where(x => x.Name != messageArgs.SourceName))
            {
                if (disabledPlugins.Contains(plugin)) continue;

                try
                {
                    plugin.HandlePluginMessage(messageArgs.SourceName, messageArgs.SourceVersion, messageArgs.Message);
                    PluginMessage wrappedLegacyMessage = new (
                        "LegacyPluginMessage", 
                        new Dictionary<string, object> { { "message", messageArgs.Message } }
                        );
                    plugin.HandlePluginMessage(sender, wrappedLegacyMessage);
                }
                catch (PluginException ex)
                {
                    RecordError(ex);
                }
                catch(Exception ex)
                {
                    RecordError(ex, plugin.Name, "OnPluginMessageEvent event", "");
                }
            }
        }

        public void SetPluginEnabled(IObservatoryPlugin plugin, bool enabled)
        {
            if (enabled) disabledPlugins.Remove(plugin);
            else disabledPlugins.Add(plugin);
        }

        private void ResetTimer()
        {
            timer.Stop();
            try
            {
                timer.Start();
            }
            catch
            {
                // Not sure why this happens, but I've reproduced it twice in a row after hitting
                // read-all while also playing (ie. generating journals).
                InitializeTimer();
                timer.Start();
            }
        }

        private void RecordError(PluginException ex)
        {
            errorList.Add(($"Error in {ex.PluginName}: {ex.Message}", ex.StackTrace ?? ""));
        }

        private void RecordError(Exception ex, string plugin, string eventType, string eventDetail)
        {
            errorList.Add(($"Error in {plugin} while handling {eventType}: {ex.Message}", eventDetail));
        }

        private void ReportErrorsIfAny(object? _, ElapsedEventArgs e)
        {
            if (errorList.Count != 0)
            {
                ErrorReporter.ShowErrorPopup($"Plugin Error{(errorList.Count > 1 ? "s" : "")}", errorList);
                
                timer.Stop();
            }
        }
    }

    internal class PluginMessageArgs
    {
        internal string LongName;
        internal string ShortName;
        internal Guid SourceId;
        internal string SourceVersion;
        internal Guid TargetId;
        internal PluginMessage Message;

        internal PluginMessageArgs(IObservatoryPlugin plugin, Guid targetId, PluginMessage message)
        {
            LongName = plugin.Name;
            ShortName = plugin.ShortName;
            SourceId = PluginManager.GetPluginGuid(plugin);
            SourceVersion = plugin.Version;
            TargetId = targetId;
            Message = message;
        }
    }

    internal class LegacyPluginMessageArgs
    {
        internal string SourceName;
        internal string SourceVersion;
        internal string TargetShortName;
        internal object Message;

        internal LegacyPluginMessageArgs(string sourceName, string sourceVersion, string targetShortName, object message)
        {
            SourceName = sourceName;
            SourceVersion = sourceVersion;
            TargetShortName = targetShortName;
            Message = message;
        }
    }
}
