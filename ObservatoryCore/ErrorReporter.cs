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
            StringBuilder displayMessage = new();
            displayMessage.AppendLine($"{errorList.Count} error{(errorList.Count > 1 ? "s" : string.Empty)} encountered.");
            var firstFiveErrors = errorList.Take(Math.Min(5, errorList.Count)).Select(e => e.error);
            displayMessage.AppendJoin(Environment.NewLine, firstFiveErrors);
            displayMessage.AppendLine();
            displayMessage.Append("Full error details logged to ObservatoryErrorLog file in your documents folder.");

            if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var errorMessage = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow(new MessageBox.Avalonia.DTO.MessageBoxStandardParams
                    {
                        ContentTitle = title,
                        ContentMessage = displayMessage.ToString(),
                        Topmost = true
                    });
                    errorMessage.Show();
                });
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

            errorList.Clear();
        }
    }
}
