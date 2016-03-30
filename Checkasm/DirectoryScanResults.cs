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
    partial class DirectoryScanResults : Form
    {
        DirectoryScannerOptions _params;
        DirectoryScannerOptions parameters 
        { 
            get { Trace.WriteLine("params being read: " + _params); return _params; }
            set 
            {
                Trace.WriteLine("params set to " + value); 
                _params = value;
                //try
                //{
                //    StackTrace stackTrace = new StackTrace();           // get call stack
                //    StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

                //    // write call stack method names
                //    foreach (StackFrame stackFrame in stackFrames)
                //    {
                //        Trace.WriteLine(stackFrame.GetMethod().DeclaringType + "." + stackFrame.GetMethod().Name);   // write method name
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Trace.WriteLine("Failed to get callstack " + ex);
                //}
                
            }
        }
        AppDomain directoryScanDomain;
        DirectoryScanner directoryScanner;
        List<AsmData> gacAssemblies;
        MainDialog applicationWindow;
        bool closing;

        public MainDialog ApplicationWindow
        {
            get { return applicationWindow; }
            set { applicationWindow = value; }
        }

        public List<AsmData> GacAssemblies
        {
            get { return gacAssemblies; }
            set { gacAssemblies = value; }
        }

        public Dictionary<string, List<Redirection>> Redirections { get; set; }

        public DirectoryScanResults()
        {
            InitializeComponent();
        }

        public DirectoryScanResults(DirectoryScannerOptions parameters)
        {
            InitializeComponent();
            if (parameters != null)
            {
                this.parameters = parameters;
                folderTextBox.Text = parameters.LastDirectory;
                recursiveCheckbox.Checked = parameters.Recursive;
            }
            else
            {
                this.parameters = new DirectoryScannerOptions();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                closing = true;
                backgroundWorker.CancelAsync();
            }
            base.OnClosing(e);
        }

        private void Analyze()
        {
            browseButton.Enabled = false;
            cancelButton.Enabled = true;
            startButton.Enabled = false;
            progressBar.Visible = true;
            statusLabel.Text = "Initializing...";
            statusLabel.Visible = true;

            string exeAssembly = Assembly.GetEntryAssembly().FullName;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Environment.CurrentDirectory;
            setup.DisallowBindingRedirects = false;
            setup.DisallowCodeDownload = true;
            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            directoryScanDomain = AppDomain.CreateDomain("DirectoryScan");
            directoryScanDomain.UnhandledException += new UnhandledExceptionEventHandler(directoryScanDomain_UnhandledException);
            directoryScanner = (DirectoryScanner)directoryScanDomain.CreateInstanceAndUnwrap(exeAssembly, typeof(DirectoryScanner).FullName);
            directoryScanner.BgWorker = backgroundWorker;
            directoryScanner.GacAssemblies = gacAssemblies;
            directoryScanner.Redirections = Redirections;
            backgroundWorker.RunWorkerAsync(parameters);
        }

        void directoryScanDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorMessageBox.Show("CheckAsm has encountered an error from which is unable to recover. Try to restart the application and repeat your action again.", e.ExceptionObject.ToString());
            Environment.Exit(1);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool cancelled = false;
            e.Result = directoryScanner.Scan((DirectoryScannerOptions)e.Argument, ref cancelled);
            e.Cancel = cancelled;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            statusLabel.Text = e.UserState.ToString();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (closing)
            {
                AppDomain.Unload(directoryScanDomain);
                MessageBox.Show("Operation cancelled", "Directory scanner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (e.Error != null)
            {
                MessageBox.Show("An error occurred during directory scan. Error: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(!e.Cancelled)
            {
                //fill in the new panel
                var asmData = from item in ((AssemblyEntry[]) e.Result) where item.AsmData != null select item.AsmData;
                var invalidData = from item in ((AssemblyEntry[]) e.Result)
                                  where item.AsmData == null
                                  select new AsmData(Path.GetFileName(item.FileName), item.FileName);
                List<AsmData> fullList = asmData.ToList();
                fullList.AddRange(invalidData);
                
                //fix file name
                foreach (var item in fullList)
                {
                    item.Name = Path.GetFileName(item.Name);
                }

                DataTable table = new DataTable();
                table.Columns.Add("Architecture", typeof(string));
                table.Columns.Add("Runtime Version", typeof(string));
                table.Columns.Add("Full Name", typeof(string));
                table.Columns.Add("Path", typeof(string));
                table.Columns.Add("Status", typeof(AsmData.AsmValidity));
                foreach (var item in fullList)
                {
                    table.Rows.Add(item.Architecture, item.RuntimeVersionShort, item.AssemblyFullName, item.Path, item.Validity);

                }
                dataGridView1.DataSource = table;
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    if (col.Index < 2 || col.Index == 4)
                    {
                        col.FillWeight = 30;
                    }
                    else
                    {
                        col.FillWeight = 100;
                    }
                }
            }
            AppDomain.Unload(directoryScanDomain);
            directoryScanDomain = null;
            cancelButton.Enabled = false;
            startButton.Enabled = true;
            progressBar.Visible = false;
            browseButton.Enabled = true;
            statusLabel.Text = "Finished";
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.folderTextBox.Text = folderBrowserDialog.SelectedPath;
                parameters.LastDirectory = folderTextBox.Text;
                
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
            Trace.WriteLine("parameters:" + _params);
            Trace.WriteLine("foldertextbox:" + folderTextBox);
            Trace.WriteLine("recursiveCheckbox:" + recursiveCheckbox);
            Trace.WriteLine("currentDomainName:" + AppDomain.CurrentDomain.FriendlyName);
            Trace.Flush();
            parameters.LastDirectory = folderTextBox.Text;
            parameters.Recursive = recursiveCheckbox.Checked;
            Environment.CurrentDirectory = parameters.LastDirectory;
            Analyze();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
        }

        private void recursiveCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            parameters.Recursive = recursiveCheckbox.Checked;
        }

        private void folderTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[4].Value == null || dataGridView1.Rows[e.RowIndex].Cells[4].Value is DBNull)
                return;

            var status = (AsmData.AsmValidity)dataGridView1.Rows[e.RowIndex].Cells[4].Value;
            if (status != AsmData.AsmValidity.Valid)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(255, 206, 150) };
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle = dataGridView1.DefaultCellStyle;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            ApplicationWindow.OpenFile((string)dataGridView1.Rows[e.RowIndex].Cells[3].Value);
            ApplicationWindow.BringToFront();
        }
    }
}
