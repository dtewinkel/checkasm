using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using CheckAsm.Properties;

namespace CheckAsm
{
    public partial class SocialBar : UserControl
    {
        public SocialBar()
        {
            InitializeComponent();
        }

        private void btnTwitter_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://twitter.com/amberfishnet");
            }
            catch
            {
            }
        }

        private void btnMail_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.amberfish.net/Register.aspx");
            }
            catch
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Settings.Default.DisplaySocialBar = false;
        }
    }
}
