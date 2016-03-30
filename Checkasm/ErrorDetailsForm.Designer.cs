namespace CheckAsm
{
    partial class ErrorDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDetailsForm));
            this.detailsTextbox = new System.Windows.Forms.TextBox();
            this.logFileLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // detailsTextbox
            // 
            this.detailsTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsTextbox.Location = new System.Drawing.Point(12, 12);
            this.detailsTextbox.Multiline = true;
            this.detailsTextbox.Name = "detailsTextbox";
            this.detailsTextbox.Size = new System.Drawing.Size(625, 414);
            this.detailsTextbox.TabIndex = 0;
            // 
            // logFileLinkLabel
            // 
            this.logFileLinkLabel.AutoSize = true;
            this.logFileLinkLabel.Location = new System.Drawing.Point(12, 442);
            this.logFileLinkLabel.Name = "logFileLinkLabel";
            this.logFileLinkLabel.Size = new System.Drawing.Size(127, 13);
            this.logFileLinkLabel.TabIndex = 1;
            this.logFileLinkLabel.TabStop = true;
            this.logFileLinkLabel.Text = "Log file will be attached...";
            this.logFileLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.logFileLinkLabel_LinkClicked);
            // 
            // ErrorDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 464);
            this.Controls.Add(this.logFileLinkLabel);
            this.Controls.Add(this.detailsTextbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ErrorDetailsForm";
            this.Text = "Details";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox detailsTextbox;
        private System.Windows.Forms.LinkLabel logFileLinkLabel;
    }
}