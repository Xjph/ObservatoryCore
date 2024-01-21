using Observatory.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Observatory.UI
{
    public partial class ReadAllForm : Form
    {
        private CancellationTokenSource ReadAllCancel;

        public ReadAllForm()
        {
            InitializeComponent();
            
            var ReadAllJournals = LogMonitor.GetInstance.ReadAllGenerator(out int fileCount);
            int progressCount = 0;
            ReadAllCancel = new CancellationTokenSource();
            HandleCreated += (_,_) =>
            Task.Run(() =>
            {
                foreach (var journal in ReadAllJournals())
                {
                    if (ReadAllCancel.IsCancellationRequested)
                    {
                        break;
                    }

                    progressCount++;
                    Invoke(() =>
                    {
                        JournalLabel.Text = journal.ToString();
                        ReadAllProgress.Value = (progressCount * 100) / fileCount;
                    });
                }
                Invoke(()=>Close());
            });
            
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ReadAllCancel.Cancel();
        }
    }
}
