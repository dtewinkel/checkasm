namespace CheckAsm
{
    partial class SearchForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.findTextComboBox = new System.Windows.Forms.ComboBox();
            this.findNextButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(291, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter a text to find (part of assembly name, case insensitive):";
            // 
            // findTextComboBox
            // 
            this.findTextComboBox.FormattingEnabled = true;
            this.findTextComboBox.Location = new System.Drawing.Point(15, 25);
            this.findTextComboBox.MaxDropDownItems = 15;
            this.findTextComboBox.Name = "findTextComboBox";
            this.findTextComboBox.Size = new System.Drawing.Size(313, 21);
            this.findTextComboBox.TabIndex = 1;
            this.findTextComboBox.SelectedIndexChanged += new System.EventHandler(this.findTextComboBox_SelectedIndexChanged);
            this.findTextComboBox.TextChanged += new System.EventHandler(this.findTextComboBox_TextChanged);
            // 
            // findNextButton
            // 
            this.findNextButton.Location = new System.Drawing.Point(334, 23);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(75, 23);
            this.findNextButton.TabIndex = 2;
            this.findNextButton.Text = "Find Next";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
            // 
            // SearchForm
            // 
            this.AcceptButton = this.findNextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 61);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.findTextComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SearchForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search for assembly...";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox findTextComboBox;
        private System.Windows.Forms.Button findNextButton;
    }
}