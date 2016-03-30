using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CheckAsm.ManagedGAC;

namespace CheckAsm
{
    class GACParserDialog:Form
    {
        private ProgressBar progressBar;
        private Label label1;
        private System.ComponentModel.BackgroundWorker bgWorker;

        private void InitializeComponent()
        {
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_ProgressChanged);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 37);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(441, 23);
            this.progressBar.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Listing Global Assembly Cache, please wait...";
            // 
            // GACParserDialog
            // 
            this.ClientSize = new System.Drawing.Size(471, 72);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GACParserDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.GACParserDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        List<AsmData> assemblies;
        double maxAssemblies;

        public GACParserDialog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public List<AsmData> GetAssembliesList(double maxAssemblies)
        {
            this.maxAssemblies = maxAssemblies;
            this.ShowDialog();
            return assemblies;
        }

        private void GACParserDialog_Load(object sender, EventArgs e)
        {
            bgWorker.RunWorkerAsync();
        }

        private void bgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            assemblies = AssemblyCache.GetAssembliesInGac(bgWorker, maxAssemblies);
        }

        private void bgWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100)
            {
                progressBar.Value = e.ProgressPercentage;
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Close();
                if (e.Error is System.IO.FileLoadException)
                {
                    MessageBox.Show("Application was not able to access GAC. Some files in GAC are locked by another process or the file \"fusion.dll\" is not available. Please ensure no installation is currently running, the fusion.dll is available, try to restart the CheckAsm or your computer. \r\nIf none of the options above resolves the problem, report a bug to support@amberfish.net and add a screenshot of this error message.\r\nIf you continue using the application, it may become unstable or provide invalid results.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ErrorMessageBox.Show(e.Error.Message, e.Error.ToString());
                }
            }
            else
            {
                progressBar.Value = 100;
                Close();
            }
        }
    }
}
