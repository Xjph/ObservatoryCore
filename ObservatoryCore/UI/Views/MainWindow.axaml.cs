using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace Observatory.UI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Height = Properties.Core.Default.MainWindowSize.Height;
            Width = Properties.Core.Default.MainWindowSize.Width;
            
            var savedPosition = new System.Drawing.Point(Properties.Core.Default.MainWindowPosition.X, Properties.Core.Default.MainWindowPosition.Y);
            if (PointWithinDesktopWorkingArea(savedPosition))
                Position = new PixelPoint(Properties.Core.Default.MainWindowPosition.X, Properties.Core.Default.MainWindowPosition.Y);
            
            Closing += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                var size = new System.Drawing.Size((int)System.Math.Round(Width), (int)System.Math.Round(Height));
                Properties.Core.Default.MainWindowSize = size;
                
                var position = new System.Drawing.Point(Position.X, Position.Y);
                if (PointWithinDesktopWorkingArea(position))
                    Properties.Core.Default.MainWindowPosition = position;

                Properties.Core.Default.Save();
            };
        }

        private bool PointWithinDesktopWorkingArea(System.Drawing.Point position)
        {
            bool inBounds = false;
            
            foreach (var screen in Screens.All)
            {
                if (screen.WorkingArea.TopLeft.X <= position.X
                    && screen.WorkingArea.TopLeft.Y <= position.Y
                    && screen.WorkingArea.BottomRight.X > position.X
                    && screen.WorkingArea.BottomRight.Y > position.Y)
                {
                    inBounds = true;
                    break;
                }
            }
            
            return inBounds;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
