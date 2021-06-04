using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Observatory.UI.Models;
using ReactiveUI;
using System.Reactive.Linq;
using Observatory.Framework;

namespace Observatory.UI.ViewModels
{
    public class BasicUIViewModel : ViewModelBase
    {
        private ObservableCollection<object> basicUIGrid;
        

        public ObservableCollection<object> BasicUIGrid
        {
            get => basicUIGrid;
            set
            {
                basicUIGrid = value;
                this.RaisePropertyChanged(nameof(BasicUIGrid));
            }
        }

        public BasicUIViewModel(ObservableCollection<object> BasicUIGrid)
        {
         
            this.BasicUIGrid = new();
            this.BasicUIGrid = BasicUIGrid;

            //// Create a timer and set a two second interval.
            //var aTimer = new System.Timers.Timer();
            //aTimer.Interval = 2000;

            //// Hook up the Elapsed event for the timer. 
            //aTimer.Elapsed += OnTimedEvent;

            //// Have the timer fire repeated events (true is the default)
            //aTimer.AutoReset = true;

            //// Start the timer
            //aTimer.Enabled = true;
        }

        private PluginUI.UIType uiType;

        public PluginUI.UIType UIType
        {
            get => uiType;
            set
            {
                uiType = value;
                this.RaisePropertyChanged(nameof(UIType));
            }
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            basicUIGrid.Count();
        }
    }
}
