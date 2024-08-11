using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Sorters
{
    public class NoOpColumnSorter : IObservatoryComparer
    {
        public int SortColumn { get; set; }
        public int Order { get; set; }

        public int Compare(object x, object y)
        {
            return 0;
        }
    }
}
