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
        private List<(string error, string detail)> errorList;
        private System.Timers.Timer timer;

        public PluginEventHandler(IEnumerable<IObservatoryWorker> observatoryWorkers, IEnumerable<IObservatoryNotifier> observatoryNotifiers)
        {
            this.observatoryWorkers = observatoryWorkers;
            this.observatoryNotifiers = observatoryNotifiers;
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
                try
                {
                    worker.LogMonitorStateChanged(e);
                }
                catch (Exception ex)
                {
                    RecordError(ex, worker.Name, "LogMonitorStateChanged event", ex.StackTrace);
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
                    RecordError(ex, notifier.Name, notificationArgs.Title, notificationArgs.Detail);
                }
                ResetTimer();
            }
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
            errorList.Add(($"Error in {ex.PluginName}: {ex.Message}", ex.StackTrace));
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
}
