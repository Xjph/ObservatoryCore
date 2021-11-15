using System;
using System.Collections.Generic;
using System.Linq;

namespace Observatory.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(PluginManagement.PluginManager pluginManager)
        {
            core = new CoreViewModel(pluginManager.workerPlugins, pluginManager.notifyPlugins);

            if (pluginManager.errorList.Any())
                ErrorReporter.ShowErrorPopup("Plugin Load Error", string.Join(Environment.NewLine, pluginManager.errorList));
        }

        public CoreViewModel core { get; }
    }
}
