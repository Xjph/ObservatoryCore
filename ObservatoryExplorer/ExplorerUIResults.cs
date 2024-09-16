using Observatory.Framework;

namespace Observatory.Explorer
{
    public class ExplorerUIResults
    {
        [ColumnSuggestedWidth(300)]
        public string Time { get; set; }

        [ColumnSuggestedWidth(350)]
        public string BodyName { get; set; }

        [ColumnSuggestedWidth(400)]
        public string Description { get; set; }

        [ColumnSuggestedWidth(600)]
        public string Details { get; set; }
    }
}
