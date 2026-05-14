namespace Observatory.Communicator
{
    public class CommunicatorGridItem
    {
        public CommunicatorGridItem() { }

        public CommunicatorGridItem(string timestamp, string channel, string from, string message)
        {
            Timestamp = timestamp;
            Channel = channel;
            From = from;
            Message = message;
        }

        public string? Timestamp { get; set; }
        public string? Channel { get; set; }
        public string? From { get; set; }
        public string? Message { get; set; }
    }
}
