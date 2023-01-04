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
using System.Collections.Specialized;

namespace Observatory.UI.ViewModels
{
    public class BasicUIViewModel : ViewModelBase
    {
        private ObservableCollection<string> _headers;
        private ObservableCollection<string> _formats;
        private ObservableCollection<ObservableCollection<object>> _items;

        public System.Collections.IList SelectedItems { get; set; }        

        
        public ObservableCollection<string> Headers
        {
            get => _headers;
            set
            {
                _headers = value;
                _headers.CollectionChanged += (o, e) => this.RaisePropertyChanged(nameof(Headers));
                this.RaisePropertyChanged(nameof(Headers));
            }
        }

        public ObservableCollection<string> Formats
        {
            get => _formats;
            set
            {
                _formats = value;
                _formats.CollectionChanged += (o, e) => this.RaisePropertyChanged(nameof(Formats));
                this.RaisePropertyChanged(nameof(Formats));
            }
        }

        public ObservableCollection<ObservableCollection<object>> Items
        {
            get => _items;
            set
            {
                void raiseItemChanged(object o, NotifyCollectionChangedEventArgs e) { this.RaisePropertyChanged(nameof(Items)); }

                _items = value;
                _items.CollectionChanged += raiseItemChanged;
                this.RaisePropertyChanged(nameof(Items));
                foreach (var itemColumn in value)
                {
                    itemColumn.CollectionChanged += raiseItemChanged;
                }
            }
        }


        public BasicUIViewModel(ObservableCollection<string> headers, ObservableCollection<string> formats)
        {
            Headers = headers;
            Formats = formats;
        }

        private PluginUI.UIType _uiType;

        public PluginUI.UIType UIType
        {
            get => _uiType;
            set
            {
                _uiType = value;
                this.RaisePropertyChanged(nameof(UIType));
            }
        }
    }
}
