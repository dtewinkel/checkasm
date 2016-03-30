using CheckAsm.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckAsm.Licensing
{
    public partial class ComparisonMatrix : Form
    {
        public string ResourceName { get; set; }

        private string tempFile;

        public ComparisonMatrix()
        {
            InitializeComponent();
            tempFile = Path.GetTempFileName();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadResource();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            try 
            {
                File.Delete(tempFile); 
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private void LoadResource()
        {
            using (var writer = new StreamWriter(tempFile))
            {
                var content = Resources.ResourceManager.GetString(ResourceName);
                writer.Write(content);
            }
            webBrowser.Url = new Uri(tempFile,UriKind.RelativeOrAbsolute);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
