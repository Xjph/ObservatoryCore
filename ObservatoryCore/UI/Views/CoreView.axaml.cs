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

            var titleBlock = this.Find<TextBlock>("Title");
            titleBlock.Text = "Elite Observatory Core - v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
