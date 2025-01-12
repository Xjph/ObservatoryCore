using System.Runtime.InteropServices;

namespace Observatory.UI
{
    public class ColourableTabControl : TabControl
    {
        private TabPage? _draggedPage;

        public Color TabColor { get; set; }
        public Color SelectedTabColor { get; set; }
        public override Color BackColor { get; set; }

        public ColourableTabControl() : base()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
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

        // Tab reordering from 
        // https://stackoverflow.com/questions/4352781/is-it-possible-to-make-the-winforms-tab-control-be-able-to-do-tab-reordering-lik/11361257#11361257

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            _draggedPage = TabAt(e.Location);
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _draggedPage == null)
            {
                return;
            }

            TabPage? tab = TabAt(e.Location);

            if (tab == null || tab == _draggedPage)
            {
                return;
            }

            Swap(_draggedPage, tab);
            SelectedTab = _draggedPage;
        }

        private TabPage? TabAt(Point position)
        {
            int count = TabCount;

            for (int i = 0; i < count; i++)
            {
                if (GetTabRect(i).Contains(position))
                {
                    return TabPages[i];
                }
            }

            return null;
        }

        private void Swap(TabPage a, TabPage b)
        {
            int i = TabPages.IndexOf(a);
            int j = TabPages.IndexOf(b);
            TabPages[i] = b;
            TabPages[j] = a;
        }

    }
}
