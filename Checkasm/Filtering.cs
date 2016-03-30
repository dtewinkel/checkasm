using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CheckAsm
{
    public partial class Filtering : Form
    {
        private List<string> filters;

        public List<string> Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        public Filtering()
        {
            InitializeComponent();
        }

        private void filterListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                deleteButton_Click(sender, EventArgs.Empty);
            }
        }

        private void Filter_Load(object sender, EventArgs e)
        {
            if (filters != null)
            {
                this.filterListBox.Items.AddRange(filters.ToArray());
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            filters.Clear();
            foreach (string item in filterListBox.Items)
            {
                filters.Add(item);
            }
            Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            filterListBox.Items.Add(textBox.Text);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            filterListBox.Items.Clear();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (filterListBox.SelectedItem != null)
            {
                filterListBox.Items.Remove(filterListBox.SelectedItem);
            }
        }

        
    }
}
