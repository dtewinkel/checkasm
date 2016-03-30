using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace CheckAsm
{
    public partial class ErrorDetailsForm : Form
    {
        public ErrorDetailsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Contents of the textbox
        /// </summary>
        public string Details
        {
            get
            {
                return detailsTextbox.Text;
            }
            set
            {
                detailsTextbox.Text = value;
            }
        }

        private void logFileLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(Program.LogFilePath));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot open folder " + Program.LogFilePath + ". " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
