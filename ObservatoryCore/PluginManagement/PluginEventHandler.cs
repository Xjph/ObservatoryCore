using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.Framework.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using Observatory.Framework.Files.Journal;
using System.Timers;

namespace Observatory.PluginManagement
{
    class PluginEventHandler
    {
        private IEnumerable<IObservatoryWorker> observatoryWorkers;
        private IEnumerable<IObservatoryNotifier> observatoryNotifiers;
        private List<string> errorList;
        private Timer timer;

        public PluginEventHandler(IEnumerable<IObservatoryWorker> observatoryWorkers, IEnumerable<IObservatoryNotifier> observatoryNotifiers)
        {
            this.observatoryWorkers = observatoryWorkers;
            this.observatoryNotifiers = observatoryNotifiers;
            errorList = new();

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
                    RecordError(ex, worker.Name, journalEventArgs.journalType.Name);
                }
                ResetTimer();
            }
        }

        public void OnStatusUpdate(object sourece, JournalEventArgs journalEventArgs)
        {
            foreach (var worker in observatoryWorkers)
            {
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
                    RecordError(ex, worker.Name, journalEventArgs.journalType.Name);
                }
                ResetTimer();
            }
        }

        internal void OnLogMonitorStateChanged(object sender, LogMonitorStateChangedEventArgs e)
        {
            foreach (var worker in observatoryWorkers)
            {
                try
                {
                    worker.LogMonitorStateChanged(e);
                }
                catch (Exception ex)
                {
                    RecordError(ex, worker.Name, "LogMonitorStateChanged event");
                }
            }
        }

        public void OnNotificationEvent(object source, NotificationArgs notificationArgs)
        {
            foreach (var notifier in observatoryNotifiers)
            {
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
                    RecordError(ex, notifier.Name, notificationArgs.Title);
                }
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            timer.Stop();
            timer.Start();
        }

        private void RecordError(PluginException ex)
        {
            errorList.Add($"Error in {ex.PluginName}: {ex.Message}");
        }

        private void RecordError(Exception ex, string plugin, string eventType)
        {
            errorList.Add($"Error in {plugin} while handling {eventType}: {ex.Message}");
        }

        private void ReportErrorsIfAny(object sender, ElapsedEventArgs e)
        {
            if (errorList.Any())
            {
                ErrorReporter.ShowErrorPopup($"Plugin Error{(errorList.Count > 1 ? "s" : "")}", string.Join(Environment.NewLine, errorList));

                errorList.Clear();
            }
        }
    }
}
