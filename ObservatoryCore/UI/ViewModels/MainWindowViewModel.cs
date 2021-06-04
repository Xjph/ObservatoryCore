using System;
using System.Collections.Generic;
using System.Text;

namespace Observatory.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(PluginManagement.PluginManager pluginManager)
        {
            core = new CoreViewModel(pluginManager.workerPlugins, pluginManager.notifyPlugins);
        }

        public CoreViewModel core { get; }
    }
}
