namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when the player takes a screenshot.
    /// </summary>
    public class Screenshot : JournalBase
    {
        /// <summary>
        /// <para>Filename of the screenshot taken in the form of "\\ED Pictures\\filename"</para>
        /// <para>"\\ED Pictures\\" corresponds to "%userprofile%\Pictures\Frontier Developments\Elite Dangerous\"</para>
        /// </summary>
        public string Filename { get; init; }
        /// <summary>
        /// Pixel width of the saved image.
        /// </summary>
        public int Width { get; init; }
        /// <summary>
        /// Pixel height of the saved image.
        /// </summary>
        public int Height { get; init; }
        /// <summary>
        /// System name of the current system.
        /// </summary>
        public string System { get; init; }
        /// <summary>
        /// Body name of the current location.
        /// </summary>
        public string Body { get; init; }
        /// <summary>
        /// Current latitude if applicable.
        /// </summary>
        public float Latitude { get; init; }
        /// <summary>
        /// Current longitude if applicable.
        /// </summary>
        public float Longitude { get; init; }
        /// <summary>
        /// Current altitude if applicable.
        /// </summary>
        public float Altitude { get; init; }
        /// <summary>
        /// Current heading if applicable.
        /// </summary>
        public int Heading { get; init; }
    }
}
