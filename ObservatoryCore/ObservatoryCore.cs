using Observatory.PluginManagement;
using Observatory.Utils;
using System.Reflection.PortableExecutable;

namespace Observatory
{
    internal static class ObservatoryCore
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            SettingsManager.Load();

            if (args.Length > 0 && File.Exists(args[0]))
            {
                var fileInfo = new FileInfo(args[0]);
                if (fileInfo.Extension == ".eop" || fileInfo.Extension == ".zip")
                    File.Copy(
                        fileInfo.FullName,
                         $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}{fileInfo.Name}");
            }

            string version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0";
            try
            {
                if (Properties.Core.Default.CoreVersion != version)
                {
                    try
                    {
                        // Properties.Core.Default.Upgrade();
                    }
                    catch 
                    {
                        // Silently ignore properties upgrade failure.
                    }
                    Properties.Core.Default.CoreVersion = version;
                    SettingsManager.Save();
                }

                

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new UI.CoreForm());
                PluginManagement.PluginManager.GetInstance.Shutdown();
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
            System.IO.File.AppendAllText(docPath + System.IO.Path.DirectorySeparatorChar + "ObservatoryCrashLog.txt", errorMessage.ToString());
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
    }
}