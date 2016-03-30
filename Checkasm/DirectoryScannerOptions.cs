using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm
{
    [Serializable]
    class DirectoryScannerOptions
    {
        /// <summary>
        /// specifies the last used directory
        /// </summary>
        public string LastDirectory { get; set; }

        /// <summary>
        /// Specifies whether to scan the subdirectories
        /// </summary>
        public bool Recursive { get; set; }

        public override string ToString()
        {
            return Recursive + " " + LastDirectory;
        }
    }
}
