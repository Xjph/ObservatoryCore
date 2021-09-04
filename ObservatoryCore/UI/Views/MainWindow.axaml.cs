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
            Position = new PixelPoint(Properties.Core.Default.MainWindowPosition.X, Properties.Core.Default.MainWindowPosition.Y);
            Closing += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                var size = new System.Drawing.Size((int)System.Math.Round(Width), (int)System.Math.Round(Height));
                var position = new System.Drawing.Point(Position.X, Position.Y);
                Properties.Core.Default.MainWindowSize = size;
                Properties.Core.Default.MainWindowPosition = position;
                Properties.Core.Default.Save();
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
