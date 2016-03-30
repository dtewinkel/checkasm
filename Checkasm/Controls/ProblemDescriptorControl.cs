using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CheckAsm.Descriptors;

namespace CheckAsm.Controls
{
    public partial class ProblemDescriptorControl : UserControl
    {
        private ProblemDescriptor _selectedObject;

        public event EventHandler NavigatePressed;

        public ProblemDescriptor SelectedObject
        {
            get { return _selectedObject; }
            set { 
                _selectedObject = value;
                UpdateData();
            }
        }

        protected virtual void OnNavigatePressed()
        {
            var handler = NavigatePressed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void UpdateData()
        {
            if (_selectedObject == null)
            {
                lblTitle.Text = "No problem is selected. Select an item from the grid on the left.";
                lblSource.Text = "";
                lblSrcAssemblyTitle.Text = "";
                lblAdditionalDetailsTitle.Text = "";
                txtDetails.Text = "";
                linkLabel1.Visible = false;
                txtDetails.Visible = false;
            }
            else
            {
                lblTitle.Text = _selectedObject.Title;
                lblSource.Text = _selectedObject.Source != null ? _selectedObject.Source.AssemblyFullName : "unknown";
                lblSrcAssemblyTitle.Text = "Source Assembly:";
                lblAdditionalDetailsTitle.Text = "Additional Details:";
                txtDetails.Text = _selectedObject.Detail;
                linkLabel1.Visible = _selectedObject.Source != null;
                txtDetails.Visible = true;
            }
        }

        public ProblemDescriptorControl()
        {
            InitializeComponent();
            UpdateData();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnNavigatePressed();
        }
    }
}
