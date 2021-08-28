using System;
using Avalonia;
using Avalonia.ReactiveUI;

namespace Observatory
{
    class ObservatoryCore
    {
        [STAThread]
        static void Main(string[] args)
        {
            string version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            if (Properties.Core.Default.CoreVersion != version)
            {
                Properties.Core.Default.Upgrade();
                Properties.Core.Default.CoreVersion = version;
                Properties.Core.Default.Save();
            }
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<UI.MainApplication>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}
