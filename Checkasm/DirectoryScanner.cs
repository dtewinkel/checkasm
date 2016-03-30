using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace CheckAsm
{
    
    class DirectoryScanner:MarshalByRefObject
    {

        BackgroundWorker bgWorker;
        List<AsmData> gacAssemblies;

        public List<AsmData> GacAssemblies
        {
            get { return gacAssemblies; }
            set { gacAssemblies = value; }
        }

        public Dictionary<string, List<Redirection>> Redirections { get; set; }

        public BackgroundWorker BgWorker
        {
            get { return bgWorker; }
            set { bgWorker = value; }
        }

        public AssemblyEntry[] Scan(DirectoryScannerOptions parameters, ref bool cancelled)
        {
            Environment.CurrentDirectory = parameters.LastDirectory;
            List<AssemblyEntry> data = new List<AssemblyEntry>();

            bgWorker.ReportProgress(0, "Reading directory");
            var assemblies = Analyzer.GetAssemblies(parameters.LastDirectory, parameters.Recursive, delegate { return bgWorker.CancellationPending; },
                delegate(int progress, string text) { bgWorker.ReportProgress(progress, text); });
            double count = 0;
            foreach (string assembly in assemblies)
            {
                if (bgWorker.CancellationPending)
                {
                    cancelled = true;
                    break;
                }
                bgWorker.ReportProgress((int)(100 * count / (double)assemblies.Count), "Loading " + assembly);
                AssemblyEntry entry = new AssemblyEntry(assembly);
                try
                {
                    Analyzer analyzer = new Analyzer();
                    analyzer.Gac = gacAssemblies;
                    //analyzer.Redirections = Redirections;
                    string currentDir = Path.GetDirectoryName(assembly);
                    if (!string.IsNullOrEmpty(currentDir) && Directory.Exists(currentDir))
                    {
                        Environment.CurrentDirectory = currentDir;
                    }
                    var result = analyzer.AnalyzeRootAssembly(assembly, true);
                    entry.AsmData = result.RootAssembly;
                    entry.Success = result.RootAssembly.Validity == AsmData.AsmValidity.Valid ||
                                    result.RootAssembly.Validity == AsmData.AsmValidity.Redirected;
                }
                catch
                {
                    Debug.WriteLine("Failed to load " + assembly);
                    entry.Success = false;
                }
                count++;
                data.Add(entry);
            }
            return data.ToArray();
        }
    }

}
