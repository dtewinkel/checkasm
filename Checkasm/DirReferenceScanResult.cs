using CheckAsm.Descriptors.DirRefAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm
{
    [Serializable]
    public class DirReferenceScanResult
    {
        public string GraphText { get; set; }
        public Dictionary<string, DirReferenceReportItem> DirectoryReport { get; set; }
        public List<MissingAssemblyDescriptor> MissingAssemblies { get; set; }
    }

    [Serializable]
    public class DirReferenceReportItem
    {
        /// <summary>
        /// Short assembly name
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Number of references of this assembly
        /// </summary>
        public int ReferencesCount { get; set; }

        /// <summary>
        /// Number of assemblies referencing the asssembly
        /// </summary>
        public int ReferencingCount { get; set; }

        public override string ToString()
        {
            return ShortName + " (" + ReferencesCount + "," + ReferencingCount +")";
        }
    }
}
