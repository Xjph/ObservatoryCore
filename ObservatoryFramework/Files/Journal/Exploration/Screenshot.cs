namespace Observatory.Framework.Files.Journal
{
    public class Screenshot : JournalBase
    {
        public string Filename { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public string System { get; init; }
        public string Body { get; init; }
        public float Latitude { get; init; }
        public float Longitude { get; init; }
        public float Altitude { get; init; }
        public int Heading { get; init; }
    }
}
