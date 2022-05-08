using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Observatory.UI.ViewModels;
using System;
using System.Timers;
using System.Runtime.InteropServices;

namespace Observatory.UI.Views
{
    public partial class NotificationView : Window
    {
        private readonly double scale;
        private readonly Timer timer;
        private readonly Guid guid;
        private bool defaultPosition = true;
        private PixelPoint originalPosition;

        public NotificationView() : this(default)
        { }

        public NotificationView(Guid guid)
        {
            this.guid = guid;
            InitializeComponent();
            SystemDecorations = SystemDecorations.None;
            ShowInTaskbar = false;
            MakeClickThrough(); //Platform specific, currently windows only.


            this.DataContextChanged += NotificationView_DataContextChanged;
            scale = Properties.Core.Default.NativeNotifyScale / 100.0;

            AdjustText();

            AdjustPanel();

            AdjustPosition();

            timer = new();
            timer.Elapsed += CloseNotification;
            timer.Interval = Properties.Core.Default.NativeNotifyTimeout;
            timer.Start();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        public Guid Guid { get => guid; }

        public void AdjustOffset(bool increase)
        {
            if (defaultPosition)
            {
                if (increase || Position != originalPosition)
                {
                    var corner = Properties.Core.Default.NativeNotifyCorner;

                    if ((corner >= 2 && increase) || (corner <= 1 && !increase))
                    {
                        Position += new PixelPoint(0, Convert.ToInt32(Height));
                    }
                    else
                    {
                        Position -= new PixelPoint(0, Convert.ToInt32(Height));
                    }

                }
            }
        }

        private void NotificationView_DataContextChanged(object sender, EventArgs e)
        {
            var notification = ((NotificationViewModel)DataContext).Notification;

            AdjustText();

            AdjustPanel();

            AdjustPosition(notification.XPos / 100, notification.YPos / 100);

            if (notification.Timeout > 0)
            {
                timer.Stop();
                timer.Interval = notification.Timeout;
                timer.Start();
            }
            else if (notification.Timeout == 0)
            {
                timer.Stop();
            }
        }

        private void AdjustText()
        {
            string font = Properties.Core.Default.NativeNotifyFont;
            var titleText = this.Find<TextBlock>("Title");
            var detailText = this.Find<TextBlock>("Detail");

            if (font.Length > 0)
            {
                var fontFamily = new Avalonia.Media.FontFamily(font);

                titleText.FontFamily = fontFamily;
                detailText.FontFamily = fontFamily;
            }

            titleText.FontSize *= scale;
            detailText.FontSize *= scale;
        }

        private void AdjustPanel()
        {
            var textPanel = this.Find<StackPanel>("TextPanel");
            Width *= scale;
            Height *= scale;
            textPanel.Width *= scale;
            textPanel.Height *= scale;

            var textBorder = this.Find<Border>("TextBorder");
            textBorder.BorderThickness *= scale;
        }

        private void AdjustPosition(double xOverride = -1.0, double yOverride = -1.0)
        {
            PixelRect screenBounds;
            int screen = Properties.Core.Default.NativeNotifyScreen;
            int corner = Properties.Core.Default.NativeNotifyCorner;

            if (screen == -1 || screen > Screens.All.Count)
                if (Screens.All.Count == 1)
                    screenBounds = Screens.All[0].Bounds;
                else
                    screenBounds = Screens.Primary.Bounds;
            else
                screenBounds = Screens.All[screen - 1].Bounds;

            double displayScale = LayoutHelper.GetLayoutScale(this);
            double scaleWidth = Width * displayScale;
            double scaleHeight = Height * displayScale;

            if (xOverride >= 0 && yOverride >= 0)
            {
                defaultPosition = false;
                Position = screenBounds.TopLeft + new PixelPoint(Convert.ToInt32(screenBounds.Width * xOverride), Convert.ToInt32(screenBounds.Height * yOverride));
            }
            else
            {
                defaultPosition = true;
                switch (corner)
                {
                    default:
                    case 0:
                        Position = screenBounds.BottomRight - new PixelPoint(Convert.ToInt32(scaleWidth) + 50, Convert.ToInt32(scaleHeight) + 50);
                        break;
                    case 1:
                        Position = screenBounds.BottomLeft - new PixelPoint(-50, Convert.ToInt32(scaleHeight) + 50);
                        break;
                    case 2:
                        Position = screenBounds.TopRight - new PixelPoint(Convert.ToInt32(scaleWidth) + 50, -50);
                        break;
                    case 3:
                        Position = screenBounds.TopLeft + new PixelPoint(50, 50);
                        break;
                }
                originalPosition = Position;
            }
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

        private void MakeClickThrough()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var style = GetWindowLong(this.PlatformImpl.Handle.Handle, GWL_EXSTYLE);

                //PlatformImpl not part of formal Avalonia API and may not be available in future versions.
                SetWindowLong(this.PlatformImpl.Handle.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
                SetLayeredWindowAttributes(this.PlatformImpl.Handle.Handle, 0, 255, LWA_ALPHA);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        [DllImport("user32.dll")]
        static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        internal const int GWL_EXSTYLE = -20;
        internal const int WS_EX_LAYERED = 0x80000;
        internal const int LWA_ALPHA = 0x2;
        internal const int WS_EX_TRANSPARENT = 0x00000020;
    }
}
