namespace Observatory.Explorer
{
    internal class CriteriaLoadException : Exception
    {
        public CriteriaLoadException(string message, string script = "")
        {
            Message = message;
            OriginalScript = script;
        }

        public new readonly string Message;
        public readonly string OriginalScript;
    }
}
