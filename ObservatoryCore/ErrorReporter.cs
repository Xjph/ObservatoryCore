using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory
{
    public static class ErrorReporter
    {
        public static void ShowErrorPopup(string title, List<(string error, string detail)> errorList)
        {
            // Limit number of errors displayed.
            string displayMessage = string.Join(Environment.NewLine, errorList.Take(Math.Min(10, errorList.Count)).Select(e => e.error));

            if (errorList.Count > 10)
                displayMessage += $"{errorList.Count - 10} more errors logged.";

            if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                var errorMessage = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow(new MessageBox.Avalonia.DTO.MessageBoxStandardParams
                {
                    ContentTitle = title,
                    ContentMessage = displayMessage,
                    Topmost = true
                });
                errorMessage.Show();
            }

            // Log entirety of errors out to file.
            var timestamp = DateTime.Now.ToString("G");
            StringBuilder errorLog = new();
            foreach (var error in errorList)
            {
                errorLog.AppendLine($"[{timestamp}]:");
                errorLog.AppendLine($"{error.error} - {error.detail}");
                errorLog.AppendLine();
            }

            var docPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            System.IO.File.AppendAllText(docPath + System.IO.Path.DirectorySeparatorChar + "ObservatoryErrorLog.txt", errorLog.ToString());
        }
    }
}
