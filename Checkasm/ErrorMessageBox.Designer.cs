namespace CheckAsm
{
    partial class ErrorMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorMessageBox));
            this.label1 = new System.Windows.Forms.Label();
            this.mainMessageLabel = new System.Windows.Forms.Label();
            this.sendReportCheckbox = new System.Windows.Forms.CheckBox();
            this.detailsButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.commentTextbox = new System.Windows.Forms.TextBox();
            this.emailMeCheckbox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.emailTextbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "We\'re Sorry";
            // 
            // mainMessageLabel
            // 
            this.mainMessageLabel.Location = new System.Drawing.Point(15, 59);
            this.mainMessageLabel.Name = "mainMessageLabel";
            this.mainMessageLabel.Size = new System.Drawing.Size(332, 43);
            this.mainMessageLabel.TabIndex = 1;
            this.mainMessageLabel.Text = "CheckAsm had a problem and crashed. To help us diagnose and fix the problem, you " +
    "can send us a crash report.";
            // 
            // sendReportCheckbox
            // 
            this.sendReportCheckbox.AutoSize = true;
            this.sendReportCheckbox.Location = new System.Drawing.Point(18, 105);
            this.sendReportCheckbox.Name = "sendReportCheckbox";
            this.sendReportCheckbox.Size = new System.Drawing.Size(119, 17);
            this.sendReportCheckbox.TabIndex = 1;
            this.sendReportCheckbox.Text = "Send a crash report";
            this.sendReportCheckbox.UseVisualStyleBackColor = true;
            this.sendReportCheckbox.CheckedChanged += new System.EventHandler(this.sendReportCheckbox_CheckedChanged);
            // 
            // detailsButton
            // 
            this.detailsButton.Location = new System.Drawing.Point(3, 2);
            this.detailsButton.Name = "detailsButton";
            this.detailsButton.Size = new System.Drawing.Size(75, 23);
            this.detailsButton.TabIndex = 2;
            this.detailsButton.Text = "Details...";
            this.detailsButton.UseVisualStyleBackColor = true;
            this.detailsButton.Click += new System.EventHandler(this.detailsButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Comment";
            // 
            // commentTextbox
            // 
            this.commentTextbox.Location = new System.Drawing.Point(6, 56);
            this.commentTextbox.Multiline = true;
            this.commentTextbox.Name = "commentTextbox";
            this.commentTextbox.Size = new System.Drawing.Size(329, 93);
            this.commentTextbox.TabIndex = 3;
            // 
            // emailMeCheckbox
            // 
            this.emailMeCheckbox.AutoSize = true;
            this.emailMeCheckbox.Location = new System.Drawing.Point(6, 167);
            this.emailMeCheckbox.Name = "emailMeCheckbox";
            this.emailMeCheckbox.Size = new System.Drawing.Size(325, 17);
            this.emailMeCheckbox.TabIndex = 4;
            this.emailMeCheckbox.Text = "Tech support may contact me with request for more information.";
            this.emailMeCheckbox.UseVisualStyleBackColor = true;
            this.emailMeCheckbox.CheckedChanged += new System.EventHandler(this.emailMeCheckbox_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "E-mail:";
            // 
            // emailTextbox
            // 
            this.emailTextbox.Location = new System.Drawing.Point(47, 3);
            this.emailTextbox.Name = "emailTextbox";
            this.emailTextbox.Size = new System.Drawing.Size(285, 20);
            this.emailTextbox.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(284, 353);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Proceed";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(203, 353);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Ignore";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.detailsButton);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.commentTextbox);
            this.panel1.Controls.Add(this.emailMeCheckbox);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(15, 124);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(350, 224);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.emailTextbox);
            this.panel2.Enabled = false;
            this.panel2.Location = new System.Drawing.Point(18, 311);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(338, 29);
            this.panel2.TabIndex = 5;
            // 
            // ErrorMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 383);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.sendReportCheckbox);
            this.Controls.Add(this.mainMessageLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorMessageBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Error";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mainMessageLabel;
        private System.Windows.Forms.CheckBox sendReportCheckbox;
        private System.Windows.Forms.Button detailsButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox commentTextbox;
        private System.Windows.Forms.CheckBox emailMeCheckbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox emailTextbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}