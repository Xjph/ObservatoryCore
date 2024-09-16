using Observatory.Framework;
using System.Diagnostics;
using Observatory.Framework.Interfaces;

namespace Observatory.Herald
{
    class HeraldQueue
    {
        private Queue<NotificationArgs> notifications;
        private bool processing;
        private string voice;
        private string style;
        private string rate;
        private SpeechRequestManager speechManager;
        private Action<Exception, String> ErrorLogger;
        private IObservatoryCore core;

        public HeraldQueue(SpeechRequestManager speechManager, Action<Exception, String> errorLogger, IObservatoryCore core)
        {
            this.speechManager = speechManager;
            this.core = core;
            processing = false;
            notifications = new();
            ErrorLogger = errorLogger;
        }


        internal void Enqueue(NotificationArgs notification, string selectedVoice, string selectedStyle = "", string selectedRate = "")
        {
            voice = selectedVoice;
            style = selectedStyle;
            rate = selectedRate;

            Debug.WriteLine("Attempting to de-dupe notification titles against '{0}': '{1}'",
                notification.Title.Trim().ToLower(),
                String.Join(',', notifications.Select(n => n.Title.Trim().ToLower())));

            if (notifications.Where(n => n.Title.Trim().ToLower() == notification.Title.Trim().ToLower()).Any())
            {
                // Suppress title.
                notification.Suppression |= NotificationSuppression.Title;
            }
            notifications.Enqueue(notification);

            if (!processing)
            {
                processing = true;
                ProcessQueueAsync();
            }
        }

        private async void ProcessQueueAsync()
        {
            await Task.Factory.StartNew(ProcessQueue);
        }

        private void ProcessQueue()
        {
            Thread.Sleep(200); // Allow time for other notifications to arrive.
            NotificationArgs notification = null;
            try
            {
                while (notifications.Any())
                {
                    notification = notifications.Dequeue();
                    Debug.WriteLine("Processing notification: {0} - {1}", notification.Title, notification.Detail);

                    List<Task<string>> audioRequestTasks = new();

                    if (!notification.Suppression.HasFlag(NotificationSuppression.Title)
                        && !string.IsNullOrWhiteSpace(notification.Title))
                    {
                        audioRequestTasks.Add(string.IsNullOrWhiteSpace(notification.TitleSsml)
                            ? RetrieveAudioToFile(notification.Title)
                            : RetrieveAudioSsmlToFile(notification.TitleSsml));
                    }

                    if (!notification.Suppression.HasFlag(NotificationSuppression.Detail)
                        && !string.IsNullOrWhiteSpace(notification.Detail))
                    {
                        audioRequestTasks.Add(string.IsNullOrWhiteSpace(notification.DetailSsml)
                            ? RetrieveAudioToFile(notification.Detail)
                            : RetrieveAudioSsmlToFile(notification.DetailSsml));
                    }

                    if (audioRequestTasks.Count > 0)
                        PlayAudioRequestsSequentially(audioRequestTasks);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to fetch/play notification: {notification?.Title} - {notification?.Detail}");
                ErrorLogger(ex, "while retrieving and playing audio for a notification");
            }
            finally
            {
                processing = false;
            }
        }

        private async Task<string> RetrieveAudioToFile(string text)
        {
            return await RetrieveAudioSsmlToFile($"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">{System.Security.SecurityElement.Escape(text)}</voice></speak>");
        }

        private async Task<string> RetrieveAudioSsmlToFile(string ssml)
        {
            return await speechManager.GetAudioFileFromSsml(ssml, voice, style, rate);
        }

        private async void PlayAudioRequestsSequentially(List<Task<string>> requestTasks)
        {
            foreach (var request in requestTasks)
            {
                string file = null;
                try
                {
                    file = request.Result;
                    Debug.WriteLine($"Playing audio file: {file}");
                    core.PlayAudioFile(file);
                }
                catch (Exception ex)
                {
                    if (file != null)
                    {
                        Debug.WriteLine($"Failed to play {file}: {ex.Message}");
                        ErrorLogger(ex, $"while playing: {file}");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to play audio file due to server error when retrieving file: {ex.Message}");
                        ErrorLogger(ex, $"while retrieving audio file");
                    }
                }
            }
            speechManager.CommitCache();
        }
    }
}
