using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm
{
    [Serializable]
    public class PathRedirection
    {
        private readonly List<string> directories = new List<string>();
        public List<string> Directories { get { return directories; } }
        public string AssemblyName { get; set; }
    }



}
