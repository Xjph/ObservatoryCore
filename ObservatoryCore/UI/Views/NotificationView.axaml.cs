using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using System;
using System.Runtime.InteropServices;

namespace Observatory.UI.Views
{
    public partial class NotificationView : Window
    {
        public NotificationView()
        {
            InitializeComponent();
            SystemDecorations = SystemDecorations.None;
            ShowInTaskbar = false;
            MakeClickThrough(); //Platform specific, currently windows only.
            
            int screen = Properties.Core.Default.NativeNotifyScreen;
            int corner = Properties.Core.Default.NativeNotifyCorner;
            string font = Properties.Core.Default.NativeNotifyFont;
            double scale = Properties.Core.Default.NativeNotifyScale / 100.0;

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

            var textPanel = this.Find<StackPanel>("TextPanel");
            Width *= scale;
            Height *= scale;
            textPanel.Width *= scale;
            textPanel.Height *= scale;

            var textBorder = this.Find<Border>("TextBorder");
            textBorder.BorderThickness *= scale;
            
            PixelRect screenBounds;

            if (screen == -1 || screen > Screens.All.Count)
                screenBounds = Screens.Primary.Bounds;
            else
                screenBounds = Screens.All[screen - 1].Bounds;

            double displayScale = LayoutHelper.GetLayoutScale(this);
            double scaleWidth = Width * displayScale;
            double scaleHeight = Height * displayScale;

            switch (corner)
            {
                default: case 0: 
                    Position = screenBounds.BottomRight - new PixelPoint((int)scaleWidth + 50, (int)scaleHeight + 50);
                    break;
                case 1:
                    Position = screenBounds.BottomLeft - new PixelPoint(-50, (int)scaleHeight + 50);
                    break;
                case 2:
                    Position = screenBounds.TopRight - new PixelPoint((int)scaleWidth + 50, -50);
                    break;
                case 3:
                    Position = screenBounds.TopLeft + new PixelPoint(50, 50);
                    break;
            }

            var timer = new System.Timers.Timer();
            timer.Elapsed += CloseNotification;
            timer.Interval = Properties.Core.Default.NativeNotifyTimeout;
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
