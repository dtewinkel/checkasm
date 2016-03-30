using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Security;
using System.Runtime.InteropServices;
using System.Threading;
using CheckAsm.Properties;
using System.Net;
using System.Xml.Linq;
using System.Xml.XPath;
using CheckAsm.Descriptors;
using CheckAsm.Dialogs;

namespace CheckAsm
{
    partial class MainDialog : Form
    {
        bool processing = false;
        bool filterOverrideEnabled = false;

        static readonly Options userOptions = new Options();
        string fileToAnalyze;   //currently open file
        List<AsmData> gacAssemblies = new List<AsmData>();
        AsmData rootAssembly;       //root assembly information
        AnalyzerResult analyzerResult;
        Analyzer analyzer = null;
        AppDomain analyzerDomain = null;
        string workingDir = ""; //current working directory
        Search search = new Search();
        SearchForm searchForm;


        Dictionary<string, List<Redirection>> assemblyRedirections;
        Dictionary<string, List<Redirection>> gacFrameworkRedirections;

        int treeDepth = 0;
        int maxTreeDepth = 0;
        int requestedTreeDepth = 0;

        ProgressDialog progressDialog;

        public static Options UserSettings { get { return userOptions; } }

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="args">args: file to analyze</param>
        /// <remarks>usage: CheckAsm [assembly.dll | assembly.exe]</remarks>
        public MainDialog(string[] args)
        {
            InitializeComponent();
#if DEBUG
            throwExceptionToolStripMenuItem.Visible = true;
#endif
            userOptions.Load(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\checkasm.cfg");
            
            GACParserDialog gacParser = new GACParserDialog();
            gacAssemblies = gacParser.GetAssembliesList(userOptions.AssembliesInGacCount);

            progressDialog = new ProgressDialog();
            progressDialog.DoWork += new DoWorkEventHandler(progressDialog_DoWork);
            progressDialog.WorkerFinished += new RunWorkerCompletedEventHandler(progressDialog_WorkerFinished);
            progressDialog.ShowDialog();

            if (userOptions.AppWindowState == FormWindowState.Normal)
            {
                if (userOptions.AppSize.Width != 0 && userOptions.AppSize.Height != 0)
                {
                    this.Size = userOptions.AppSize;
                }
                if (!userOptions.AppStartLocation.IsEmpty)
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = userOptions.AppStartLocation;
                }
            }
            if (userOptions.AppWindowState == FormWindowState.Normal || userOptions.AppWindowState == FormWindowState.Maximized)
            {
                WindowState = userOptions.AppWindowState;
            }

            if(userOptions.SplitterPosition != 0)
            {
                this.splitContainer.SplitterDistance = userOptions.SplitterPosition;
            }

            if (userOptions.Splitter2Position != 0)
            {
                this.allResultsSplitContainer.SplitterDistance = userOptions.Splitter2Position;
            }

            if (userOptions.SearchPhrases != null)
            {
                search.Phrases = userOptions.SearchPhrases;
            }

            if (userOptions.AssemblyFilter.Count > 0)
            {
                Filtering fDialog = new Filtering();
                fDialog.Filters = userOptions.AssemblyFilter;
                fDialog.ShowDialog();
            }

            RefreshRecentFiles();

            if(args.Length > 0)
            {
                fileToAnalyze = args[0];
            }
            if (!string.IsNullOrEmpty(fileToAnalyze) && File.Exists(fileToAnalyze))
            {
                workingDir = Path.GetDirectoryName(fileToAnalyze);
                if (!string.IsNullOrEmpty(workingDir))
                {
                    Environment.CurrentDirectory = workingDir;
                }
                OpenFile(fileToAnalyze);
            } 
            else
            {
                if (!string.IsNullOrEmpty(fileToAnalyze))
                {
                    MessageBox.Show("Could not find file \"" + fileToAnalyze + "\"","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }

        void progressDialog_WorkerFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            progressDialog.Close();
        }

        void progressDialog_DoWork(object sender, DoWorkEventArgs e)
        {
            gacFrameworkRedirections = Redirection.GetFrameworkRedirections(gacAssemblies, progressDialog);
        }

        void mi_Click(object sender, EventArgs e)
        {
            OpenFile(((ToolStripMenuItem)sender).Text);
        }

        

        private void AssemblyTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            AsmData asmData = e.Node.Tag as AsmData;
            if (asmData != null)
            {
                asmInfoTextBox.Text = e.Node.Tag.ToString();
                //if (asmData.Validity == AsmData.AsmValidity.Invalid)
                //{
                //    asmInfoTextBox.Text += "File could not be found.";
                //}
            }            
        }

        private void AssemblyTreeView_DragDrop(object sender, DragEventArgs e)
        {
            string[] data = (string[])e.Data.GetData("FileDrop");
            if(data.Length > 0)
            {
                workingDir = Path.GetDirectoryName(data[0]);
                if(!string.IsNullOrEmpty(workingDir))
                {
                    Environment.CurrentDirectory = workingDir;
                }
                
                OpenFile(data[0]);
            }
        }

        private void AssemblyTreeView_DragEnter(object sender, DragEventArgs e)
        {
            this.Activate();
            this.AllowDrop = true;
            string[] formats = e.Data.GetFormats();
            foreach (string f in formats)
            {
                if(f == "FileDrop")
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        /// <summary>
        /// worker start function (start of assembly analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            analyzer.Gac = gacAssemblies;
            //analyzer.Redirections = assemblyRedirections;
            analyzerResult = analyzer.AnalyzeRootAssembly(fileToAnalyze);
            rootAssembly = analyzerResult.RootAssembly;
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (InvokeRequired)
                return;
            if (e.ProgressPercentage < 100)
            {
                progressBar.Value = e.ProgressPercentage;
            }
        }

        /// <summary>
        /// analysis finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ErrorMessageBox.Show("An error occured while processing assembly. " + e.Error.Message, e.Error.ToString());
                if (analyzer != null)
                {
                    AppDomain.Unload(analyzerDomain);
                    analyzerDomain = null;
                    analyzer = null;
                }
            }
            else
            {
                if (InvokeRequired) { Invoke(new MethodInvoker(WorkCompleted)); } else WorkCompleted();
            }
            if (InvokeRequired) { Invoke(new MethodInvoker(SetDefaultState)); } else SetDefaultState();
        }

        private void ExportNodeToText(StreamWriter writer, int indentation, AsmData assembly)
        {
            for (int i = 0; i < indentation; i++)
            {
                writer.Write(' ');
            }
            
            switch (rootAssembly.Validity)
            {
                case AsmData.AsmValidity.Invalid:
                    writer.Write("- ");
                    break;
                case AsmData.AsmValidity.ReferencesOnly:
                    writer.Write("* ");
                    break;
                case AsmData.AsmValidity.CircularDependency:
                    writer.Write("c ");
                    break;
                case AsmData.AsmValidity.Redirected:
                    writer.Write("r ");
                    break;
                case AsmData.AsmValidity.Valid:
                default:
                    writer.Write("+ ");
                    break;
            }

            writer.WriteLine(assembly.AssemblyFullName);
            foreach (AsmData reference in assembly.References)
            {
                ExportNodeToText(writer, indentation + 4, reference);
            }
        }

        private void WorkCompleted()
        {
            try
            {
                adRotator1.Visible = false;
                LoadAsmDataTree();
                ShowProblems();
                if (AssemblyTreeView.Nodes.Count > 0)
                {
                    AssemblyTreeView.Nodes[0].Expand();
                    ExpandRedNodes(AssemblyTreeView.Nodes[0]);
                }
                if (analyzer != null)
                {
                    AppDomain.Unload(analyzerDomain);
                    analyzerDomain = null;
                    analyzer = null;
                }
                progressBar.Value = 100;
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("The results of the analysis could not be processed and displayed in the GUI due to the insufficient memory. The application will try text output now.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                try
                {
                    ExportToText();
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("Not enough memory to finish the operation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowProblems()
        {
            bool wasCollapsed = splitContainer.Panel2Collapsed;

            splitContainer.Panel2Collapsed = false;
            if (analyzerResult == null)
            {
                return;
            }
            listViewProblems.Items.Clear();

            foreach (var problem in analyzerResult.Problems)
            {
                listViewProblems.Items.Add(problem.GetListViewItem());
            }
            if (listViewProblems.Items.Count > 0)
            {
                listViewProblems.Items[0].Selected = true;
            }
            else
            {
                problemDescriptorControl1.SelectedObject = null;
            }
            if (wasCollapsed) //the first time the results bar is displayed, the splitter position needs to be set again
            {
                if (userOptions.SplitterPosition != 0)
                {
                    this.splitContainer.SplitterDistance = userOptions.SplitterPosition;
                }

                if (userOptions.Splitter2Position != 0)
                {
                    this.allResultsSplitContainer.SplitterDistance = userOptions.Splitter2Position;
                }
            }
        }

        private void SetDefaultState()
        {
            progressBar.Value = 0;
            progressBar.Visible = false;
            refreshToolStripMenuItem.Enabled = true;
            toolStripStatusLabel1.Text = "Ready";
            this.Cursor = System.Windows.Forms.Cursors.Default;
            processing = false;
        }

        /// <summary>
        /// expands all nodes with red icons in AssemblyTreeView
        /// </summary>
        /// <param name="parentNode"></param>
        private bool ExpandRedNodes(TreeNode rootNode)
        {
            bool ret = false;
            
            if (rootNode.ImageIndex != 0)
                ret = true;

            foreach (TreeNode subNode in rootNode.Nodes)
            {
                bool expand = ExpandRedNodes(subNode);
                if (expand)
                {
                    subNode.Expand();
                    ret = true;
                }
            }
            return ret;
        }

        private void LoadAsmDataTree(bool showValid)
        {
            treeDepth = 0;
            maxTreeDepth = 0;
            if (rootAssembly != null)
            {
                AssemblyTreeView.Nodes.Clear();
                TreeNode rootNode = new TreeNode(rootAssembly.RuntimeVersionShort + "   " +  rootAssembly.AssemblyFullName);
                rootNode.ContextMenuStrip = treeNodeContextMenu;
                rootNode.Tag = rootAssembly;
                switch (rootAssembly.Validity)
                {
                    case AsmData.AsmValidity.Invalid:
                        rootNode.SelectedImageIndex = 1;
                        rootNode.ImageIndex = 1;
                        
                        break;
                    case AsmData.AsmValidity.ReferencesOnly:
                        rootNode.SelectedImageIndex = 2;
                        rootNode.ImageIndex = 2;
                        
                        break;
                    case AsmData.AsmValidity.CircularDependency:
                        rootNode.SelectedImageIndex = 3;
                        rootNode.ImageIndex = 3;
                        
                        break;
                    case AsmData.AsmValidity.Redirected:
                        rootNode.SelectedImageIndex = 4;
                        rootNode.ImageIndex = 4;
                        break;
                    case AsmData.AsmValidity.Valid:
                    default:
                        rootNode.SelectedImageIndex = 0;
                        rootNode.ImageIndex = 0;
                        break;
                }
                
                LoadNode(rootNode, rootAssembly, showValid);
                AssemblyTreeView.Nodes.Add(rootNode);
                if(searchForm != null)
                {
                    searchForm.RootNode = rootNode;
                }    
            }
            
            CreateShowLevelsMenu();
        }

        /// <summary>
        /// Loads AsmData tree into the AssemblyTreeView
        /// </summary>
        private void LoadAsmDataTree()
        {
            hideAssembliesWithNoIssuesToolStripMenuItem.Checked = false;
            LoadAsmDataTree(true);
        }

        private bool IsInFilter(string assemblyFullName)
        {
            if (filterOverrideEnabled)
                return false;

            foreach (string f in userOptions.AssemblyFilter)
            {
                if (assemblyFullName.Contains(f))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// recuring fuction. loads data from AsmData item to a TreeNode item
        /// </summary>
        /// <param name="node"></param>
        /// <param name="asm"></param>
        private void LoadNode(TreeNode node, AsmData asm, bool showValid)
        {
            treeDepth++;
            if (maxTreeDepth < treeDepth)
                maxTreeDepth = treeDepth;

            foreach(AsmData asmRef in asm.References)
            {
                if (IsInFilter(asmRef.AssemblyFullName))
                {
                    continue;
                }
                TreeNode n = new TreeNode(asmRef.RuntimeVersionShort + "   " + asmRef.AssemblyFullName);
                n.Tag = asmRef;
                n.ContextMenuStrip = treeNodeContextMenu;
                bool visible = showValid;
                switch(asmRef.Validity)
                {
                    case AsmData.AsmValidity.Invalid:
                        n.SelectedImageIndex = 1;
                        n.ImageIndex = 1;
                        visible = true;
                        break;
                    case AsmData.AsmValidity.ReferencesOnly:
                        n.SelectedImageIndex = 2;
                        n.ImageIndex = 2;
                        visible = true;
                        break;
                    case AsmData.AsmValidity.CircularDependency:
                        n.SelectedImageIndex = 3;
                        n.ImageIndex = 3;
                        visible = true;
                        break;
                    case AsmData.AsmValidity.Redirected:
                        n.SelectedImageIndex = 4;
                        n.ImageIndex = 4;
                        break;
                    case AsmData.AsmValidity.Valid:
                    default:
                        n.SelectedImageIndex = 0;
                        n.ImageIndex = 0;
                        break;
                }
                if (visible)
                {
                    LoadNode(n, asmRef, showValid);
                    node.Nodes.Add(n);
                }
            }
            treeDepth--;
        }

        private void CreateShowLevelsMenu()
        {
            showLevelsToolStripMenuItem.Enabled = true;
            showLevelsToolStripMenuItem.DropDownItems.Clear();
            for (int i = 1; i < maxTreeDepth; i++)
            {
                ToolStripMenuItem showLevelsToolsTripSubMenuItem = new ToolStripMenuItem(i.ToString());
                showLevelsToolStripMenuItem.DropDownItems.Add(showLevelsToolsTripSubMenuItem);
                showLevelsToolsTripSubMenuItem.Click += new EventHandler(showLevelsToolsTripSubMenuItem_Click);
            }
        }

        void showLevelsToolsTripSubMenuItem_Click(object sender, EventArgs e)
        {
            requestedTreeDepth = Convert.ToInt32(((ToolStripMenuItem)sender).Text);
            if (AssemblyTreeView.Nodes.Count == 0)
                return;
            Cursor = Cursors.WaitCursor;
            TreeNode root = AssemblyTreeView.Nodes[0];
            root.Collapse(false);
            ShowLevelsExpandNode(root);
            treeDepth = 0;
            Cursor = Cursors.Default;
        }

        void ShowLevelsExpandNode(TreeNode node)
        {
            if (treeDepth >= requestedTreeDepth)
                return;
            treeDepth++;

            node.Expand();
            foreach (TreeNode current in node.Nodes)
            {
                ShowLevelsExpandNode(current);
            }

            treeDepth--;
        }


        /// <summary>
        /// Loads file and starts analysis
        /// </summary>
        /// <param name="file"></param>
        public void OpenFile(string file)
        {
            if(processing)
            {
                MessageBox.Show("Can not load new assembly while parsing another one. Please wait for the task to finish.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Debug.Assert(analyzer == null, "Analyzer still not released.");
            Debug.Assert(analyzerDomain == null, "Analyzer Domain not unloaded");

            if (searchForm != null)
            {
                searchForm.Close();
                searchForm = null;
            }
            if (assemblyRedirections != null)
            {
                assemblyRedirections.Clear();
            }
            string appConfigFile = Redirection.FindConfigFile(file);
            if (!string.IsNullOrEmpty(appConfigFile))
            {
                try
                {
                    assemblyRedirections = Redirection.GetVersionRedirections(appConfigFile);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed to read assembly config file " + ex);
                    MessageBox.Show("Application configuration file contains errors. Make sure it is valid XML.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (assemblyRedirections == null && gacFrameworkRedirections !=null)
            {
                assemblyRedirections = new Dictionary<string, List<Redirection>>(gacFrameworkRedirections);
            }
            else if (gacFrameworkRedirections != null)
            {
                foreach (KeyValuePair<string, List<Redirection>> redirection in gacFrameworkRedirections)
                {
                    assemblyRedirections.Add(redirection.Key, redirection.Value);
                }
            }

            if (File.Exists(file))
            {
                workingDir = Path.GetDirectoryName(file);
                if (!string.IsNullOrEmpty(workingDir))
                {
                    Environment.CurrentDirectory = workingDir;
                }
                string callingDomainName = Thread.GetDomain().FriendlyName;
                string exeAssembly = Assembly.GetEntryAssembly().FullName;
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = Environment.CurrentDirectory;
                setup.DisallowBindingRedirects = false;
                setup.DisallowCodeDownload = true;
                setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                analyzerDomain = AppDomain.CreateDomain("Analyzer");
                analyzerDomain.UnhandledException += new UnhandledExceptionEventHandler(analyzerDomain_UnhandledException);
                analyzer = (Analyzer)analyzerDomain.CreateInstanceAndUnwrap(exeAssembly, typeof(Analyzer).FullName);
                analyzer.BgWorker = bgWorker;
                string message;
                if (!analyzer.IsValidAssembly(file, out message))
                {
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (analyzer != null)
                    {
                        AppDomain.Unload(analyzerDomain);
                        analyzerDomain = null;
                        analyzer = null;
                    }
                    return;
                }
                processing = true;
                listViewProblems.Items.Clear();
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                toolStripStatusLabel1.Text = "Working...";
                progressBar.Value = 0;
                progressBar.Visible = true;
                asmInfoTextBox.Text = "";
                refreshToolStripMenuItem.Enabled = true;
                fileToAnalyze = file;
                userOptions.AddRecentFile(file);
                RefreshRecentFiles();
                bgWorker.RunWorkerAsync(fileToAnalyze);
            }
            else
            {
                MessageBox.Show(string.Format("File {0} could not be found.", file), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void analyzerDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorMessageBox.Show("CheckAsm has encountered an error from which is unable to recover. Try to restart the application and repeat your action again. ", e.ExceptionObject.ToString());
            Environment.Exit(1);
        }

        private void RefreshRecentFiles()
        {
            var removed = new List<string>();
            if(userOptions.RecentFiles.Count > 0)
            {
                recentFilesToolStripMenuItem.Visible = true;
                recentFilesSeparator.Visible = true;
                recentFilesToolStripMenuItem.DropDownItems.Clear();
                foreach (string file in userOptions.RecentFiles)
                {
                    if (File.Exists(file))
                    {
                        ToolStripMenuItem mi = new ToolStripMenuItem(file);
                        mi.Click += new EventHandler(mi_Click);
                        recentFilesToolStripMenuItem.DropDownItems.Add(mi);
                    }
                    else
                    {
                        removed.Add(file);
                    }
                }
            } 
            else
            {
                recentFilesToolStripMenuItem.Visible = false;
                recentFilesSeparator.Visible = false;
            }
            foreach (var item in removed)
            {
                userOptions.RecentFiles.Remove(item);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile(fileToAnalyze);
        }

        private void checkAsmOnlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.amberfish.net");
            }
            catch
            {
                MessageBox.Show("Cannot open URL http://www.amberfish.net.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Thread checkUpdatesThread = new Thread(() => CheckForUpdates());
            checkUpdatesThread.IsBackground = true;
            checkUpdatesThread.Name = "CheckForUpdates Thread";
            checkUpdatesThread.Start();

            socialBar.Visible = Settings.Default.DisplaySocialBar;

#if !FREE
            adRotator1.Visible = false;
#endif
#if FREE
            if (Environment.TickCount % 2 == 0)
            {
                adRotator1.Target = "http://www.dropici.cz/?src=checkasm";
                adRotator1.Images.Add(Resources.banner1);
                adRotator1.Images.Add(Resources.banner2);
                adRotator1.Images.Add(Resources.banner3);
                adRotator1.SetInitialImage(Resources.banner1);
            }
            else
            {
                adRotator1.Target = "http://www.amberfish.net/keylock/?src=checkasm";
                adRotator1.Images.Add(Resources.keylock1);
                adRotator1.Images.Add(Resources.keylock2);
                adRotator1.Images.Add(Resources.keylock3);
                adRotator1.Images.Add(Resources.keylock4);
                adRotator1.SetInitialImage(Resources.keylock1);
            }
#endif
        }

        private void CheckForUpdates()
        {
#if EVAL
            return;
#endif
            string downloadedManifestFileName = null;
            try
            {
                Trace.WriteLine("Checking for updates...");
                WebClient client = new WebClient();
                downloadedManifestFileName = Path.GetTempFileName();
                client.DownloadFile(Settings.Default.UpdateManifestUrl, downloadedManifestFileName);
                XDocument doc = XDocument.Load(downloadedManifestFileName);
                XPathNavigator navigator = doc.CreateNavigator();
                var versionNode = navigator.SelectSingleNode(@"//checkasm/currentVersion/version");
                if (versionNode != null)
                {
                    var latestVersion = new Version(versionNode.Value);
                    if (latestVersion > Assembly.GetEntryAssembly().GetName().Version)
                    {
                        Trace.WriteLine("New version is available: " + latestVersion);
                        string updateInfoUrl = "http://www.amberfish.net/download.aspx?scr=checkasm";
#if FREE
                        var updateUrlNode = navigator.SelectSingleNode(@"//checkasm/currentVersion/trialUpdateUrl");
#endif
#if !FREE
                        var updateUrlNode = navigator.SelectSingleNode(@"//checkasm/currentVersion/updateUrl");
#endif
                        var descriptionNode = navigator.SelectSingleNode(@"//checkasm/currentVersion/details");

                        if (updateUrlNode != null && !string.IsNullOrEmpty(updateUrlNode.Value))
                        {
                            updateInfoUrl = updateUrlNode.Value;
                        }
                        ShowUpdateAvailable(latestVersion.ToString(), updateInfoUrl, descriptionNode.Value);
                    }
                    else
                    {
                        Trace.WriteLine("CheckAsm is up to date");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Cannot check for updates. Failed to download manifest file. " + ex);
            }
            finally
            {
                if(!string.IsNullOrEmpty(downloadedManifestFileName) && File.Exists(downloadedManifestFileName))
                {
                    try
                    {
                        File.Delete(downloadedManifestFileName);
                    }
                    catch{}
                }
            }
        }

        private void ShowUpdateAvailable(string latestVersion, string updateInfoUrl, string description)
        {
            var updateInfo = new UpdateInfo 
            { 
                Description = description,
                LatestVersion = latestVersion,
                UpdateUrl = updateInfoUrl
            };
            updateIsAvailableToolStripMenuItem.Tag = updateInfo;
            Invoke(new MethodInvoker(() => updateIsAvailableToolStripMenuItem.Visible = true));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (processing)
            {
                MessageBox.Show("Application can not stop while processing an assembly. Please wait for the task to finish.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                userOptions.AppSize = this.Size;
                userOptions.AppStartLocation = this.Location;
            }

            userOptions.SplitterPosition = this.splitContainer.SplitterDistance;
            userOptions.Splitter2Position = this.allResultsSplitContainer.SplitterDistance;
            userOptions.AppWindowState = WindowState;

            if (gacAssemblies != null)
                userOptions.AssembliesInGacCount = this.gacAssemblies.Count;

            userOptions.SearchPhrases = search.Phrases;

            try
            {
                userOptions.Save(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\checkasm.cfg");
                Settings.Default.Save();
            }
            catch (Exception) { } //there are actually no user settings, so just ignore the error
            Trace.Flush();
            base.OnFormClosing(e);
        }

        private void iconsDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpPage hlp = new HelpPage();
            hlp.ShowDialog();
        }

        private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("mailto:support@amberfish.net?subject=bug%20report&body=Please%20describe%20problem%20behavior%20below:");
            }
            catch
            {
                MessageBox.Show("Please report all bugs to support@amberfish.net.");
            }
        }

        private void collapseNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedNode != null)
            {
                clickedNode.Collapse(true);
                AssemblyTreeView.SelectedNode = clickedNode;
            }
        }

        private void expandNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedNode != null)
            {
                clickedNode.Expand();
                AssemblyTreeView.SelectedNode = clickedNode;
            }
        }

        private void collapseNodeAndSubnodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedNode != null)
            {
                clickedNode.Collapse(false);
                AssemblyTreeView.SelectedNode = clickedNode;
            }
        }

        private void expandNodeAndSubnodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedNode != null)
            {
                clickedNode.ExpandAll();
                AssemblyTreeView.SelectedNode = clickedNode;
            }
        }

        TreeNode clickedNode = null;

        private void AssemblyTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            clickedNode = e.Node;
        }

        private void collapseNodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            collapseNodeToolStripMenuItem_Click(sender, e);
        }

        private void expandNodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            expandNodeToolStripMenuItem_Click(sender, e);
        }

        private void collapseNodeAndChildNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            collapseNodeAndSubnodesToolStripMenuItem_Click(sender, e);
        }

        private void expandNodeAndChildNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            expandNodeAndSubnodesToolStripMenuItem_Click(sender, e);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode rnode = null;
            if(AssemblyTreeView.Nodes.Count > 0)
            {
                rnode = AssemblyTreeView.Nodes[0];
            }
            
            if (searchForm == null)
            {
                searchForm = new SearchForm(search, rnode);
                searchForm.StartPosition = FormStartPosition.Manual;
                int x, y;
                x = this.Location.X + (this.Width / 2) - (searchForm.Width / 2);
                y = this.Location.Y + (this.Height / 2) - (searchForm.Height / 2);
                searchForm.Location = new Point(x, y);
                searchForm.FormClosed += new FormClosedEventHandler(searchForm_FormClosed);
            }
            searchForm.Show(this);
            searchForm.BringToFront();
        }

        void searchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            searchForm = null;
        }

        private void filteringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtering filterDialog = new Filtering();
            filterDialog.Filters = userOptions.AssemblyFilter;
            if (filterDialog.ShowDialog() == DialogResult.OK)
            {
                SaveTreeState();
                LoadAsmDataTree();
                LoadTreeState();
            }

        }

        private void doNotShowThisAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedNode != null)
            {
                var asm = clickedNode.Tag as AsmData;
                if(asm != null)
                {
                    userOptions.AssemblyFilter.Add(asm.AssemblyFullName);
                    SaveTreeState();
                    LoadAsmDataTree();
                    LoadTreeState();
                }
            }
        }

        private void listGACToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (gacAssemblies != null)
            {
                GacListForm frm = new GacListForm();
                frm.DataSource = gacAssemblies;
                frm.Show();
            }
            else
            {
                MessageBox.Show("GAC list is not available. Please try to restart the application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void scanDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if FREE
            var dr = MessageBox.Show("This feature is not available in the free version. Visit our website to get the full version.", "CheckAsm", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                try
                {
                    Process.Start("http://www.amberfish.net/Paypal.aspx");
                }
                catch
                {

                }
            }

#endif
#if !FREE
            DirectoryScanResults scanResults = new DirectoryScanResults(UserSettings.ScannerOptions);
            scanResults.ApplicationWindow = this;
            scanResults.GacAssemblies = gacAssemblies;
            scanResults.Redirections = gacFrameworkRedirections;
            scanResults.Show();
#endif
        }


        private void throwExceptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
        #if DEBUG
            throw new Exception("test exception");
        #endif
        }

        private void analyzeReferencesWithinDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if FREE
            var dr = MessageBox.Show("This feature is not available in the free version. Visit our website to get the full version.", "CheckAsm", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                try
                {
                    Process.Start("http://www.amberfish.net/Paypal.aspx");
                }
                catch
                {

                }
            }

#endif

#if !FREE
            AnalyzeDirReferencesForm dialog = new AnalyzeDirReferencesForm(UserSettings.DirReferenceAnalyzerParameters);
            dialog.GacAssemblies = gacAssemblies;
            dialog.ApplicationWindow = this;
            dialog.Show();
#endif
        }

        private void protectYourComputerFromBabiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.amberfish.net/keylock/default.aspx?source=checkasm");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to start process.\r\n" + ex);
                MessageBox.Show("Visit http://www.amberfish.net/keylock");
            }
        }

        private void ExportNodeToText(StreamWriter writer, int indentation, AsmData assembly, bool includeValidity)
        {
            if (this.IsInFilter(assembly.AssemblyFullName))
            {
                return;
            }
            for (int i = 0; i < indentation; i++)
            {
                writer.Write(' ');
            }
            if (includeValidity)
            {
                switch (this.rootAssembly.Validity)
                {
                    case AsmData.AsmValidity.ReferencesOnly:
                        writer.Write("* ");
                        goto Label_008E;

                    case AsmData.AsmValidity.Invalid:
                        writer.Write("- ");
                        goto Label_008E;

                    case AsmData.AsmValidity.CircularDependency:
                        writer.Write("c ");
                        goto Label_008E;

                    case AsmData.AsmValidity.Redirected:
                        writer.Write("r ");
                        goto Label_008E;
                }
                writer.Write("+ ");
            }
        Label_008E:
            writer.WriteLine(assembly.AssemblyFullName);
            foreach (AsmData data in assembly.References)
            {
                this.ExportNodeToText(writer, indentation + 4, data, includeValidity);
            }
        }

        private void ExportToFlatList(string fileName)
        {
            Queue<AsmData> queue = new Queue<AsmData>();
            queue.Enqueue(this.rootAssembly);
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        do
                        {
                            AsmData assembly = queue.Dequeue();
                            foreach (AsmData data2 in assembly.References)
                            {
                                queue.Enqueue(data2);
                            }
                            if (!dictionary.ContainsKey(assembly.AssemblyFullName))
                            {
                                this.WriteAsmDataText(writer, assembly);
                                dictionary.Add(assembly.AssemblyFullName, true);
                            }
                        }
                        while (queue.Count != 0);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("The text output could not be written to the file " + fileName + ". Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void ExportToText()
        {
            string fileName = Path.GetTempFileName() + ".txt";
            this.ExportToText(fileName, true, true);
        }

        private void ExportToText(string fileName, bool startNotepad, bool includeValidity)
        {
            try
            {
                using (FileStream stream = File.OpenWrite(fileName))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        if (includeValidity)
                        {
                            writer.WriteLine(" - invalid, * problems with DLL imports, c Circular dependency detected, r Redirected, + Valid");
                        }
                        this.ExportNodeToText(writer, 0, this.rootAssembly, includeValidity);
                        writer.Close();
                    }
                    stream.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("The text output could not be written to the file " + fileName + ". Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (startNotepad)
            {
                try
                {
                    Process.Start("notepad.exe", fileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("The application failed to start notepad. The text output has been saved to " + fileName + ".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void WriteAsmDataText(StreamWriter writer, AsmData assembly)
        {
            string line = string.Format("{0}\t{1}\t{2}\t{3}",
                AssemblyStatusTextProvider.GetText(assembly.Validity),
                 assembly.AssemblyFullName.Substring(0, assembly.AssemblyFullName.IndexOf(", Version=")),
                assembly.AssemblyFullName.Substring(assembly.AssemblyFullName.IndexOf(", Version=") + 10, assembly.AssemblyFullName.IndexOf(", Culture=") - assembly.AssemblyFullName.IndexOf(", Version=") - 10),
                assembly.Path);
            if (!this.IsInFilter(assembly.AssemblyFullName))
            {
                writer.WriteLine(line);
            }
        }

        /*
         select new ListViewItem(new[] { item.AssemblyFullName.Substring(0, item.AssemblyFullName.IndexOf(", Version=")),
                            item.AssemblyFullName.Substring(item.AssemblyFullName.IndexOf(", Version=") + 10, 
         item.AssemblyFullName.IndexOf(", Culture=") - item.AssemblyFullName.IndexOf(", Version=") - 10),
                            item.Path }, (int)item.Validity) { Tag = item };
         */

        private void hierarchyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.rootAssembly == null)
            {
                MessageBox.Show("There is nothing to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                this.exportDialog.FileName = "";
                this.exportDialog.Title = "Export as text file with tree hierarchy";
                this.exportDialog.Filter = "Text files|*.txt|All files|*.*";
                if (this.exportDialog.ShowDialog() == DialogResult.OK)
                {
                    this.ExportToText(this.exportDialog.FileName, false, false);
                }
            }
        }

        private void flatListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.rootAssembly == null)
            {
                MessageBox.Show("There is nothing to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                this.exportDialog.FileName = "";
                this.exportDialog.Title = "Export as flat list";
                this.exportDialog.Filter = "Text files|*.txt|All files|*.*";
                if (this.exportDialog.ShowDialog() == DialogResult.OK)
                {
                    this.ExportToFlatList(this.exportDialog.FileName);
                }
            }
        }

        private void RecordFirstTimeRun()
        {
            //if (Settings.Default.EnableUsageStats)
            //{
            //    Thread thread = new Thread(()=>
            //    {
            //        try
            //        {
            //            new Stats().RecordInstallation(userOptions.CustomerId.ToString());
            //        }
            //        catch
            //        {
            //        }
            //    })
            //    {
            //        IsBackground = true
            //    };
            //    thread.SetApartmentState(ApartmentState.STA);
            //    thread.Start();
            //}
        }

        private void RecordStart()
        {
            //if (Settings.Default.EnableUsageStats)
            //{
            //    Thread thread = new Thread(()=>
            //    {
            //        try
            //        {
            //            new Stats().RecordStart(userOptions.CustomerId.ToString());
            //        }
            //        catch
            //        {
            //        }
            //    })
            //    {
            //        IsBackground = true
            //    };
            //    thread.SetApartmentState(ApartmentState.STA);
            //    thread.Start();
            //}
        }

        private void updateIsAvailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var updateInfo = (UpdateInfo)item.Tag;
            try
            {
                var dialog = new UpdateAvailableDialog 
                {
                    UpdateTextInfo = updateInfo
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Process.Start(updateInfo.UpdateUrl);
                }
            }
            catch 
            {
                MessageBox.Show("A new version has been released. Visit http://www.amberfish.net to get it!", "Update Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void trashFinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if FREE
            var dr = MessageBox.Show("This feature is not available in the free version. Visit our website to get the full version.", "CheckAsm", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                try
                {
                    Process.Start("http://www.amberfish.net/Paypal.aspx");
                }
                catch
                { }
            }

#endif

#if !FREE
            UserSettings.DirReferenceAnalyzerParameters.GacAssemblies = gacAssemblies;
            var dialog = new TrashFinderForm(UserSettings.DirReferenceAnalyzerParameters);
            dialog.Show();
#endif
        }

        private void onlineDocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.amberfish.net/help/default.aspx");
            }
            catch
            { }
        }

        private void hideAssembliesWithNoIssuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideAssembliesWithNoIssuesToolStripMenuItem.Checked = !hideAssembliesWithNoIssuesToolStripMenuItem.Checked;
            LoadAsmDataTree(!hideAssembliesWithNoIssuesToolStripMenuItem.Checked);
            if (AssemblyTreeView.Nodes.Count > 0)
            {
                AssemblyTreeView.Nodes[0].Expand();
                ExpandRedNodes(AssemblyTreeView.Nodes[0]);
            }
        }

        private void registerForUpdateNotificationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.amberfish.net/Register.aspx");
            }
            catch
            {
            }
        }

        private void AssemblyTreeView_DragOver(object sender, DragEventArgs e)
        {

        }

        private void asmInfoTextBox_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void MainDialog_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void listBoxProblems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewProblems.SelectedItems.Count>0)
            {
                problemDescriptorControl1.SelectedObject = (ProblemDescriptor)listViewProblems.SelectedItems[0].Tag;
            }
        }

        private void listViewProblems_Resize(object sender, EventArgs e)
        {
            listViewProblems.Columns[2].Width = listViewProblems.Width - listViewProblems.Columns[0].Width - listViewProblems.Columns[1].Width - 2;
        }

        private void problemDescriptorControl1_NavigatePressed(object sender, EventArgs e)
        {
            TreeNode rnode;
            if (AssemblyTreeView.Nodes.Count == 0)
                    rnode = new TreeNode("");
                else
                    rnode = AssemblyTreeView.Nodes[0];

            var descriptor = (ProblemDescriptor)problemDescriptorControl1.SelectedObject;
            var asmName = descriptor.Source.AssemblyFullName;
            var result = search.Find(asmName, rnode);
            if (result.Count > 0)
            {
                AssemblyTreeView.SelectedNode = result[0];
                AssemblyTreeView.Focus();
            }
        }

        private void disableAllFiltersToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            filterOverrideEnabled = disableAllFiltersToolStripMenuItem.Checked;
            SaveTreeState();
            LoadAsmDataTree();
            LoadTreeState();
        }

        readonly Dictionary<string, bool> treeState = new Dictionary<string, bool>();

        private void LoadTreeState()
        {
            if(AssemblyTreeView.Nodes.Count > 0 && treeState.Count > 0)
            {
                if (treeState.ContainsKey(AssemblyTreeView.Nodes[0].FullPath))
                {
                    var rootNodeExpanded = treeState[AssemblyTreeView.Nodes[0].FullPath];
                    if (rootNodeExpanded)
                        AssemblyTreeView.Nodes[0].Expand();
                    else
                        AssemblyTreeView.Nodes[0].Collapse();
                }
                LoadTreeNodeState(AssemblyTreeView.Nodes[0]);
            }
        }

        private void LoadTreeNodeState(TreeNode treeNode)
        {
            foreach (TreeNode child in treeNode.Nodes)
            {
                if(treeState.ContainsKey(child.FullPath))
                {
                    if (treeState[child.FullPath])
                        child.Expand();
                    else
                        child.Collapse();
                }                
            }
        }

        private void SaveTreeState()
        {
            treeState.Clear();
            if(AssemblyTreeView.Nodes.Count > 0)
            {
                treeState.Add(AssemblyTreeView.Nodes[0].FullPath, AssemblyTreeView.Nodes[0].IsExpanded);
                SaveTreeNodeState(AssemblyTreeView.Nodes[0]);
            }
        }

        private void SaveTreeNodeState(TreeNode treeNode)
        {
            foreach(TreeNode child in treeNode.Nodes)
            {
                if (!treeState.ContainsKey(child.FullPath))
                {
                    treeState.Add(child.FullPath, child.IsExpanded);
                }
                SaveTreeNodeState(child);
            }
        }

        private void showLoadedAssembliesInAListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rootAssembly != null)
            {
                ListFormBase frm = new ListFormBase();
                frm.Text = "Assemblies referenced by " + rootAssembly.Name;
                frm.DataSource = GetFlatList();
                frm.Show();
            }
            else
            {
                MessageBox.Show("There are no assembies loaded right now. Analyze an assembly first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        List<AsmData> GetFlatList()
        {
            Dictionary<string, AsmData> dict = new Dictionary<string, AsmData>();
            AddToList(rootAssembly, dict);
            var list = new List<AsmData>();
            foreach(var item in dict.Values)
            {
                list.Add(item);
            }
            list.Add(rootAssembly);
            return list;
        }

        private void AddToList(AsmData rootAssembly, Dictionary<string, AsmData> list)
        {
            foreach(var r in rootAssembly.References)
            {
                if(!list.ContainsKey(r.AssemblyFullName))
                {
                    list.Add(r.AssemblyFullName, r);
                }
                AddToList(r, list);
            }
        }
    }
}