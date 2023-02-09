using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework
{
    public class BasicGrid
    {
        public BasicGrid()
        {
            Headers = new();
            Formats = new();
            Items = new();
        }

        public readonly ObservableCollection<string> Headers;
        public readonly ObservableCollection<string> Formats;
        public readonly ObservableCollection<ObservableCollection<object>> Items;
    }
}
