using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.UI
{
    public class ColourableTabControl : TabControl
    {
        public ColourableTabControl() : base()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
        }

        override protected void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

            for (int i = 0; i < TabPages.Count; i++)
            {
                var tab = TabPages[i];
                var selected = SelectedIndex == i;
                var tabArea = GetTabRect(i);
                var stringFormat = new StringFormat()
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                if (selected)
                {
                    try
                    {
                        e.Graphics.FillRectangle(new SolidBrush(SelectedTabColor), tabArea);
                    }
                    catch (ExternalException ex) // A generic error occurred in GDI+.
                    {
                        // This happens sometimes randomly when resizing things a bunch, but doesn't seem to break anything.
                    }
                    tabArea.Offset(-1, -1);
                }
                else
                {
                    try
                    {
                        e.Graphics.FillRectangle(new SolidBrush(TabColor), tabArea);
                    }
                    catch (ExternalException ex) // A generic error occurred in GDI+.
                    {
                        // This happens sometimes randomly when resizing things a bunch, but doesn't seem to break anything.
                    }
                    tabArea.Offset(1, 1);
                }

                if (Alignment == TabAlignment.Left)
                {
                    stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                }

                e.Graphics.DrawString(tab.Text, Font ?? new Font("Segoe UI", 9), new SolidBrush(ForeColor), tabArea, stringFormat);
            }
        }

        public Color TabColor { get; set; }
        public Color SelectedTabColor { get; set; }

        public override Color BackColor { get; set; }
    }
}
