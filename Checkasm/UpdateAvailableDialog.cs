using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckAsm
{
    public partial class UpdateAvailableDialog : Form
    {
        public UpdateInfo UpdateTextInfo { get; set; }
        
        public UpdateAvailableDialog()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            lblChanges.Text = UpdateTextInfo.Description;
            lblVersion.Text = UpdateTextInfo.LatestVersion.ToString();
        }
    }
}
