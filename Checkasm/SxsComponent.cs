using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CheckAsm
{
    class SxsComponent
    {
        string name;
        string version;
        string processorArchitecture;

        public SxsComponent(string name, string version, string processorArchitecture)
        {
            this.name = name;
            this.version = version;
            this.processorArchitecture = processorArchitecture;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public string ProcessorArchitecture
        {
            get { return processorArchitecture; }
            set { processorArchitecture = value; }
        }

        /// <summary>
        /// gets path to the component directory
        /// </summary>
        /// <returns></returns>
        public string GetFullPath()
        {
            string sxs = Path.GetFullPath(Environment.GetFolderPath(System.Environment.SpecialFolder.System) + "\\..\\WinSxs");
            string[] dirs = Directory.GetDirectories(sxs);
            foreach (string dir in dirs)
            {
                if (dir.Contains(name) && dir.Contains(version) && dir.Contains(processorArchitecture))
                {
                    return dir;
                }
            }
            return "";
        }

        public override string ToString()
        {
            return name + " " + version + "(" + processorArchitecture + ")";
        }
    }
}
