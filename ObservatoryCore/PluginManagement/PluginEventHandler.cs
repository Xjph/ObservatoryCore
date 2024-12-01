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

        public void OnPluginMessageEvent(object? _, PluginMessageArgs messageArgs)
        {
            foreach (var plugin in observatoryNotifiers.Cast<IObservatoryPlugin>().Concat(observatoryWorkers))
            {
                if (disabledPlugins.Contains(plugin)) continue;

                try
                {
                    plugin.HandlePluginMessage(messageArgs.SourceName, messageArgs.SourceVersion, messageArgs.Message);
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
        internal string SourceName;
        internal string SourceVersion;
        internal object Message;

        internal PluginMessageArgs(string sourceName, string sourceVersion, object message)
        {
            SourceName = sourceName;
            SourceVersion = sourceVersion;
            Message = message;
        }
    }
}
