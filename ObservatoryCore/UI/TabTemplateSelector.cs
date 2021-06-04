using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using Observatory.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.UI
{
    public class TabTemplateSelector : IDataTemplate
    {
        public bool SupportsRecycling => false;

        [Content]
        public Dictionary<string, IDataTemplate> Templates { get; } = new Dictionary<string, IDataTemplate>();


        public IControl Build(object param)
        {
            return new BasicUIView(); //Templates[param].Build(param);
        }

        public bool Match(object data)
        {
            return data is BasicUIView;
        }
    }
}
