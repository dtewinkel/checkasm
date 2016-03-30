using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CheckAsm
{
    public partial class SearchForm : Form
    {

        private Search search;
        bool resultsAvailable;
        List<TreeNode> nodes;
        internal TreeNode RootNode { get; set; }
        int currentIndex = 0;

        private SearchForm()
        {
            InitializeComponent();
        }

        public SearchForm(Search search, TreeNode rootNode)
        {
            InitializeComponent();
            this.search = search;
            this.RootNode = rootNode;
            findTextComboBox.Items.AddRange(search.Phrases.ToArray());
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            if (findTextComboBox.Text == "")
            {
                MessageBox.Show("Cannot search for empty string.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!findTextComboBox.Items.Contains(findTextComboBox.Text))
            {
                findTextComboBox.Items.Add(findTextComboBox.Text);
            }
            if (findTextComboBox.Items.Count > 20)
            {
                findTextComboBox.Items.RemoveAt(0);
            }
            if (!resultsAvailable)
            {
                nodes = search.Find(findTextComboBox.Text, RootNode);
                if (nodes.Count > 0)
                {
                    resultsAvailable = true;
                    currentIndex = 0;
                    nodes[0].TreeView.SelectedNode = nodes[0];
                    Owner.BringToFront();
                }
                else
                {
                    MessageBox.Show("The specified text was not found.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (currentIndex >= nodes.Count -1)
                    currentIndex = 0;
                else
                    currentIndex++;

                nodes[currentIndex].TreeView.SelectedNode = nodes[currentIndex];
                Owner.BringToFront();
            }
        }

        private void findTextComboBox_TextChanged(object sender, EventArgs e)
        {
            resultsAvailable = false;
        }

        private void findTextComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            resultsAvailable = false;
        }
    }
}