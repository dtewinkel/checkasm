using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace CheckAsm.Dialogs
{
    internal class ListFormBase:Form
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
        private ImageList imageList1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem copySelectedToolStripMenuItem;

        protected List<AsmData> dataSource;

        public List<AsmData> DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        public ListFormBase()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListFormBase));
            this.listView = new System.Windows.Forms.ListView();
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.versionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pathColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.versionColumnHeader,
            this.pathColumnHeader});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(631, 494);
            this.listView.SmallImageList = this.imageList1;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "Name";
            this.nameColumnHeader.Width = 108;
            // 
            // versionColumnHeader
            // 
            this.versionColumnHeader.Text = "Version";
            this.versionColumnHeader.Width = 96;
            // 
            // pathColumnHeader
            // 
            this.pathColumnHeader.Text = "Path";
            this.pathColumnHeader.Width = 421;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "BlueSheet.ico");
            this.imageList1.Images.SetKeyName(1, "RedSheet.ico");
            this.imageList1.Images.SetKeyName(2, "OrangeSheet.ico");
            this.imageList1.Images.SetKeyName(3, "circ.ico");
            this.imageList1.Images.SetKeyName(4, "Redirection.ico");
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtSearch.ForeColor = System.Drawing.Color.Gray;
            this.txtSearch.Location = new System.Drawing.Point(475, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(152, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.Text = "Search...";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.Leave += new System.EventHandler(this.txtSearch_Leave);
            this.txtSearch.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtSearch_MouseDown);
            // 
            // timer
            // 
            this.timer.Interval = 300;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(631, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySelectedToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.copyToolStripMenuItem.Text = "Edit";
            // 
            // copySelectedToolStripMenuItem
            // 
            this.copySelectedToolStripMenuItem.Name = "copySelectedToolStripMenuItem";
            this.copySelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copySelectedToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.copySelectedToolStripMenuItem.Text = "Copy Selected";
            this.copySelectedToolStripMenuItem.Click += new System.EventHandler(this.copySelectedToolStripMenuItem_Click);
            // 
            // ListFormBase
            // 
            this.ClientSize = new System.Drawing.Size(631, 494);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ListFormBase";
            this.Text = "Assemblies in Global Assembly Cache";
            this.Load += new System.EventHandler(this.ListFormBase_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ListFormBase_Load(object sender, EventArgs e)
        {
            curentSorting = Sorting.Name;
            this.listView.Items.AddRange(GetSortedItems(dataSource, curentSorting).ToArray());
        }

        protected virtual List<ListViewItem> GetSortedItems(List<AsmData> assemblies, Sorting sorting, string filter)
        {
            List<AsmData> asmData = new List<AsmData>(assemblies);
            AsmDataComparer comparer = new AsmDataComparer();
            comparer.SortingType = sorting;
            asmData.Sort(comparer);
            var items = from item in asmData
                        where (item.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) != -1 || item.Path.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) != -1)
                        select new ListViewItem(new[] { item.AssemblyFullName.Substring(0, item.AssemblyFullName.IndexOf(", Version=")),
                            item.AssemblyFullName.Substring(item.AssemblyFullName.IndexOf(", Version=") + 10, item.AssemblyFullName.IndexOf(", Culture=") - item.AssemblyFullName.IndexOf(", Version=") - 10),
                            item.Path }, (int)item.Validity) { Tag = item };
            return items.ToList();
        }

        private List<ListViewItem> GetSortedItems(List<AsmData> assemblies, Sorting sorting)
        {
            return GetSortedItems(assemblies, sorting, "");
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listView.Items.Clear();
            Sorting sorting = curentSorting;
            switch (e.Column)
            {
                case 1:
                    sorting = Sorting.Version;
                    break;
                case 2:
                    sorting = Sorting.Path;
                    break;
                default:
                    break;
            }
            curentSorting = sorting;
            listView.Items.AddRange(GetSortedItems(dataSource, sorting).ToArray());
        }

        private void txtSearch_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtSearch.Text == SearchText)
            {
                txtSearch.Text = "";
                txtSearch.Font = new System.Drawing.Font(txtSearch.Font, System.Drawing.FontStyle.Regular);
                txtSearch.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                txtSearch.Text = SearchText;
                txtSearch.Font = new System.Drawing.Font(txtSearch.Font, System.Drawing.FontStyle.Italic);
                txtSearch.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text) && txtSearch.Text != SearchText)
            {
                timer.Stop();
                timer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            listView.Items.Clear();
            listView.Items.AddRange(GetSortedItems(dataSource, curentSorting, txtSearch.Text).ToArray());

        }

        internal enum Sorting
        {
            Name,
            Version,
            Path
        }

        internal class AsmDataComparer : IComparer<AsmData>
        {
            private Sorting sortingType;

            public Sorting SortingType
            {
                get { return sortingType; }
                set { sortingType = value; }
            }

            #region IComparer<AsmData> Members

            public int Compare(AsmData x, AsmData y)
            {
                switch (sortingType)
                {
                    case Sorting.Name:
                        string nameX = x.AssemblyFullName.Substring(0, x.AssemblyFullName.IndexOf(", Version="));
                        return nameX.CompareTo(y.AssemblyFullName.Substring(0, y.AssemblyFullName.IndexOf(", Version=")));
                    case Sorting.Path:
                        return x.Path.CompareTo(y.Path);
                    case Sorting.Version:
                        string versionX = x.AssemblyFullName.Substring(x.AssemblyFullName.IndexOf(", Version=") + 10, x.AssemblyFullName.IndexOf(", Culture=") - x.AssemblyFullName.IndexOf(", Version=") - 10);
                        string versionY = y.AssemblyFullName.Substring(y.AssemblyFullName.IndexOf(", Version=") + 10, y.AssemblyFullName.IndexOf(", Culture=") - y.AssemblyFullName.IndexOf(", Version=") - 10);
                        return versionX.CompareTo(versionY);
                    default:
                        break;
                }
                throw new NotImplementedException("method is not implemented for SortingType " + sortingType);
            }

            #endregion
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var csv = new StringBuilder();
            var tabbed = new StringBuilder();
            foreach(ListViewItem i in listView.SelectedItems)
            {
                AsmData dataItem = (AsmData)i.Tag;
                csv.AppendLine(string.Format("{0},{1},{2},{3}", AssemblyStatusTextProvider.GetText(dataItem.Validity), i.SubItems[0].Text, i.SubItems[1].Text, i.SubItems[2].Text));
                tabbed.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}", AssemblyStatusTextProvider.GetText(dataItem.Validity), i.SubItems[0].Text, i.SubItems[1].Text, i.SubItems[2].Text));
            }
            // Create the container object that will hold both versions of the data.
            var dataObject = new DataObject();

            // Add tab-delimited text to the container object as is.
            dataObject.SetText(tabbed.ToString());

            // Convert the CSV text to a UTF-8 byte stream before adding it to the container object.
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var stream = new System.IO.MemoryStream(bytes);
            dataObject.SetData(DataFormats.CommaSeparatedValue, stream);

            // Copy the container object to the clipboard.
           Clipboard.SetDataObject(dataObject, true);
            
        }

    }

    
}
