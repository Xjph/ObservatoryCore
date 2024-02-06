using Observatory.Utils;
using System.Text;

namespace Observatory.UI
{
    public partial class ReadAllForm : Form
    {
        private CancellationTokenSource ReadAllCancel;

        private byte[] eggBytes = 
        { 
            0x52, 0x65, 0x74, 0x69, 
            0x63, 0x75, 0x6C, 0x61, 
            0x74, 0x69, 0x6E, 0x67, 
            0x20, 0x53, 0x70, 0x6C, 
            0x69, 0x6E, 0x65, 0x73, 
            0x2E, 0x2E, 0x2E 
        };

        public ReadAllForm()
        {
            InitializeComponent();

            if (new Random().Next(1, 20) == 20)
                Text = Encoding.UTF8.GetString(eggBytes);

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
