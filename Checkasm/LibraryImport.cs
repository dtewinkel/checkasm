using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace CheckAsm
{
    [Serializable]
    public class LibraryImport
    {
        string fileName;
        string version;

        
        string fullPath;
        bool exists;

        public LibraryImport(string fileName, string fullPath, bool exists)
        {
            this.fileName = fileName;
            this.fullPath = fullPath;
            this.exists = exists;
            if(File.Exists(fullPath))
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(fullPath);
                version = info.FileVersion;
            }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public LibraryImport(string fullPath, bool exists) : this(Path.GetFileName(fullPath), fullPath, exists){}

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string FullPath
        {
            get { return fullPath; }
            set 
            { 
                fullPath = value;
                if (File.Exists(fullPath))
                {
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(fullPath);
                    version = info.FileVersion;
                }
            }
        }

        public bool Exists
        {
            get { return exists; }
            set { exists = value; }
        }

        public string GetLongDescription()
        {
            return exists ? fullPath + " (version " + version+ ")": (fileName + " [missing]");
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
