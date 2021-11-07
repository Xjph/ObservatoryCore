using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory
{
    public static class ErrorReporter
    {
        public static void ShowErrorPopup(string title, string message)
        {
            if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                var errorMessage = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow(new MessageBox.Avalonia.DTO.MessageBoxStandardParams
                {
                    ContentTitle = title,
                    ContentMessage = message
                });
                errorMessage.ShowDialog(desktop.MainWindow);
            }
        }
    }
}
