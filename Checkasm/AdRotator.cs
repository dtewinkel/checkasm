using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CheckAsm
{
    public partial class AdRotator : UserControl
    {
        private readonly List<Image> _images = new List<Image>();

        public List<Image> Images { get { return _images; } }

        private int _currentIndex = 0;

        private int _intervalMs = 1000;

        [EditorBrowsable]
        public int IntervalMs { get { return _intervalMs; } set { _intervalMs = value; } }

        [EditorBrowsable]
        public string Target { get; set; }

        public AdRotator()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_images.Count > 0)
            {
                pictureBox.Image = _images[_currentIndex];
            }
            timer1.Interval = _intervalMs;
            timer1.Start();
        }

        public void SetInitialImage(Image img)
        {
            pictureBox.Image = img;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_images.Count > 0)
            {
                pictureBox.Image = _images[_currentIndex];
                _currentIndex++;
                if (_currentIndex >= _images.Count)
                {
                    _currentIndex = 0;
                }
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Target) && Target.StartsWith("http://"))
            {
                try
                {
                    Process.Start(Target);
                }
                catch { }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://www.amberfish.net/paypal.aspx");
            }
            catch { }   
        }

    }
}
