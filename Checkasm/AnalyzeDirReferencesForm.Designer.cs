namespace CheckAsm
{
    partial class AnalyzeDirReferencesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalyzeDirReferencesForm));
            this.browseButton = new System.Windows.Forms.Button();
            this.folderTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.startButton = new System.Windows.Forms.Button();
            this.containerPanel = new System.Windows.Forms.Panel();
            this.canvas = new Amberfish.Canvas.Views.CanvasView();
            this.nodeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.analyzeBringToMainWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.setColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReverseLookupToolstripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSymmetrical = new System.Windows.Forms.ToolStripButton();
            this.btnHierarchical = new System.Windows.Forms.ToolStripButton();
            this.btnCircular = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.lblTip = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.containerPanel.SuspendLayout();
            this.nodeContextMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(609, 25);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 7;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // folderTextBox
            // 
            this.folderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folderTextBox.Location = new System.Drawing.Point(58, 27);
            this.folderTextBox.Name = "folderTextBox";
            this.folderTextBox.Size = new System.Drawing.Size(545, 20);
            this.folderTextBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Directory:";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 493);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(777, 22);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 17);
            this.progressBar.Visible = false;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 17);
            this.statusLabel.Text = "Ready";
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.Location = new System.Drawing.Point(690, 24);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 9;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // containerPanel
            // 
            this.containerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.containerPanel.AutoScroll = true;
            this.containerPanel.BackColor = System.Drawing.Color.White;
            this.containerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.containerPanel.Controls.Add(this.canvas);
            this.containerPanel.Location = new System.Drawing.Point(12, 54);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Size = new System.Drawing.Size(753, 407);
            this.containerPanel.TabIndex = 12;
            // 
            // canvas
            // 
            this.canvas.AutoSize = true;
            this.canvas.BackColor = System.Drawing.Color.White;
            this.canvas.Location = new System.Drawing.Point(3, 3);
            this.canvas.Name = "canvas";
            this.canvas.NodeContextMenu = this.nodeContextMenuStrip;
            this.canvas.Size = new System.Drawing.Size(572, 268);
            this.canvas.Suspend = false;
            this.canvas.TabIndex = 13;
            // 
            // nodeContextMenuStrip
            // 
            this.nodeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analyzeBringToMainWindowToolStripMenuItem,
            this.ReverseLookupToolstripItem,
            this.showInExplorerToolStripMenuItem,
            this.toolStripMenuItem1,
            this.setColorToolStripMenuItem});
            this.nodeContextMenuStrip.Name = "nodeContextMenuStrip";
            this.nodeContextMenuStrip.Size = new System.Drawing.Size(244, 120);
            // 
            // analyzeBringToMainWindowToolStripMenuItem
            // 
            this.analyzeBringToMainWindowToolStripMenuItem.Name = "analyzeBringToMainWindowToolStripMenuItem";
            this.analyzeBringToMainWindowToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.analyzeBringToMainWindowToolStripMenuItem.Text = "Analyze (Bring to main window)";
            this.analyzeBringToMainWindowToolStripMenuItem.Click += new System.EventHandler(this.analyzeBringToMainWindowToolStripMenuItem_Click);
            // 
            // showInExplorerToolStripMenuItem
            // 
            this.showInExplorerToolStripMenuItem.Name = "showInExplorerToolStripMenuItem";
            this.showInExplorerToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.showInExplorerToolStripMenuItem.Text = "Show in Explorer";
            this.showInExplorerToolStripMenuItem.Click += new System.EventHandler(this.showInExplorerToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(240, 6);
            // 
            // setColorToolStripMenuItem
            // 
            this.setColorToolStripMenuItem.Name = "setColorToolStripMenuItem";
            this.setColorToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.setColorToolStripMenuItem.Text = "Set Color...";
            this.setColorToolStripMenuItem.Click += new System.EventHandler(this.setColorToolStripMenuItem_Click);
            // 
            // ReverseLookupToolstripItem
            // 
            this.ReverseLookupToolstripItem.Name = "ReverseLookupToolstripItem";
            this.ReverseLookupToolstripItem.Size = new System.Drawing.Size(243, 22);
            this.ReverseLookupToolstripItem.Text = "What is using this assembly?";
            this.ReverseLookupToolstripItem.Click += new System.EventHandler(this.ReverseLookupToolstripItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSymmetrical,
            this.btnHierarchical,
            this.btnCircular,
            this.toolStripSeparator3,
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(777, 25);
            this.toolStrip1.TabIndex = 13;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSymmetrical
            // 
            this.btnSymmetrical.Enabled = false;
            this.btnSymmetrical.Image = global::CheckAsm.Properties.Resources.symmetrical;
            this.btnSymmetrical.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSymmetrical.Name = "btnSymmetrical";
            this.btnSymmetrical.Size = new System.Drawing.Size(132, 22);
            this.btnSymmetrical.Text = "Symmetrical Layout";
            this.btnSymmetrical.Click += new System.EventHandler(this.btnSymmetrical_Click);
            // 
            // btnHierarchical
            // 
            this.btnHierarchical.Enabled = false;
            this.btnHierarchical.Image = global::CheckAsm.Properties.Resources.hierarchical;
            this.btnHierarchical.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHierarchical.Name = "btnHierarchical";
            this.btnHierarchical.Size = new System.Drawing.Size(129, 22);
            this.btnHierarchical.Text = "Hierarchical Layout";
            this.btnHierarchical.Click += new System.EventHandler(this.btnHierarchical_Click);
            // 
            // btnCircular
            // 
            this.btnCircular.Enabled = false;
            this.btnCircular.Image = global::CheckAsm.Properties.Resources.circular;
            this.btnCircular.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCircular.Name = "btnCircular";
            this.btnCircular.Size = new System.Drawing.Size(107, 22);
            this.btnCircular.Text = "Circular Layout";
            this.btnCircular.ToolTipText = "Circular Layout";
            this.btnCircular.Click += new System.EventHandler(this.btnCircular_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Image = global::CheckAsm.Properties.Resources.diskette;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 22);
            this.btnSave.Text = "Save as picture...";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "PNG Files|*.png";
            this.saveFileDialog.Title = "Save as picture...";
            // 
            // colorDialog
            // 
            this.colorDialog.FullOpen = true;
            // 
            // lblTip
            // 
            this.lblTip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTip.AutoSize = true;
            this.lblTip.Location = new System.Drawing.Point(13, 464);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(268, 13);
            this.lblTip.TabIndex = 14;
            this.lblTip.Text = "Tip: Right click the nodes in the graph for more options.";
            // 
            // AnalyzeDirReferencesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 515);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lblTip);
            this.Controls.Add(this.containerPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.folderTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browseButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AnalyzeDirReferencesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Analyze References Within Selected Directory";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.containerPanel.ResumeLayout(false);
            this.containerPanel.PerformLayout();
            this.nodeContextMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox folderTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Panel containerPanel;
        private Amberfish.Canvas.Views.CanvasView canvas;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSymmetrical;
        private System.Windows.Forms.ToolStripButton btnHierarchical;
        private System.Windows.Forms.ContextMenuStrip nodeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem analyzeBringToMainWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setColorToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label lblTip;
        private System.Windows.Forms.ToolStripButton btnCircular;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ReverseLookupToolstripItem;
    }
}