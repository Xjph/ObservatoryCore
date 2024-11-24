using Observatory.Framework.Interfaces;
using Observatory.Framework;
using Observatory.Utils;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Dynamic;
using static System.Windows.Forms.ListViewItem;
using System.Text;

namespace Observatory.UI
{
    internal partial class PluginUIGrid : ListView
    {
        private bool _suspend = false;
        private Dictionary<Guid, List<ListViewItem>> _groupedItems = [];
        private IObservatoryComparer _columnSorter;
        private List<ColumnSizing> _columnSizing;
        private ColumnSizing _pluginColumnSizing;
        private bool _selectionInProgress = false;

        public PluginUIGrid(IObservatoryPlugin plugin, List<ColumnSizing> columnSizings)
        {
            View = View.Details;
#if PROTON
            GridLines = true;
#else
            OwnerDraw = true;
            GridLines = false;
            DrawSubItem += PluginListView_DrawSubItem;
            // DrawItem += PluginListView_DrawItem;
            DrawColumnHeader += PluginListView_DrawColumnHeader;
#endif
            DoubleBuffered = true;
            
            _columnSizing = columnSizings;

            FullRowSelect = true;
            MultiSelect = true;
            KeyDown += PluginListView_KeyDown;

            if (plugin.ColumnSorter != null)
                _columnSorter = plugin.ColumnSorter;
            else
                _columnSorter = new DefaultSorter();

            ColumnClick += PluginListView_ColumnClick;

            // Is losing column sizes between versions acceptable?
            _pluginColumnSizing = _columnSizing
                .Where(c => c.PluginName == plugin.Name && c.PluginVersion == plugin.Version)
                .FirstOrDefault(new ColumnSizing() { PluginName = plugin.Name, PluginVersion = plugin.Version });

            if (!_columnSizing.Contains(_pluginColumnSizing))
            {
                _columnSizing.Add(_pluginColumnSizing);
            }

            // Some plugins don't provide any columns?
            if (plugin.PluginUI.DataGrid.Any())
            foreach (var property in plugin.PluginUI.DataGrid.First().GetType().GetProperties())
            {
                string columnLabel = LowerUpper().Replace(UpperUpperLower().Replace(property.Name, "$1 $2"), "$1 $2");

                int width;

                if (_pluginColumnSizing.ColumnWidth.TryGetValue(columnLabel, out int value))
                {
                    width = value;
                }
                else
                {
                    var widthAttrib = property.GetCustomAttribute<ColumnSuggestedWidth>();

                    width = widthAttrib == null
                        // Rough approximation of width by label length if none specified.
                        ? columnLabel.Length * 10
                        : widthAttrib.Width;

                    _pluginColumnSizing.ColumnWidth.Add(columnLabel, width);
                }

                Columns.Add(columnLabel, width);

            }

            Properties.Core.Default.ColumnSizing = JsonSerializer.Serialize(_columnSizing);
            SettingsManager.Save();

            ColumnWidthChanged += PluginListView_ColumnWidthChanged;
            Resize += PluginListView_Resize;

            ConcurrentQueue<object> addedItemList = new();
            var timer = new System.Timers.Timer
            {
                Interval = 100,
                AutoReset = false
            };

            timer.Elapsed += (_, _) =>
            {
                List<ListViewItem> items = [];

                ListViewItem? finalItem = null;

                while (addedItemList.TryDequeue(out object? newItem))
                {
                    ListViewItem newListItem = new();
                    if (newItem.GetType() == typeof(ExpandoObject))
                    {
                        dynamic groupedItem = (ExpandoObject)newItem;
                        
                        foreach (KeyValuePair<string, object> property in groupedItem)
                        {
                            if (property.Key != "ObservatoryListViewGroupID")
                            newListItem.SubItems.Add(property.Value?.ToString());
                        }

                        Guid groupId = groupedItem.ObservatoryListViewGroupID;

                        if (_groupedItems.ContainsKey(groupId))
                        {
                            _groupedItems[groupId].Add(newListItem);
                        }
                        else
                        {
                            _groupedItems.Add(groupId, [newListItem]);
                        }
                    }
                    else
                    foreach (var property in newItem.GetType().GetProperties())
                    {
                        newListItem.SubItems.Add(property.GetValue(newItem)?.ToString());
                    }
                    newListItem.SubItems.RemoveAt(0);
                    items.Add(newListItem);
                    finalItem = newListItem;
                }

                var addItemsAndScroll = () =>
                {
                    Items.AddRange(items.ToArray());
                    if (finalItem != null
                    && (LogMonitor.GetInstance.CurrentState & LogMonitorState.Batch) != LogMonitorState.Batch)
                        EnsureVisible(Items.IndexOf(finalItem));
                };

                if (Created)
                {
                    Invoke(addItemsAndScroll);
                }
                else
                {
                    addItemsAndScroll();
                }

                timer.Stop();

                // Possible that something was added between last dequeue and timer.Stop()
                if (addedItemList.TryPeek(out _))
                    timer.Start();
            };


            plugin.PluginUI.DataGrid.CollectionChanged += (sender, e) =>
            {
                void updateGrid()
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
                    {
                        timer.Stop();
                        foreach (var item in e.NewItems)
                            addedItemList.Enqueue(item);

                        timer.Start();
                    }

                    if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            ListViewItem oldListItem = new();
                            foreach (var property in oldItem.GetType().GetProperties())
                            {
                                oldListItem.SubItems.Add(property.GetValue(oldItem)?.ToString());
                            }
                            oldListItem.SubItems.RemoveAt(0);

                            var itemToRemove = Items.Cast<ListViewItem>().Where(i => i.SubItems.Cast<string>().SequenceEqual(oldListItem.SubItems.Cast<string>())).First();
                            if (itemToRemove != null)
                            {
                                Items.Remove(itemToRemove);
                            }
                        }
                    }

                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        timer.Stop();
                        Items.Clear();
                        addedItemList.Clear();
                        foreach (var item in plugin.PluginUI.DataGrid)
                        {
                            ListViewItem listItem = new();
                            foreach (var property in item.GetType().GetProperties())
                            {
                                listItem.SubItems.Add(property.GetValue(item)?.ToString());
                            }
                            listItem.SubItems.RemoveAt(0);
                            Items.Add(listItem);
                        }
                    }
                }

                if (Created)
                {
                    Invoke(updateGrid);
                }
                else
                {
                    updateGrid();
                }
            };
        }

        

        private void PluginListView_Resize(object? sender, EventArgs e)
        {
            HandleColSize(false);
        }

        private void PluginListView_ColumnWidthChanged(object? sender, ColumnWidthChangedEventArgs e)
        {
            HandleColSize(true);
        }

        // Oddly, the listview resize event often fires after the column size change but
        // with stale (default?!) column width values.
        // Still need a resize handler to avoid the ugliness of the rightmost column
        // leaving gaps, but preventing saving the width changes there should stop the
        // stale resize event from overwriting with bad data.
        // Using a higher-order function here to create two different versions of the
        // event handler for these purposes.
        private void HandleColSize(bool saveProps)
        {
            int colTotalWidth = 0;
            ColumnHeader? rightmost = null;
            foreach (ColumnHeader column in Columns)
            {
                colTotalWidth += column.Width;
                if (rightmost == null || column.DisplayIndex > rightmost.DisplayIndex)
                    rightmost = column;

                if (saveProps)
                {
                    if (_pluginColumnSizing.ColumnWidth.ContainsKey(column.Text))
                        _pluginColumnSizing.ColumnWidth[column.Text] = column.Width;
                    else
                        _pluginColumnSizing.ColumnWidth.Add(column.Text, column.Width);
                }
            }

            if (rightmost != null && colTotalWidth < Width)
            {
                rightmost.Width = Width - (colTotalWidth - rightmost.Width);

                if (saveProps)
                    _pluginColumnSizing.ColumnWidth[rightmost.Text] = rightmost.Width;
            }

            if (saveProps)
            {
                Properties.Core.Default.ColumnSizing = JsonSerializer.Serialize(_columnSizing);
                SettingsManager.Save();
            }
        }

        // Stash for performance when doing large UI updates.
        private IComparer? comparer = null;

        private void PluginListView_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
            SuspendDrawing();
            
            if (e.Column == _columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (_columnSorter.Order == 1)
                {
                    _columnSorter.Order = -1;
                }
                else
                {
                    _columnSorter.Order = 1;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _columnSorter.SortColumn = e.Column;
                _columnSorter.Order = 1;
            }

            if (_groupedItems.Count > 0)
            {
                // Should sort only on first row, remove following rows for now
                // Believe it or not casting to a list here is faster than operating 
                // directly from ListView.Items
                ArrayList itemList = [];
                itemList.AddRange(Items);//.Cast<ListViewItem>().ToList();

                foreach (var group in _groupedItems)
                {
                    foreach (var item in group.Value.Skip(1))
                        itemList.Remove(item);
                }

                Items.Clear();
                itemList.Sort(_columnSorter);

                List<ListViewItem> areYouKiddingMeThatAllThisIsFaster = [];
                foreach (ListViewItem sortedItem in itemList)
                {
                    areYouKiddingMeThatAllThisIsFaster.Add(sortedItem);

                    var group = _groupedItems.Where(g => g.Value.First() == sortedItem);

                    if (group.Any())
                    foreach (var item in _groupedItems.Where(g => g.Value.First() == sortedItem).First().Value.Skip(1).Reverse())
                    {
                        areYouKiddingMeThatAllThisIsFaster.Add(item);
                    }
                }
                
                Items.AddRange(areYouKiddingMeThatAllThisIsFaster.ToArray());
            }
            else
            {
                ListViewItemSorter = _columnSorter;
                Sort();
                ListViewItemSorter = null;
            }
            
            ResumeDrawing();
        }

        public void SuspendDrawing()
        {
            _suspend = true;
            BeginUpdate();
            comparer = ListViewItemSorter;
        }

        public void ResumeDrawing()
        {
            if (comparer != null)
            {
                ListViewItemSorter = comparer;
                comparer = null;
            }
            _suspend = false;
            EndUpdate();
        }

        private void DrawBorder(Graphics graphics, Pen pen, Rectangle bounds)
        {
            if (!_suspend)
            {
                // Right bound tweaking to avoid being overdrawn by adjacent subitem background
                // Top bound tweaking to intentionally overdraw above subitem background
                Point topRight = new(bounds.Right - 1, Math.Max(bounds.Top - 1, 0));
                Point bottomRight = new(bounds.Right - 1, bounds.Bottom);
                
                graphics.DrawLine(pen, topRight, bottomRight);
            }
        }

        private void PluginListView_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            using var g = e.Graphics;
            if (!_suspend && g != null)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Pen pen = new(new SolidBrush(Color.LightGray));
                DrawBorder(g, pen, e.Bounds);
                using (var font = new Font(Font, FontStyle.Bold))
                {
                    Brush textBrush = new SolidBrush(ForeColor);
                    g.DrawString(e.Header?.Text, font, textBrush, e.Bounds);
                }
            }
        }

        private void PluginListView_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            using var g = e.Graphics;
            if (!_suspend && g != null)
            {
                Pen pen = new(new SolidBrush(Color.LightGray));
                
                if (e.Item != null)
                {
                    if (_groupedItems.Count == 0)
                    {
                        e.Item.BackColor = AlternatedColor(BackColor, e.Item.Index % 2 == 0);
                    }
                    else
                    {
                        var currentGroupedItems = new Dictionary<Guid, List<ListViewItem>>(_groupedItems);
                        var firstLines = currentGroupedItems.Values.Select(v => v.First()).ToList();
                        var thisGroup = currentGroupedItems.Values.Where(v => v.Contains(e.Item)).FirstOrDefault([e.Item]);
                        
                        if (firstLines.Contains(e.Item) || !currentGroupedItems.Values.Where(v => v.Contains(e.Item)).Any())
                        {
                            e.Item.BackColor = AlternatedColor(
                                BackColor,
                                e.Item.Index == 0 || Items[e.Item.Index - 1].BackColor != BackColor);
                        }
                        else
                        {
                            e.Item.BackColor = thisGroup.First().BackColor;
                        }
                    }

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var backColor = SelectedColor(e.Item.BackColor, e.Item.Selected);
                    using var backBrush = new SolidBrush(backColor);
                    g.FillRectangle(backBrush, e.Bounds);

                    e.DrawText();
                    e.DrawFocusRectangle(e.Item.Bounds);
                    DrawBorder(g, pen, e.Bounds);
                }
            }
        }

        private Color AlternatedColor(Color color, bool isEven)
        {
            if (isEven) return color;
            int offset = (isEven ? 0 : 20);
            var r = color.R + (color.R > 127 ? -1 * offset : offset);
            var g = color.G + (color.G > 127 ? -1 * offset : offset);
            var b = color.B + (color.B > 127 ? -1 * offset : offset);
            return Color.FromArgb(r, g, b);
        }

        private Color SelectedColor(Color color, bool isSelected)
        {
            if (!isSelected) return color;
            int offset = (isSelected ? 50 : 0);
            var r = color.R + (color.R > 127 ? -1 * offset : offset);
            var g = color.G + (color.G > 127 ? -1 * offset : offset);
            var b = color.B + (color.B > 127 ? -1 * offset : offset);
            return Color.FromArgb(r, g, b);
        }

        private void PluginListView_KeyDown(object? sender, KeyEventArgs e)
        {
            if (SelectedItems.Count == 0)
            {
                e.Handled = false;
                return;
            }

            if (e.KeyCode == Keys.C && e.Control & !e.Alt & !e.Shift)
            {
                StringBuilder sb = new StringBuilder();
                foreach (int index in SelectedIndices)
                {
                    foreach (ListViewSubItem subItem in Items[index].SubItems)
                    {
                        sb.Append(subItem.Text);
                        sb.Append('\t');
                    }
                    sb.AppendLine();
                }
                Clipboard.SetText(sb.ToString());
                e.Handled = true;
            }
        }

        // https://stackoverflow.com/questions/5796383/insert-spaces-between-words-on-a-camel-cased-token
        [GeneratedRegex(@"(\p{Ll})(\P{Ll})")]
        private static partial Regex LowerUpper();
        [GeneratedRegex(@"(\P{Ll})(\P{Ll}\p{Ll})")]
        private static partial Regex UpperUpperLower();
    }
}
