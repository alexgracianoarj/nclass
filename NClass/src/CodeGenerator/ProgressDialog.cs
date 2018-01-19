using System;
using System.Windows.Forms;

namespace NClass.CodeGenerator
{
    public partial class ProgressDialog : Form
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }

        public void UpdateProgress(int progress)
        {
            if (progressBar1.InvokeRequired)
                progressBar1.BeginInvoke(new Action(() => progressBar1.Value = progress));
            else
                progressBar1.Value = progress;

        }

        public void SetIndeterminate(bool isIndeterminate)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.BeginInvoke(new Action(() =>
                {
                    if (isIndeterminate)
                        progressBar1.Style = ProgressBarStyle.Marquee;
                    else
                        progressBar1.Style = ProgressBarStyle.Blocks;
                }
                ));
            }
            else
            {
                if (isIndeterminate)
                    progressBar1.Style = ProgressBarStyle.Marquee;
                else
                    progressBar1.Style = ProgressBarStyle.Blocks;
            }
        }
    }
}
