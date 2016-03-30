using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm.Descriptors.DirRefAnalyzer
{
    [Serializable]
    public class MissingAssemblyDescriptor
    {
        public string Parent { get; set; }
        public string Missing { get; set; }

        public override string ToString()
        {
            return Missing;
        }
    }
}
