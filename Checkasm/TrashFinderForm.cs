using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace CheckAsm
{
    public partial class TrashFinderForm : Form
    {
        DirectoryReferenceAnalyzerParameters parameters = new DirectoryReferenceAnalyzerParameters();
        AppDomain directoryScanDomain;
        DirReferenceAnalyzer scanner;
        bool closing;
        private readonly List<string> pendingDeletes = new List<string>();

     
        public TrashFinderForm():this(null)
        {
            
        }

        public TrashFinderForm(DirectoryReferenceAnalyzerParameters parameters)
        {
#if !FREE
            InitializeComponent();
            if (parameters != null)
            {
                folderTextBox.Text = parameters.Directory;
                folderBrowserDialog.SelectedPath = parameters.Directory;
                this.parameters = parameters;
            }
            dataGridView.DataSource = null;
#endif
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.folderTextBox.Text = folderBrowserDialog.SelectedPath;
                parameters.Directory = folderTextBox.Text;
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(folderTextBox.Text))
            {
                MessageBox.Show("Directory path cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(folderTextBox.Text))
            {
                MessageBox.Show("Selected directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            parameters.Directory = folderTextBox.Text;
            Environment.CurrentDirectory = parameters.Directory;
            Analyze();
        }

        private void Analyze()
        {
            browseButton.Enabled = false;
            cancelButton.Enabled = true;
            startButton.Enabled = false;
            progressBar.Visible = true;
            statusLabel.Text = "Initializing...";
            statusLabel.Visible = true;
#if !FREE
            string callingDomainName = Thread.GetDomain().FriendlyName;
            string exeAssembly = Assembly.GetEntryAssembly().FullName;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Environment.CurrentDirectory;
            setup.DisallowBindingRedirects = false;
            setup.DisallowCodeDownload = true;
            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            directoryScanDomain = AppDomain.CreateDomain("DirectoryScan");
            directoryScanDomain.UnhandledException += new UnhandledExceptionEventHandler(directoryScanDomain_UnhandledException);
            scanner = (DirReferenceAnalyzer)directoryScanDomain.CreateInstanceAndUnwrap(exeAssembly, typeof(DirReferenceAnalyzer).FullName);
            scanner.BgWorker = backgroundWorker;
            backgroundWorker.RunWorkerAsync(parameters);
#endif
        }

        void directoryScanDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorMessageBox.Show("CheckAsm has encountered an error from which is unable to recover. Try to restart the application and repeat your action again.", e.ExceptionObject.ToString());
            Environment.Exit(1);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool cancelled = false;
            e.Result = scanner.Scan((DirectoryReferenceAnalyzerParameters)e.Argument, ref cancelled);
            e.Cancel = cancelled;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!closing)
            {
                progressBar.Value = e.ProgressPercentage;
                statusLabel.Text = e.UserState.ToString();
            }
            Trace.WriteLine(e.UserState.ToString());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                closing = true;
                backgroundWorker.CancelAsync();
            }
           
            foreach (string str in this.pendingDeletes)
            {
                try
                {
                    File.Delete(str);
                    continue;
                }
                catch
                {
                    continue;
                }
            }
            base.OnClosing(e);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (closing)
            {
                AppDomain.Unload(directoryScanDomain);
                MessageBox.Show("Operation cancelled", "Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (e.Error != null)
            {
                MessageBox.Show("An error occured during directory scan. Error: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!e.Cancelled)
            {

                var report = ((DirReferenceScanResult)e.Result).DirectoryReport.Values;
                DataTable table = new DataTable();
                table.Columns.Add("Assembly Name", typeof(string));
                table.Columns.Add("Number of references", typeof(int));
                table.Columns.Add("Number of assemblies referencing this assembly", typeof(int));
                foreach (var item in report)
                {
                    table.Rows.Add(item.ShortName, item.ReferencesCount, item.ReferencingCount);
                    
                }
                dataGridView.DataSource = null;
                dataGridView.DataSource = table;                
                
            }

            AppDomain.Unload(directoryScanDomain);
            directoryScanDomain = null;

            
            cancelButton.Enabled = false;
            startButton.Enabled = true;
            progressBar.Visible = false;
            browseButton.Enabled = true;
        }

        private void dataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = ((DataTable)dataGridView.DataSource).Rows[e.RowIndex];
            if ((int)dataGridView.Rows[e.RowIndex].Cells[1].Value == 0 && (int)dataGridView.Rows[e.RowIndex].Cells[2].Value == 0)
                dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 206, 150);
            else
                dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
        }
    }
}
