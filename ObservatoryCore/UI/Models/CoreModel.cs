using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Observatory.UI.ViewModels;

namespace Observatory.UI.Models
{
    public class CoreModel
    {
        public string Name { get; set; }
        public ViewModelBase UI { get; set; }
        
    }
}
