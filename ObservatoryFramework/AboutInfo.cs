using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework
{
    /// <summary>
    /// Provides metadata about a plugin or app.
    /// </summary>
    public class AboutInfo
    {
        public AboutInfo()
        {
            Links = new();
        }

        /// <summary>
        /// The short name of the plugin or app.
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// The full name of the plugin or app.
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// A brief, multiline description of the plugin or app.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Your name. This is public, remember.
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// Links to websites, documentation, donation, what have you. Up to 4 links will be rendered in the order added.
        /// </summary>
        public List<AboutLink> Links { get; init; }
    }

    /// <summary>
    /// Provides information for a link to be displayed in the "About" view.
    /// </summary>
    public class AboutLink
    {
        public AboutLink(string text, string url)
        {
            Text = text;
            Url = url;
        }

        /// <summary>
        /// Required. The text that will be linked to the URL below.
        /// </summary>
        public string Text { get; init; }
        /// <summary>
        /// Required. The URL to launch when the link is clicked.
        /// </summary>
        public string Url { get; init; }
    }
}
