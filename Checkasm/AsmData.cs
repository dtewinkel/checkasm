using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;

namespace CheckAsm
{
    [Serializable]
    [XmlType("Assembly")]
    public class AsmData
    {
        private LibraryImport[] imports;
        private string name;
        private string path;
        private List<AsmData> references;
        private AsmValidity validity;
        private string assemblyFullName;

        /// <summary>
        /// Gets or sets additional details displayed when Validity is not valid
        /// </summary>
        [XmlAttribute("ErrorDetailsText")]
        public string InvalidAssemblyDetails { get; set; }

        /// <summary>
        /// Additional information to be appended to the end of the data description
        /// </summary>
        [XmlAttribute("AdditionalInfo")]
        public string AdditionalInfo { get; set; }

        [XmlAttribute("Architecture")]
        public ProcessorArchitecture Architecture { get; set; }

        /// <summary>
        /// this is for XML Serialization only
        /// </summary>
        public AsmData()
        {
        }

        public AsmData(string name, string path)
        {
            this.name = name;
            this.path = path;
            this.imports = new LibraryImport[] { };
            references = new List<AsmData>();
            validity = AsmValidity.Invalid;
            Architecture = ProcessorArchitecture.None;
        }

        /// <summary>
        /// Gets or sets assembly's product name
        /// </summary>
        [XmlAttribute("ProductName")]
        public string AssemblyProductName { get; set; }

        [XmlAttribute("OriginalVersion")]
        public string OriginalVersion { get; set; }

        [XmlAttribute("RuntimeVersion")]
        public string RuntimeVersion { get; set; }

        public string RuntimeVersionShort { 
            get
            {
                var str = RuntimeVersion;
                if(String.IsNullOrEmpty(str))
                {
                    str = "   ";
                }
                else
                {
                    str = str.Replace("v", "").Trim();
                    Version v = new Version(str);
                    str = v.Major + "." + v.Minor;
                }
                return str;
            } 
        }

        /// <summary>
        /// Full name of the assembly
        /// </summary>
        [XmlAttribute("FullName")]
        public string AssemblyFullName
        {
            get { return assemblyFullName; }
            set { assemblyFullName = value; }
        }


        /// <summary>
        /// DLL imports
        /// </summary>
        [XmlIgnore]
        public LibraryImport[] Imports
        {
            get { return imports; }
            set { imports = value; }
        }

        /// <summary>
        /// Assembly name
        /// </summary>
        [XmlAttribute("Name")]
	    public string Name
	    {
		    get { return name; }
		    set { name = value; }
	    }

        /// <summary>
        /// Path to the DLL file
        /// </summary>
        [XmlAttribute("Path")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Assembly references
        /// </summary>
        public List<AsmData> References
        {
            get { return references; }
        }

        /// <summary>
        /// Assembly validity
        /// </summary>
        [XmlAttribute("IsValid")]
        public AsmValidity Validity
        {
            get { return validity; }
            set { validity = value; }
        }

        /// <summary>
        /// Specifies status of assembly validity
        /// </summary>
        public enum AsmValidity
        {
            /// <summary>
            /// Assembly is available and can be loaded
            /// </summary>
            Valid,
            /// <summary>
            /// Assembly is available but can not be loaded.
            /// </summary>
            ReferencesOnly,
            /// <summary>
            /// Assembly is missing
            /// </summary>
            Invalid,
            /// <summary>
            /// Assembly is valid but it is referenced by itself
            /// </summary>
            CircularDependency,
            /// <summary>
            /// Assembly is valid, assembly redirection is applied by publisher policy or application configuration file
            /// </summary>
            Redirected
        }

        /// <summary>
        /// Converts current object to its human readable representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringValue = new StringBuilder();
            stringValue.AppendLine(AssemblyFullName);
            stringValue.AppendLine(path);
            if (!string.IsNullOrEmpty(OriginalVersion))
            {
                stringValue.Append("Original referenced assembly version: ");
                stringValue.AppendLine(OriginalVersion);
            }
            
            if (imports.Length > 0)
            {
                stringValue.AppendLine();
                stringValue.AppendLine("Imports: ");
                foreach (LibraryImport imp in imports)
                {
                    stringValue.AppendLine(imp.GetLongDescription());
                }
            }
            if (!string.IsNullOrEmpty(InvalidAssemblyDetails) && Validity != AsmValidity.Valid)
            {
                stringValue.AppendLine("\r\n" + InvalidAssemblyDetails);
            }
            if (!string.IsNullOrEmpty(AdditionalInfo))
            {
                stringValue.AppendLine("\r\n" + AdditionalInfo);
            }
            stringValue.AppendLine("Architecture: " + Architecture);
            if (!string.IsNullOrEmpty(RuntimeVersion))
            {
                stringValue.AppendLine("Runtime version: " + RuntimeVersion);
            }

            return stringValue.ToString();
        }

       
    }
}
