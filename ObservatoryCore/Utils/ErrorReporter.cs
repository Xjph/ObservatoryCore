using System.Text;

namespace Observatory.Utils
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

            //TODO: Winform error popup

            // Log entirety of errors out to file.
            var timestamp = DateTime.Now.ToString("G");
            StringBuilder errorLog = new();
            foreach (var error in errorList)
            {
                errorLog.AppendLine($"[{timestamp}]:");
                errorLog.AppendLine($"{error.error} - {error.detail}");
                errorLog.AppendLine();
            }

            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            File.AppendAllText(docPath + Path.DirectorySeparatorChar + "ObservatoryErrorLog.txt", errorLog.ToString());

            errorList.Clear();
        }
    }
}
