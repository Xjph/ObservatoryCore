using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observatory.Framework.Interfaces;

namespace Observatory.UI
{
    internal class DefaultSorter : IObservatoryComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private int OrderOfSort;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public DefaultSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = 0;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object? x, object? y)
        {
            int compareResult;

            ListViewItem? listviewX = (ListViewItem?)x;
            ListViewItem? listviewY = (ListViewItem?)y;
                                    
            if (OrderOfSort == 0)
                return 0;

            // Compare the two items
            compareResult = NaturalCompare(listviewX?.SubItems[ColumnToSort].Text, listviewY?.SubItems[ColumnToSort].Text);

            // Calculate correct return value based on object comparison
            if (OrderOfSort == 1)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else 
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
        }

        private static int NaturalCompare(string? x, string? y)
        {
            for (int i = 0; i <= x?.Length && i <= y?.Length; i++)
            {
                // If we've reached the end of the string without finding a difference
                // the longer string is "greater".
                if (i == x.Length || i == y.Length)
                    return x.Length > y.Length ? 1 : y.Length > x.Length ? -1 : 0;

                // We've found a number in the same place in both strings.
                if (Char.IsDigit(x[i]) && Char.IsDigit(y[i]))
                {
                    // Walk ahead and get the full numbers.
                    string xNum = new(x[i..].TakeWhile(c => Char.IsDigit(c)).ToArray());
                    string yNum = new(y[i..].TakeWhile(c => Char.IsDigit(c)).ToArray());

                    // Pad with zeroes to equal lengths.
                    int numLength = Math.Max(xNum.Length, yNum.Length);
                    string xNumPadded = xNum.PadLeft(numLength, '0');
                    string yNumPadded = yNum.PadLeft(numLength, '0');

                    // Now that they're the same length a direct compare works.
                    int result = xNumPadded.CompareTo(yNumPadded);
                    if (result != 0)
                    {
                        return result;
                    }
                    else
                    {
                        // The numbers are identical, skip them and keep moving.
                        i += numLength - 1;
                    }
                }
                // Check if we have unequal letters.
                else if (x[i] != y[i])
                {
                    // Straight compare and return.
                    return x[i] > y[i] ? 1 : -1;
                }
            }

            // If we somehow make it here, return equal result.
            return 0;
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public int Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }
    }
}
