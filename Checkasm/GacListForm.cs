using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using CheckAsm.Dialogs;

namespace CheckAsm
{
    class GacListForm:ListFormBase
    {
        private ListView listView;
        private ColumnHeader nameColumnHeader;
        private ColumnHeader versionColumnHeader;
        private ColumnHeader pathColumnHeader;
        private TextBox txtSearch;

        private Sorting curentSorting;

        const string SearchText = "Search...";
        private Timer timer;
        private System.ComponentModel.IContainer components;

        public GacListForm()
        {
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = "Assemblies in Global Assembly Cache";
        }
        
    }
}
