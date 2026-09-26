namespace Observatory.UI
{
    public class ColumnSizing
    {
        public required string PluginName { get; set; }
        public required string PluginVersion { get; set; }
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
