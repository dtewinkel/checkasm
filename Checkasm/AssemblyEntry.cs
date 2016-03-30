using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;

namespace CheckAsm
{
    [Serializable]
    class AssemblyEntry
    {
        private string fileName;
        private bool success;

        public AssemblyEntry(string fileName)
        {
            this.fileName = fileName;
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public AsmData AsmData { get; set; }

        public bool Success
        {
            get { return success; }
            set { success = value; }
        }
    }
}
