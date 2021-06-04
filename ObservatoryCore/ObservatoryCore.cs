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
