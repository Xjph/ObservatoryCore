namespace Observatory.UI
{
    public class ColumnSizing
    {
        public string PluginName { get; set; }
        public string PluginVersion { get; set; }
        public Dictionary<string, int> ColumnWidth 
        {
            get
            {
                _columnWidth ??= new Dictionary<string, int>();

                return _columnWidth;
            }
             
            set => _columnWidth = value;
        }

        private Dictionary<string, int>? _columnWidth;
    }
}
