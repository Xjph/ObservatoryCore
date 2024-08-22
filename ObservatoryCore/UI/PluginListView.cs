using System.Collections;
using static System.Windows.Forms.ListViewItem;
using System.Text;

namespace Observatory.UI
{
    internal class PluginListView : ListView
    {
        private bool _suspend = false;

        public PluginListView()
        {
            View = View.Details;
            OwnerDraw = true;
            GridLines = false;
            base.GridLines = false;//We should prevent the default drawing of gridlines.
            DrawItem += PluginListView_DrawItem;
            DrawSubItem += PluginListView_DrawSubItem;
            DrawColumnHeader += PluginListView_DrawColumnHeader;

            FullRowSelect = true;
            MultiSelect = true;
            KeyDown += PluginListView_KeyDown;

            // Workaround win32 Listview owner draw bug mentioned here: https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.listview.drawitem?view=windowsdesktop-8.0&redirectedfrom=MSDN
            // From https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.listview.ownerdraw?view=windowsdesktop-8.0
            // This manifests if full-row select is enabled.
            MouseMove += PluginListView_MouseMove;
            Invalidated += PluginListView_Invalidated;
            
            DoubleBuffered = true;
        }

        // Stash for performance when doing large UI updates.
        private IComparer? comparer = null;

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

        private void DrawBorder(Graphics graphics, Pen pen, Rectangle bounds, bool header = false)
        {
            if (!_suspend)
            {
                Point topRight = new(bounds.Right, bounds.Top);
                Point bottomRight = new(bounds.Right, bounds.Bottom);

                graphics.DrawLine(pen, topRight, bottomRight);

                if (header)
                {
                    Point bottomLeft = new(bounds.Left, bounds.Bottom);
                    // Point topLeft = new(bounds.Left, bounds.Top);
                    // graphics.DrawLine(pen, topLeft, topRight);
                    // graphics.DrawLine(pen, topLeft, bottomLeft);
                    graphics.DrawLine(pen, bottomLeft, bottomRight);
                }
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
                using (var font = new Font(this.Font, FontStyle.Bold))
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
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Pen pen = new(new SolidBrush(Color.LightGray));
                DrawBorder(g, pen, e.Bounds, false);

                e.DrawText();
            }
        }

        private void PluginListView_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            if (!_suspend)
            {
                var offsetColor = (Color color, bool isEven, bool isSelected) =>
                {
                    if (isEven && !isSelected) return color;

                    int offset = (isEven ? 0 : 20) + (isSelected ? 50 : 0);
                    var r = color.R + (color.R > 127 ? -1 * offset : offset);
                    var g = color.G + (color.G > 127 ? -1 * offset : offset);
                    var b = color.B + (color.B > 127 ? -1 * offset : offset);

                    return Color.FromArgb(r, g, b);
                };

                using var g = e.Graphics;
                e.Item.BackColor = offsetColor(BackColor, e.ItemIndex % 2 == 0, e.Item.Selected);

                if (g != null)
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    Pen pen = new(new SolidBrush(Color.LightGray));
                    e.DrawBackground();
                    e.DrawFocusRectangle();
                }
            }
        }

        private void PluginListView_Invalidated(object? sender, InvalidateEventArgs e)
        {
            foreach (ListViewItem item in Items)
            {
                if (item == null) return;
                item.Tag = null;
            }
        }

        private void PluginListView_MouseMove(object? sender, MouseEventArgs e)
        {
            ListViewItem item = GetItemAt(e.X, e.Y);
            if (item != null && item.Tag == null)
            {
                Invalidate(item.Bounds);
                item.Tag = "invalidated";
            }
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
    }
}
