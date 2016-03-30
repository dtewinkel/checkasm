using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace CheckAsm
{
    partial class AssemblyEntryControl : UserControl
    {
        public event EventHandler<AssemblyEntryEventArgs> LinkClicked;

        private AssemblyEntry data;

        public AssemblyEntry Data
        {
            get { return data; }
            set
            {
                data = value;
                assemblyNameLabel.Text = Path.GetFileName(data.FileName);
                try
                {
                    assemblyFullNameLabel.Text = AssemblyName.GetAssemblyName(data.FileName).FullName;
                }
                catch
                {
                    assemblyFullNameLabel.Text = "An error occurred while trying to load this assembly.";
                    data.Success = false;
                }
                if (!data.Success)
                {
                    assemblyNameLabel.ForeColor = Color.Red;
                }
                else
                {
                    assemblyNameLabel.ForeColor = Color.Green;
                }
            }
        }

        public AssemblyEntryControl()
        {
            InitializeComponent();
        }

        public AssemblyEntryControl(AssemblyEntry data)
        {
            InitializeComponent();
            Data = data;
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (LinkClicked != null)
            {
                LinkClicked(this, new AssemblyEntryEventArgs(data));
            }
        }

        internal class AssemblyEntryEventArgs : EventArgs
        {
            AssemblyEntry entry;
            public AssemblyEntry Entry
            {
                get { return entry; }
            }

            public AssemblyEntryEventArgs(AssemblyEntry entry)
            {
                this.entry = entry;
            }
        }
    }
}
