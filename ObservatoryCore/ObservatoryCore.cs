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
            try
            {
                if (Properties.Core.Default.CoreVersion != version)
                {
                    Properties.Core.Default.Upgrade();
                    Properties.Core.Default.CoreVersion = version;
                    Properties.Core.Default.Save();
                }
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                LogError(ex, version);
            }
        }

        internal static void LogError(Exception ex, string context)
        {
            var docPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var errorMessage = new System.Text.StringBuilder();
            var timestamp = DateTime.Now.ToString("G");
            errorMessage
                .AppendLine($"[{timestamp}] Error encountered in Elite Observatory {context}")
                .AppendLine(FormatExceptionMessage(ex))
                .AppendLine();
            System.IO.File.AppendAllText(docPath + System.IO.Path.DirectorySeparatorChar + "ObservatoryErrorLog.txt", errorMessage.ToString());
        }

        static string FormatExceptionMessage(Exception ex, bool inner = false)
        {
            var errorMessage = new System.Text.StringBuilder();
            errorMessage
                .AppendLine($"{(inner ? "Inner e" : "E")}xception message: {ex.Message}")
                .AppendLine($"Stack trace:")
                .AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
                errorMessage.AppendLine(FormatExceptionMessage(ex.InnerException, true));
            
            return errorMessage.ToString();
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
