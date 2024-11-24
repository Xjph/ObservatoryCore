using Observatory.Utils;
using System.Diagnostics;

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

            if (args.Length > 0 && args[0].ToLower() == "/nomin")
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Application.ExecutablePath;
                startInfo.WorkingDirectory = Path.GetDirectoryName(startInfo.FileName);
                startInfo.Arguments = "";
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                Process.Start(startInfo);
            }
            else
            {
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

#if !PORTABLE && !PROTON
                    if (Properties.Core.Default.CoreVersion != version)
                    {
                        try
                        {
                            Properties.Core.Default.Upgrade();
                        }
                        catch
                        {
                            // Silently ignore properties upgrade failure.
                        }
                        Properties.Core.Default.CoreVersion = version;
                        SettingsManager.Save();
                    }
#endif

                    // To customize application configuration such as set high DPI settings or default font,
                    // see https://aka.ms/applicationconfiguration.
                    ApplicationConfiguration.Initialize();
                    Application.Run(new UI.CoreForm());

                    PluginManagement.PluginManager.GetInstance.Shutdown();
                }
                catch (Exception ex)
                {
                    try
                    {
                        LogError(ex, version, true);
                    }
                    catch
                    {
                        // Ignore. Errors while logging might mask actual error.
                    }

                    // Re-throw to avoid masking error at OS level.
                    throw;
                }
            }
        }

        internal static void LogError(Exception ex, string context, bool fatal = false)
        {
#if PORTABLE || PROTON
            var docPath = Application.StartupPath;
#else
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#endif
            var errFile = docPath + Path.DirectorySeparatorChar + $"Observatory{(fatal ? "Crash" : "Error")}Log.txt";

            var showMessage = !fatal && File.Exists(errFile) && File.GetLastWriteTime(errFile) < Process.GetCurrentProcess().StartTime;
            
            var errorMessage = new System.Text.StringBuilder();
            var timestamp = DateTime.Now.ToString("G");
            errorMessage
                .AppendLine($"[{timestamp}] Error encountered in Elite Observatory {context}")
                .AppendLine(FormatExceptionMessage(ex))
                .AppendLine();
            File.AppendAllText(errFile, errorMessage.ToString());

            if (showMessage)
            {
                Task.Run(() => MessageBox.Show(
                    $"An error of type {ex.GetType().Name} with context \"{context}\" has been encountered and details have been logged to {errFile}.{Environment.NewLine}" +
                    "You will not be informed of further non-fatal errors this session.",
                    $"Observatory Non-Fatal Error Encountered",
                    MessageBoxButtons.OK, MessageBoxIcon.Error));
            }

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