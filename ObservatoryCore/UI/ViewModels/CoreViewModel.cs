using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Avalonia.Controls;
using Observatory.Framework.Interfaces;
using Observatory.UI.Models;
using ReactiveUI;

namespace Observatory.UI.ViewModels
{
    public class CoreViewModel : ViewModelBase
    {
        private readonly ObservableCollection<IObservatoryNotifier> notifiers;
        private readonly ObservableCollection<IObservatoryWorker> workers;
        private readonly ObservableCollection<CoreModel> tabs;
        private string toggleButtonText;
        private bool _UpdateAvailable;
        
        public CoreViewModel(IEnumerable<(IObservatoryWorker plugin, PluginManagement.PluginManager.PluginStatus signed)> workers, IEnumerable<(IObservatoryNotifier plugin, PluginManagement.PluginManager.PluginStatus signed)> notifiers)
        {
            _UpdateAvailable = CheckUpdate();
            
            this.notifiers = new ObservableCollection<IObservatoryNotifier>(notifiers.Select(p => p.plugin));
            this.workers = new ObservableCollection<IObservatoryWorker>(workers.Select(p => p.plugin));
            ToggleButtonText = "Start Monitor";
            tabs = new ObservableCollection<CoreModel>();
            
            foreach(var worker in workers.Select(p => p.plugin))
            {
                if (worker.PluginUI.PluginUIType == Framework.PluginUI.UIType.Basic)
                {
                    CoreModel coreModel = new();
                    coreModel.Name = worker.ShortName;
                    coreModel.UI = new BasicUIViewModel(worker.PluginUI.DataGrid)
                    {
                        UIType = worker.PluginUI.PluginUIType
                    };
                    
                    tabs.Add(coreModel);
                }
            }

            foreach(var notifier in notifiers.Select(p => p.plugin))
            {
                Panel notifierPanel = new Panel();
                TextBlock notifierTextBlock = new TextBlock();
                notifierTextBlock.Text = notifier.Name;
                notifierPanel.Children.Add(notifierTextBlock);
                //tabs.Add(new CoreModel() { Name = notifier.ShortName, UI = (ViewModelBase)notifier.UI });
            }

            
            tabs.Add(new CoreModel() { Name = "Core", UI = new BasicUIViewModel(new ObservableCollection<object>()) { UIType = Framework.PluginUI.UIType.Core } });

        }

        public void ReadAll()
        {
            SetWorkerReadAllState(true);
            LogMonitor.GetInstance.ReadAllJournals();
            SetWorkerReadAllState(false);
        }

        public void ToggleMonitor()
        {
            var logMonitor = LogMonitor.GetInstance;

            if (logMonitor.IsMonitoring())
            {
                logMonitor.Stop();
                ToggleButtonText = "Start Monitor";
            }
            else
            {
                // HACK: Find a better way of suppressing notifications when pre-reading.
                SetWorkerReadAllState(true);
                logMonitor.Start();
                SetWorkerReadAllState(false);
                ToggleButtonText = "Stop Monitor";
            }
        }

        public void OpenGithub()
        {
            ProcessStartInfo githubOpen = new("https://github.com/Xjph/ObservatoryCore");
            githubOpen.UseShellExecute = true;
            Process.Start(githubOpen);
        }

        public void OpenDonate()
        {
            ProcessStartInfo donateOpen = new("https://paypal.me/eliteobservatory");
            donateOpen.UseShellExecute = true;
            Process.Start(donateOpen);
        }

        public void GetUpdate()
        {
            ProcessStartInfo githubOpen = new("https://github.com/Xjph/ObservatoryCore/releases");
            githubOpen.UseShellExecute = true;
            Process.Start(githubOpen);
        }

        public string ToggleButtonText
        {
            get => toggleButtonText;
            set
            {
                if (toggleButtonText != value)
                {
                    toggleButtonText = value;
                    this.RaisePropertyChanged(nameof(ToggleButtonText));
                }
            }
        }

        public ObservableCollection<IObservatoryWorker> Workers
        {
            get { return workers; }
        }

        public ObservableCollection<IObservatoryNotifier> Notifiers
        {
            get { return notifiers; }
        }

        public ObservableCollection<CoreModel> Tabs
        {
            get { return tabs; }
        }

        private void SetWorkerReadAllState(bool isReadingAll)
        {
            foreach (var worker in workers)
            {
                if (isReadingAll)
                {
                    worker.ReadAllStarted();
                }
                else
                {
                    worker.ReadAllFinished();
                }
            }
        }

        private bool CheckUpdate()
        {
            try
            {
                string releasesResponse;

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.github.com/repos/xjph/ObservatoryCore/releases"),
                    Headers = { { "User-Agent", "Xjph/ObservatoryCore" } }
                };

                releasesResponse = HttpClient.SendRequest(request).Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(releasesResponse))
                {
                    var releases = System.Text.Json.JsonDocument.Parse(releasesResponse).RootElement.EnumerateArray();

                    foreach (var release in releases)
                    {
                        if (release.GetProperty("tag_name").ToString().CompareTo("v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()) > 0)
                        {
                            return true;
                        }
                    }
                }

            }
            catch
            {
                return false;
            }

            return false;
        }

        private bool UpdateAvailable
        {
            get => _UpdateAvailable;
            set
            {
                this.RaiseAndSetIfChanged(ref _UpdateAvailable, value);
            }
        }

    }
}
