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

        public System.Collections.IList SelectedItems { get; set; }        

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
            this.BasicUIGrid = BasicUIGrid;
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
    }
}
