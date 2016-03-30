using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CheckAsm
{
    public partial class ProgressDialog : Form
    {
        private const int VisibleWidth = 471;
        private const int InvisibleWidth = 383;

        public bool IsCancelButtonVisible
        {
            get
            {
                return cancelButton.Visible;
            }
            set
            {
                cancelButton.Visible = value;
                Width = value? VisibleWidth : InvisibleWidth;
            }
        }

        public string CancelButtonText
        {
            get
            {
                return cancelButton.Text;
            }
            set
            {
                cancelButton.Text = value;
            }
        }

        public new string Text
        {
            get
            {
                return actionTextLabel.Text;
            }
            set
            {
                actionTextLabel.Text = value;
            }
        }

        public object Argument { get; set; }

        public int Progress
        {
            get
            {
                return progressBar.Value;
            }
            set
            {
                progressBar.Value = value;
            }
        }

        public ProgressDialog()
        {
            InitializeComponent();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            actionTextLabel.Text = e.UserState.ToString();
        }

        public void ReportProgress(int progressPercentage, string actionText)
        {
            backgroundWorker.ReportProgress(progressPercentage, actionText);
        }

        protected override void OnLoad(EventArgs e)
        {
            backgroundWorker.RunWorkerAsync(Argument);
            base.OnLoad(e);
        }

        public event DoWorkEventHandler DoWork
        {
            add
            {
                backgroundWorker.DoWork += value;
            }
            remove
            {
                backgroundWorker.DoWork -= value;
            }
        }

        public event RunWorkerCompletedEventHandler WorkerFinished
        {
            add
            {
                backgroundWorker.RunWorkerCompleted += value;
            }
            remove
            {
                backgroundWorker.RunWorkerCompleted -= value;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        public bool CancellationPending
        {
            get
            {
                return backgroundWorker.CancellationPending;
            }
        }

    }
}
