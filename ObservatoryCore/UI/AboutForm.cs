using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Observatory.UI
{
    public partial class AboutForm : Form
    {
        private readonly AboutInfo _metadata;
        private readonly List<LinkLabel> _linkLabels;
        
        public AboutForm(AboutInfo metadata)
        {
            InitializeComponent();
            _metadata = metadata;
            _linkLabels = new() { lnk1, lnk2, lnk3, lnk4 };

            Text = $"About {metadata.FullName}";
            lblFullName.Text = metadata.FullName ?? "";
            lblShortName.Text = metadata.ShortName ?? "";
            txtDescription.Text = metadata.Description ?? "";
            txtDescription.Select(0, 0);
            lblAuthorName.Text = metadata.AuthorName ?? "";

            SetupLinks(_metadata.Links);
        }

        private void SetupLinks(List<AboutLink> links)
        {
            // Initiate every link.
            int linkIdx = 0;
            for (int ll = 0; ll < _linkLabels.Count; ll++)
            {
                var linkLabel = _linkLabels[ll];

                // The list of links may have many links, some of which are invalid. Render the first 4 valid links we find.
                // If not enough links are found, we clear the UI/hide the link.
                AboutLink link = null;
                while (linkIdx < links.Count)
                {
                    link = links[linkIdx];
                    linkIdx++;
                    if (!IsValidLink(link))
                        continue;
                    else
                        break;
                }
                SetupLink(linkLabel, link);
            }
        }

        private static bool IsValidLink(AboutLink link)
        {
            if (string.IsNullOrEmpty(link.Text) || string.IsNullOrEmpty(link.Url)) return false;

            Uri uri;
            if (!Uri.TryCreate(link.Url, new UriCreationOptions(), out uri)) return false;

            return true;
        }

        private void SetupLink(LinkLabel lnkLabel, AboutLink? linkInfo = null)
        {
            lnkLabel.Links.Clear();
            if (linkInfo != null)
            {
                lnkLabel.Visible =  true;
                lnkLabel.Text = linkInfo.Text;
                LinkLabel.Link link = new(0, lnkLabel.Text.Length, linkInfo.Url);
                lnkLabel.Links.Add(link);
            }
            else
            {
                lnkLabel.Visible = false;
                lnkLabel.Text = string.Empty;
            }
        }

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = e.Link?.LinkData?.ToString() ?? "";
            if (!string.IsNullOrWhiteSpace(url))
                OpenURL(url);
        }

        private static void OpenURL(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
