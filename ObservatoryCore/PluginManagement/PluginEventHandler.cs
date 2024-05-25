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
        private IEnumerable<IObservatoryWorker> observatoryWorkers;
        private IEnumerable<IObservatoryNotifier> observatoryNotifiers;
        private HashSet<IObservatoryPlugin> disabledPlugins;
        private List<(string error, string detail)> errorList;
        private System.Timers.Timer timer;

        public PluginEventHandler(IEnumerable<IObservatoryWorker> observatoryWorkers, IEnumerable<IObservatoryNotifier> observatoryNotifiers)
        {
            this.observatoryWorkers = observatoryWorkers;
            this.observatoryNotifiers = observatoryNotifiers;
            disabledPlugins = new();
            errorList = new();

            InitializeTimer();
        }

        private void InitializeTimer()
        {
            // Use a timer to delay error reporting until incoming errors are "quiet" for one full second.
            // Should resolve issue where repeated plugin errors open hundreds of error windows.
            timer = new();
            timer.Interval = 1000;
            timer.Elapsed += ReportErrorsIfAny;
        }

        public void OnJournalEvent(object source, JournalEventArgs journalEventArgs)
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

        public void OnStatusUpdate(object sourece, JournalEventArgs journalEventArgs)
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

        internal void OnLogMonitorStateChanged(object sender, LogMonitorStateChangedEventArgs e)
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

        public void OnNotificationEvent(object source, NotificationArgs notificationArgs)
        {
            foreach (var notifier in observatoryNotifiers)
            {
                if (disabledPlugins.Contains(notifier)) continue;
                try
                {
                    notifier.OnNotificationEvent(notificationArgs);
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

        public void OnPluginMessageEvent(object _, PluginMessageArgs messageArgs)
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

        private void ReportErrorsIfAny(object sender, ElapsedEventArgs e)
        {
            if (errorList.Any())
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
