using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CheckAsm.Properties;

namespace CheckAsm
{
    public partial class EulaForm : Form
    {
        public EulaForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            textBox1.Text = Resources.EULA;
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
        }
    }
}
