using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;

namespace Observatory.UI.Views
{
    public class CoreView : UserControl
    {
        public CoreView()
        {
            InitializeComponent();
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
