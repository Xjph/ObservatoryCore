using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Explorer
{
    internal class CriteriaLoadException : Exception
    {
        public CriteriaLoadException(string message, string script)
        {
            Message = message;
            OriginalScript = script;
        }

        new public readonly string Message;
        public readonly string OriginalScript;
    }
}
