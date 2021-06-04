using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Observatory.UI.Views
{
    public partial class NotificationView : Window
    {
        public NotificationView()
        {
            InitializeComponent();
            SystemDecorations = SystemDecorations.None;
            var screenBounds = Screens.Primary.Bounds;
            Position = screenBounds.BottomRight - new PixelPoint((int)Width, (int)Height);
            var timer = new System.Timers.Timer();
            timer.Elapsed += CloseNotification;
            timer.Interval = 5000;
            timer.Start();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void CloseNotification(object sender, System.Timers.ElapsedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                Close();
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
