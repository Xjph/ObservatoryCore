﻿using System.Collections;

namespace Observatory.UI
{
    internal class PluginListView : ListView
    {
        private bool _suspend = false;

        public PluginListView()
        {
            OwnerDraw = true;
            GridLines = false;
            DrawItem += PluginListView_DrawItem;
            DrawSubItem += PluginListView_DrawSubItem;
            DrawColumnHeader += PluginListView_DrawColumnHeader;
            
            
            DoubleBuffered = true;
            base.GridLines = false;//We should prevent the default drawing of gridlines.
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
                var offsetColor = (int value) =>
                {
                    if (value > 127)
                    {
                        return value - 20;
                    }
                    else
                    {
                        return value + 20;
                    }
                };

                using var g = e.Graphics;
                if (e.ItemIndex % 2 == 0)
                {
                    e.Item.BackColor = BackColor;
                }
                else
                {
                    e.Item.BackColor = Color.FromArgb(offsetColor(BackColor.R), offsetColor(BackColor.G), offsetColor(BackColor.B));
                }

                if (g != null)
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    Pen pen = new(new SolidBrush(Color.LightGray));
                    e.DrawBackground();
                }
            }
        }
    }
}