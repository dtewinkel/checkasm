using CheckAsm.Descriptors;
using System;
namespace CheckAsm
{
    public interface IAnalyzer
    {
        AnalyzerResult AnalyzeRootAssembly(string assemblyName);
        AnalyzerResult AnalyzeRootAssembly(string assemblyName, bool throwWhenMissing);
        System.ComponentModel.BackgroundWorker BgWorker { get; set; }
        System.Collections.Generic.List<AsmData> Gac { get; set; }
        bool IsValidAssembly(string path, out string error);
    }


}
