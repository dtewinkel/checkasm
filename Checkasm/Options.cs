using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CheckAsm
{
    [Serializable]
    class Options
    {
        private Point appStartLocation;
        private List<string> recentFiles = new List<string>();
        private Size appSize;
        private int splitterPosition = 0;
        private int assembliesInGacCount = 500;
        private List<string> searchPhrases;
        private List<string> assemblyFilter = new List<string>();
        private Guid customerId;
        private FormWindowState appWindowState;

        public DirectoryScannerOptions ScannerOptions { get; private set; }
        public DirectoryReferenceAnalyzerParameters DirReferenceAnalyzerParameters { get; private set; }
        private int splitter2Position = 0;

        public List<string> SearchPhrases
        {
            get { return searchPhrases; }
            set { searchPhrases = value; }
        }

        public int AssembliesInGacCount
        {
            get { return assembliesInGacCount; }
            set { assembliesInGacCount = value; }
        }

        public int SplitterPosition
        {
            get { return splitterPosition; }
            set { splitterPosition = value; }
        }

        public int Splitter2Position
        {
            get { return splitter2Position; }
            set { splitter2Position = value; }
        }

        public Size AppSize
        {
            get { return appSize; }
            set { appSize = value; }
        }

        public Point AppStartLocation
        {
            get { return appStartLocation; }
            set { appStartLocation = value; }
        }

        public List<string> RecentFiles
        {
            get { return recentFiles; }
            set { recentFiles = value; }
        }

        public List<string> AssemblyFilter
        {
            get { return assemblyFilter; }
            set { assemblyFilter = value; }
        }

        public Guid CustomerId
        {
            get
            {
                return this.customerId;
            }
            set
            {
                this.customerId = value;
            }
        }

        public FormWindowState AppWindowState
        {
            get { return appWindowState; }
            set { appWindowState = value; }
            
        }

        public Options()
        {
            ScannerOptions = new DirectoryScannerOptions();
            DirReferenceAnalyzerParameters = new DirectoryReferenceAnalyzerParameters();
        }

        public void AddRecentFile(string file)
        {
            if(recentFiles.IndexOf(file) == -1)
            {
                recentFiles.Add(file);
                if (recentFiles.Count > 10)
                    recentFiles.RemoveAt(0);
            }
        }

        public void Save(string file)
        {
            FileStream fs = new FileStream(file, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, this);
            fs.Close();
        }

        public void Load(string file)
        {
            if(File.Exists(file))
            {
                try
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        Options opt = (Options)formatter.Deserialize(fs);
                       
                        this.recentFiles = opt.recentFiles;
                        this.appStartLocation = opt.AppStartLocation;
                        this.appSize = opt.AppSize;
                        this.splitterPosition = opt.SplitterPosition;
                        this.splitter2Position = opt.Splitter2Position;
                        this.assembliesInGacCount = opt.AssembliesInGacCount;
                        this.searchPhrases = opt.SearchPhrases;
                        this.CustomerId = opt.CustomerId;
                        this.ScannerOptions = opt.ScannerOptions;
                        this.appWindowState = opt.AppWindowState;

                        if (opt.AssemblyFilter != null) 
                            this.assemblyFilter = opt.AssemblyFilter;

                        if (assembliesInGacCount <= 0) 
                            assembliesInGacCount = 500;

                        if (opt.DirReferenceAnalyzerParameters != null)
                            this.DirReferenceAnalyzerParameters = opt.DirReferenceAnalyzerParameters;

                        fs.Close();
                    }
                }
                catch(Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    Trace.WriteLine("Exception: " + ex.Message);
                }
            }
        }
    }
}
