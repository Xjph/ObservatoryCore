using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class RequiredResource
    {
        public string Name { get; init; }

        public string Name_Localised { get; init; }

        public int RequiredAmount { get; init; }

        public int ProvidedAmount { get; init; }

        public int Payment { get; init; }
    }
}
