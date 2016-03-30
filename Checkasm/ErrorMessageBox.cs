using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CheckAsm.amberfish_errorreporting;

namespace CheckAsm
{
    public partial class ErrorMessageBox : Form
    {
        public ErrorMessageBox()
        {
            InitializeComponent();
        }

        public string Message { get; set; }

        public string Details { get; set; }

        private string comment;
        private string email;
        ProgressDialog progressDialog;

        public ErrorMessageBox(string message, string details):this()
        {
            Message = message;
            Details = details;
            Trace.Flush();
        }

        public static DialogResult Show(string message, string details)
        {
            ErrorMessageBox erm = new ErrorMessageBox(message, details);
            return erm.ShowDialog();
        }

        public static DialogResult Show(string dialogErrorMessage, string errorMessage, string details)
        {
            ErrorMessageBox erm = new ErrorMessageBox(errorMessage, details);
            erm.mainMessageLabel.Text = dialogErrorMessage;
            return erm.ShowDialog();
        }

        private void detailsButton_Click(object sender, EventArgs e)
        {
            ErrorDetailsForm details = new ErrorDetailsForm();
            details.Details = Message + Details;
            details.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (sendReportCheckbox.Checked)
            {
                progressDialog = new ProgressDialog();
                progressDialog.CancelButtonText = "Cancel";
                progressDialog.DoWork += new DoWorkEventHandler(progressDialog_DoWork);
                progressDialog.WorkerFinished += new RunWorkerCompletedEventHandler(progressDialog_WorkerFinished);
                comment = commentTextbox.Text;
                email = emailTextbox.Text;

                progressDialog.ShowDialog();
                Cursor = Cursors.Default;
            }
        }

        void progressDialog_WorkerFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            progressDialog.Close();
            if (e.Error != null)
            {
                if (DialogResult.Cancel == MessageBox.Show("Your report could not be submitted. Please make sure you are connected to the Internet and try it again. \r\nError: " + e.Error.Message, "Connection problem", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error))
                {
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Operation cancelled", "Error Reporting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                ReportSentMessageBox msg = new ReportSentMessageBox();
                msg.ShowDialog();
                Close();
            }
            
        }

        void progressDialog_DoWork(object sender, DoWorkEventArgs e)
        {
            
            progressDialog.ReportProgress(0, "Initializing...");
            ErrorReporting errorReportingService = new ErrorReporting();
            
            string targetFileName = Guid.NewGuid().ToString() + ".log";
            string message = string.Format("{0}\r\n\r\n{1}\r\n\r\n{2}\r\n{3}\r\n{4}\r\nhttp://www.amberfish.net/diagnostics/{5}", 
                Message,
                Details,
                comment, 
                email, 
                Assembly.GetExecutingAssembly().GetName().Version, 
                targetFileName
                );

            errorReportingService.ReportBug(message, targetFileName);

            //upload log file
            
            Trace.Flush();
            byte[] buffer = new byte[20000];
            Program.StopLogging();
            using (var stream = new FileStream(Program.LogFilePath, FileMode.Open, FileAccess.Read))
            {
                //do not upload more than last 500000 bytes
                if (stream.Length > 500000)
                {
                    stream.Seek(stream.Length - 500000, SeekOrigin.Begin);
                }
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                bool append = false;
                while (bytesRead > 0 && !progressDialog.CancellationPending)
                {
                    errorReportingService.UploadFilePart(targetFileName, append, buffer, bytesRead);
                    buffer = new byte[buffer.Length];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    append = true;
                    progressDialog.ReportProgress((int)(100 * stream.Position/ stream.Length), "Uploading log file...");    
                }
            }
            if(progressDialog.CancellationPending)
                e.Cancel = true;
            Program.StartLogging();
        }

        private void sendReportCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Enabled = sendReportCheckbox.Checked;
            button1.Enabled = sendReportCheckbox.Checked;
        }

        private void emailMeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Enabled = emailMeCheckbox.Checked && sendReportCheckbox.Checked;
        }

    }
}
