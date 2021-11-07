using System;

namespace Observatory.Framework
{
    /// <summary>
    /// Container for exceptions within plugins which cannot be gracefully handled in context,
    /// but benefit from having a context-specific user message.
    /// </summary>
    public class PluginException : Exception
    {
        /// <summary>
        /// Initialze new PluginException with details of the originating plugin and a specific user-facing message for display.
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="userMessage"></param>
        /// <param name="innerException"></param>
        public PluginException(string pluginName, string userMessage, Exception innerException) : base(innerException.Message, innerException)
        {
            PluginName = pluginName;
            UserMessage = userMessage;
        }

        /// <summary>
        /// Name of plugin from which the exception was thrown.
        /// </summary>
        public string PluginName { get; }

        /// <summary>
        /// Message to be displayed to user.
        /// </summary>
        public string UserMessage { get; }
        
    }
}
