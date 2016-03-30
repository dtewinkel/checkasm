using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm
{
    [Serializable]
    public class DirectoryReferenceAnalyzerParameters
    {
        public string Directory { get; set; }
        public List<AsmData> GacAssemblies { get; set; }
    }
}
