using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Amberfish.Canvas.Views;

namespace CheckAsm
{
    public partial class AnalyzeDirReferencesForm : Form
    {
        DirectoryReferenceAnalyzerParameters parameters = new DirectoryReferenceAnalyzerParameters();
        public List<AsmData> GacAssemblies { get; set; }

        AppDomain directoryScanDomain;
        DirReferenceAnalyzer scanner;
        bool closing;
        private readonly List<string> pendingDeletes = new List<string>();
        string lastFolder;

        internal MainDialog ApplicationWindow { get; set; }

        public AnalyzeDirReferencesForm():this(null)
        {
            
        }

        public AnalyzeDirReferencesForm(DirectoryReferenceAnalyzerParameters parameters)
        {
#if !FREE
            InitializeComponent();
            if (parameters != null)
            {
                folderTextBox.Text = parameters.Directory;
                folderBrowserDialog.SelectedPath = parameters.Directory;
                this.parameters = parameters;
            }
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
            lastFolder = folderTextBox.Text;
            Environment.CurrentDirectory = parameters.Directory;
            
            Analyze();
        }

        private void Analyze()
        {
            browseButton.Enabled = false;
           
            startButton.Enabled = false;
            progressBar.Visible = true;
            statusLabel.Text = "Initializing...";
            statusLabel.Visible = true;
#if !FREE
            string exeAssembly = System.Reflection.Assembly.GetEntryAssembly().FullName;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Environment.CurrentDirectory;
            setup.DisallowBindingRedirects = false;
            setup.DisallowCodeDownload = true;
            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            directoryScanDomain = AppDomain.CreateDomain("DirectoryScan");
            directoryScanDomain.UnhandledException += new UnhandledExceptionEventHandler(directoryScanDomain_UnhandledException);
            scanner = (DirReferenceAnalyzer)directoryScanDomain.CreateInstanceAndUnwrap(exeAssembly, typeof(DirReferenceAnalyzer).FullName);
            scanner.BgWorker = backgroundWorker;
            parameters.GacAssemblies = GacAssemblies;
            backgroundWorker.RunWorkerAsync(parameters);
#endif
        }

        void directoryScanDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorMessageBox.Show("CheckAsm has encountered an error from which is unable to recover. Try to restart the application and repeat your action again.", e.ExceptionObject.ToString());
            Environment.Exit(1);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void OnClosing(CancelEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                closing = true;
                backgroundWorker.CancelAsync();
            }
            //if (this.pictureBox.Image != null)
            //{
            //    this.pictureBox.Image.Dispose();
            //    this.pictureBox.Image = null;
            //}
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
                MessageBox.Show("An error occurred during directory scan. Error: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!e.Cancelled)
            {
                canvas.Reset();
                string source = ((DirReferenceScanResult)e.Result).GraphText;
                canvas.Suspend = true;
                canvas.Controller.CreateGraphFromString(source);
                canvas.Controller.UpdateShadowNodes(from m in ((DirReferenceScanResult)e.Result).MissingAssemblies select m.Missing);
                canvas.Suspend = false;
                btnHierarchical.Enabled = true;
                btnCircular.Enabled = true;
                btnSave.Enabled = true;
                btnSymmetrical.Enabled = true;
            }

            AppDomain.Unload(directoryScanDomain);
            directoryScanDomain = null;

            
           
            startButton.Enabled = true;
            progressBar.Visible = false;
            browseButton.Enabled = true;
            statusLabel.Text = "Finished";
        }

        private void btnSymmetrical_Click(object sender, EventArgs e)
        {
            canvas.Suspend = true;
            
            for (int i = 0; i < 15; i++)
            {
                var best = int.MaxValue;
                string bestState = canvas.GetState();
                do
                {
                    var intersections = canvas.Controller.ApplyDirectedForceLayout();
                    if (best > intersections)
                    {
                        best = intersections;
                        bestState = canvas.GetState();
                    }
                    else
                    {
                        canvas.LoadState(bestState);
                        break;
                    }
                    
                } while (true);
            }
            Application.DoEvents();
            canvas.Invalidate(); 
            canvas.Suspend = false;
            canvas.Controller.BringToView(canvas.Width, canvas.Height);
        }

        private void btnHierarchical_Click(object sender, EventArgs e)
        {
            canvas.Suspend = true;
            canvas.Controller.ApplyHierarchicalLayout();
            canvas.Suspend = false;
            Application.DoEvents();
            canvas.Invalidate();
            canvas.Controller.BringToView(canvas.Width, canvas.Height);

        }

        private void analyzeBringToMainWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ApplicationWindow != null)
            {
                var target = GetTargetFromCtxMenu(sender);
                ApplicationWindow.OpenFile(target);
                ApplicationWindow.BringToFront();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var target = GetTargetFromCtxMenu(sender);
                Process.Start("explorer.exe", "/select,\"" + target + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to navigate to the selected file: " + ex.Message);
            }
        }

        private string GetTargetFromCtxMenu(object sender)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var view = (NodeView)((ContextMenuStrip)menuItem.GetCurrentParent()).SourceControl;
            var fileName = view.Model.Text;
            var target = Path.Combine(lastFolder, fileName);
            return target;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                canvas.SaveImageAs(saveFileDialog.FileName);
            }
        }

        private void setColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            
            var view = (NodeView)((ContextMenuStrip)menuItem.GetCurrentParent()).SourceControl;
            colorDialog.Color = view.NodeColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                canvas.Controller.SetNodeColor(view.Model.Id, colorDialog.Color);
            }
        }

        private void btnCircular_Click(object sender, EventArgs e)
        {
            canvas.Suspend = true;
            canvas.Controller.ApplyCircularLayout();
            canvas.Suspend = false;
            Application.DoEvents();
            canvas.Invalidate();
            canvas.Controller.BringToView(canvas.Width, canvas.Height);
        }

        private void ReverseLookupToolstripItem_Click(object sender, EventArgs e)
        {
            canvas.Suspend = true;
            canvas.HighlightedEdgeColor = Color.Orange;
            var menuItem = (ToolStripMenuItem)sender;
            var view = (NodeView)((ContextMenuStrip)menuItem.GetCurrentParent()).SourceControl;
            canvas.Controller.HighlightNeighbors(view.Model.Id, Color.Orange, Color.Blue);
            canvas.Suspend = false;
            Application.DoEvents();
            canvas.Invalidate();
            canvas.Controller.BringToView(canvas.Width, canvas.Height);
        }

        
    }
}
