using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Observatory.UI.ViewModels;

namespace Observatory.UI
{
    public class MainApplication : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var pluginManager = PluginManagement.PluginManager.GetInstance;
                desktop.MainWindow = new Views.MainWindow()
                {
                    DataContext = new MainWindowViewModel(pluginManager)
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
