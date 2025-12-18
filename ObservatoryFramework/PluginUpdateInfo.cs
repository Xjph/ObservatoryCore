using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework
{
    /// <summary>
    /// The update status of a plugin returned via <see cref="PluginUpdateInfo"/> from the
    /// <see cref="Interfaces.IObservatoryPlugin.CheckForPluginUpdate()"/> method.
    /// </summary>
    public enum PluginUpdateStatus
    {
        /// <summary>
        /// Indicates that there is no update available known or available for the plugin. Other properties
        /// of the <see cref="PluginUpdateInfo"/> class are ignored.
        /// </summary>
        NoUpdate,
        /// <summary>
        /// Indicates that there is an update available for the plugin. It is recommended that plugins provide a download Url
        /// via the <see cref="PluginUpdateInfo.Url"/> property.
        /// </summary>
        UpdateAvailable,
        /// <summary>
        /// Indicates that there is an update available and it has already been downloaded and is ready for installation.
        /// The user just needs to restart the application to begin using the new version.
        /// </summary>
        UpdateReady,
    }

    /// <summary>
    /// A response object that contains information about the state of a plugin update.
    /// </summary>
    public class PluginUpdateInfo
    {
        private string _customUrlText = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginUpdateInfo"/> class.
        /// With no further modification, it indicates there is no update available.
        /// </summary>
        public PluginUpdateInfo()
        {
            Status = PluginUpdateStatus.NoUpdate;
        }

        /// <summary>
        /// Gets or sets an enumeration that specifies the state of the plugin update.
        /// See <see cref="PluginUpdateStatus"/> enum for possible values.
        /// </summary>
        public PluginUpdateStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a URL. This URL will most likely point to the download location of the plugin update.
        /// However, this can point to other destinations as well (ie. documentation or release notes).
        /// See the <see cref="UrlText"/> property to customized the text the URL is linked to for non-typical uses.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets optional custom text that is linked to the URL provided by the <see cref="Url"/> property. Use this
        /// to customize the text linked to the URL provided via the <see cref="Url"/> property.
        /// If an empty/no value is provided, default text is provided based on the value of <see cref="Status"/> property.
        /// </summary>
        public string UrlText
        {
            get
            {
                if (string.IsNullOrEmpty(_customUrlText))
                {
                    switch (Status)
                    {
                        case PluginUpdateStatus.UpdateAvailable:
                            return "Update Available";
                        case PluginUpdateStatus.UpdateReady:
                            return "Update Ready; Restart the application";
                        default:
                            return "";
                    }
                }
                else return _customUrlText;
            }
            set => _customUrlText = value;
        }
    }
}
