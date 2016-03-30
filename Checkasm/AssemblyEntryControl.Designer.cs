namespace CheckAsm
{
    partial class AssemblyEntryControl
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
            this.assemblyNameLabel = new System.Windows.Forms.Label();
            this.assemblyFullNameLabel = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // assemblyNameLabel
            // 
            this.assemblyNameLabel.AutoSize = true;
            this.assemblyNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.assemblyNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.assemblyNameLabel.Location = new System.Drawing.Point(3, 3);
            this.assemblyNameLabel.Name = "assemblyNameLabel";
            this.assemblyNameLabel.Size = new System.Drawing.Size(51, 16);
            this.assemblyNameLabel.TabIndex = 0;
            this.assemblyNameLabel.Text = "label1";
            // 
            // assemblyFullNameLabel
            // 
            this.assemblyFullNameLabel.AutoSize = true;
            this.assemblyFullNameLabel.Location = new System.Drawing.Point(19, 23);
            this.assemblyFullNameLabel.Name = "assemblyFullNameLabel";
            this.assemblyFullNameLabel.Size = new System.Drawing.Size(35, 13);
            this.assemblyFullNameLabel.TabIndex = 1;
            this.assemblyFullNameLabel.Text = "label1";
            // 
            // linkLabel
            // 
            this.linkLabel.AutoSize = true;
            this.linkLabel.Location = new System.Drawing.Point(384, 5);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new System.Drawing.Size(47, 13);
            this.linkLabel.TabIndex = 3;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "Check...";
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // AssemblyEntryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.linkLabel);
            this.Controls.Add(this.assemblyFullNameLabel);
            this.Controls.Add(this.assemblyNameLabel);
            this.MaximumSize = new System.Drawing.Size(5000, 50);
            this.MinimumSize = new System.Drawing.Size(0, 50);
            this.Name = "AssemblyEntryControl";
            this.Size = new System.Drawing.Size(442, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label assemblyNameLabel;
        private System.Windows.Forms.Label assemblyFullNameLabel;
        private System.Windows.Forms.LinkLabel linkLabel;
    }
}
