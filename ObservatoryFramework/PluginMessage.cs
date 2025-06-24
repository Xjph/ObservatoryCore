namespace Observatory.Framework
{
    /// <summary>
    /// Represents a message sent between plugins in the Observatory Framework.
    /// </summary>
    public class PluginMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginMessage"/> class.
        /// </summary>
        /// <param name="messageType">The type of the message, used to distinguish between different messages which may be sent by the same plugin.</param>
        /// <param name="messagePayload">A dictionary containing the payload of the message as key-value pairs.</param>
        /// <param name="inReplyTo">An optional identifier of the message for which this message is a reply.</param>
        public PluginMessage(string messageType, Dictionary<string, object> messagePayload = null, Guid? inReplyTo = null)
        {
            InReplyTo = inReplyTo;
            MessageType = messageType;
            MessagePayload = messagePayload ?? [];
        }

        private readonly Guid _messageId = Guid.NewGuid();

        /// <summary>
        /// Gets the unique identifier for the message.
        /// </summary>
        public Guid MessageId { get => _messageId; }

        /// <summary>
        /// Gets the identifier of the message to which this message is a reply, if applicable.
        /// </summary>
        public Guid? InReplyTo { get; init; }

        /// <summary>
        /// Gets the type of the message represented as a string, or an empty string if the sending plugin does not specify message types.
        /// </summary>
        public string MessageType { get; init; }

        /// <summary>
        /// Gets the payload of the message as a dictionary of key-value pairs.
        /// </summary>
        public Dictionary<string, object> MessagePayload { get; init; }
    }
}
