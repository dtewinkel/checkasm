namespace CheckAsm.Controls
{
    partial class ProblemDescriptorControl
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSrcAssemblyTitle = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.lblAdditionalDetailsTitle = new System.Windows.Forms.Label();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.Location = new System.Drawing.Point(3, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(41, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "label1";
            // 
            // lblSrcAssemblyTitle
            // 
            this.lblSrcAssemblyTitle.AutoSize = true;
            this.lblSrcAssemblyTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSrcAssemblyTitle.Location = new System.Drawing.Point(3, 33);
            this.lblSrcAssemblyTitle.Name = "lblSrcAssemblyTitle";
            this.lblSrcAssemblyTitle.Size = new System.Drawing.Size(107, 13);
            this.lblSrcAssemblyTitle.TabIndex = 1;
            this.lblSrcAssemblyTitle.Text = "Source Assembly:";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSource.Location = new System.Drawing.Point(10, 46);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(91, 13);
            this.lblSource.TabIndex = 2;
            this.lblSource.Text = "Source Assembly:";
            // 
            // lblAdditionalDetailsTitle
            // 
            this.lblAdditionalDetailsTitle.AutoSize = true;
            this.lblAdditionalDetailsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAdditionalDetailsTitle.Location = new System.Drawing.Point(3, 86);
            this.lblAdditionalDetailsTitle.Name = "lblAdditionalDetailsTitle";
            this.lblAdditionalDetailsTitle.Size = new System.Drawing.Size(110, 13);
            this.lblAdditionalDetailsTitle.TabIndex = 3;
            this.lblAdditionalDetailsTitle.Text = "Additional Details:";
            // 
            // txtDetails
            // 
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.Location = new System.Drawing.Point(13, 102);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDetails.Size = new System.Drawing.Size(287, 74);
            this.txtDetails.TabIndex = 4;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(10, 61);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(75, 13);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Show in tree...";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // ProblemDescriptorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.lblAdditionalDetailsTitle);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.lblSrcAssemblyTitle);
            this.Controls.Add(this.lblTitle);
            this.Name = "ProblemDescriptorControl";
            this.Size = new System.Drawing.Size(317, 190);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSrcAssemblyTitle;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label lblAdditionalDetailsTitle;
        private System.Windows.Forms.TextBox txtDetails;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
