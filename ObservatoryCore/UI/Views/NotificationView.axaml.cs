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
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
