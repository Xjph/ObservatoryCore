using Observatory.Framework;
using Observatory.Framework.Interfaces;
using Observatory.Framework.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observatory.Framework.Files.Journal;

namespace Observatory.PluginManagement
{
    class PluginEventHandler
    {
        private IEnumerable<IObservatoryWorker> observatoryWorkers;
        private IEnumerable<IObservatoryNotifier> observatoryNotifiers;

        public PluginEventHandler(IEnumerable<IObservatoryWorker> observatoryWorkers, IEnumerable<IObservatoryNotifier> observatoryNotifiers)
        {
            this.observatoryWorkers = observatoryWorkers;
            this.observatoryNotifiers = observatoryNotifiers;
        }

        public void OnJournalEvent(object source, JournalEventArgs journalEventArgs)
        {
            foreach (var worker in observatoryWorkers)
            {
                worker.JournalEvent((JournalBase)journalEventArgs.journalEvent);
            }
        }

        public void OnStatusUpdate(object sourece, JournalEventArgs journalEventArgs)
        {
            foreach (var worker in observatoryWorkers)
            {
                worker.StatusChange((Status)journalEventArgs.journalEvent);
            }
        }

        public void OnNotificationEvent(object source, NotificationEventArgs notificationEventArgs)
        {
            foreach (var notifier in observatoryNotifiers)
            {
                notifier.OnNotificationEvent(notificationEventArgs.Title, notificationEventArgs.Detail);
            }
        }
    }
}
