using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
        
        public CoreViewModel(IEnumerable<(IObservatoryWorker plugin, PluginManagement.PluginManager.PluginStatus signed)> workers, IEnumerable<(IObservatoryNotifier plugin, PluginManagement.PluginManager.PluginStatus signed)> notifiers)
        {
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
                    coreModel.UI = new();
                    var uiViewModel = new BasicUIViewModel(worker.PluginUI.DataGrid)
                    {
                        UIType = worker.PluginUI.PluginUIType
                    };
                    coreModel.UI = uiViewModel;
                    
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
            foreach (var worker in workers)
            {
                worker.ReadAllStarted();
            }
            LogMonitor.GetInstance.ReadAllJournals();
            foreach (var worker in workers)
            {
                worker.ReadAllFinished();
            }
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
                logMonitor.Start();
                ToggleButtonText = "Stop Monitor";
            }
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
    }
}
