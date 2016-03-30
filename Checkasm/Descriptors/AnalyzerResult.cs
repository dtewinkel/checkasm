using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm.Descriptors
{
    [Serializable]
    public class AnalyzerResult
    {
        private readonly List<ProblemDescriptor> _problems = new List<ProblemDescriptor>();
        public AsmData RootAssembly { get; set; }
        public List<ProblemDescriptor> Problems { get { return _problems; } }
    }
}
