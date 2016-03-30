namespace CheckAsm
{
    partial class SocialBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SocialBar));
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.btnTwitter = new System.Windows.Forms.PictureBox();
            this.btnMail = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelHorizontalBottomLine = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTwitter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMail)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Get notified about a new version: ";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(319, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(10, 10);
            this.btnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnClose.TabIndex = 1;
            this.btnClose.TabStop = false;
            this.toolTip.SetToolTip(this.btnClose, "Hide");
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnTwitter
            // 
            this.btnTwitter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTwitter.Image = ((System.Drawing.Image)(resources.GetObject("btnTwitter.Image")));
            this.btnTwitter.Location = new System.Drawing.Point(168, 2);
            this.btnTwitter.Name = "btnTwitter";
            this.btnTwitter.Size = new System.Drawing.Size(21, 21);
            this.btnTwitter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnTwitter.TabIndex = 2;
            this.btnTwitter.TabStop = false;
            this.toolTip.SetToolTip(this.btnTwitter, "Follow us on twitter...");
            this.btnTwitter.Click += new System.EventHandler(this.btnTwitter_Click);
            // 
            // btnMail
            // 
            this.btnMail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMail.Image = ((System.Drawing.Image)(resources.GetObject("btnMail.Image")));
            this.btnMail.Location = new System.Drawing.Point(197, 5);
            this.btnMail.Name = "btnMail";
            this.btnMail.Size = new System.Drawing.Size(21, 15);
            this.btnMail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnMail.TabIndex = 3;
            this.btnMail.TabStop = false;
            this.toolTip.SetToolTip(this.btnMail, "Register to get updates via e-mail...");
            this.btnMail.Click += new System.EventHandler(this.btnMail_Click);
            // 
            // panelHorizontalBottomLine
            // 
            this.panelHorizontalBottomLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(190)))));
            this.panelHorizontalBottomLine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelHorizontalBottomLine.Location = new System.Drawing.Point(0, 24);
            this.panelHorizontalBottomLine.Name = "panelHorizontalBottomLine";
            this.panelHorizontalBottomLine.Size = new System.Drawing.Size(338, 1);
            this.panelHorizontalBottomLine.TabIndex = 4;
            // 
            // SocialBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(231)))));
            this.Controls.Add(this.panelHorizontalBottomLine);
            this.Controls.Add(this.btnMail);
            this.Controls.Add(this.btnTwitter);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label1);
            this.Name = "SocialBar";
            this.Size = new System.Drawing.Size(338, 25);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTwitter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.PictureBox btnTwitter;
        private System.Windows.Forms.PictureBox btnMail;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panelHorizontalBottomLine;
    }
}
