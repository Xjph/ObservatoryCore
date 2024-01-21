using Observatory.Framework;

namespace Observatory.Explorer
{
    public class ExplorerUIResults
    {
        [ColumnSuggestedWidth(150)]
        public string Time { get; set; }

        [ColumnSuggestedWidth(150)]
        public string BodyName { get; set; }

        [ColumnSuggestedWidth(200)]
        public string Description { get; set; }

        [ColumnSuggestedWidth(200)]
        public string Details { get; set; }
    }
}
