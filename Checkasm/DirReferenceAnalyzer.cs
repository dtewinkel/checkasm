using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using CheckAsm.Descriptors.DirRefAnalyzer;

namespace CheckAsm
{
    [Serializable]
    class DirReferenceAnalyzer
    {
        public BackgroundWorker BgWorker { get; set; }

        internal DirReferenceScanResult Scan(DirectoryReferenceAnalyzerParameters parameters, ref bool cancelled)
        {
            Trace.WriteLine("DirReferenceAnalyzer reading directory " + parameters.Directory);

            Environment.CurrentDirectory = parameters.Directory;
           

            Dictionary<string, string> asmNameToFileMap;
            List<Assembly> loadedAssemblies;
            Dictionary<string, AsmData> fileToAsmDataMap;
            List<MissingAssemblyDescriptor> missingAssemblies = new List<MissingAssemblyDescriptor>();

            LoadAssemblies(parameters, out asmNameToFileMap, out loadedAssemblies, out fileToAsmDataMap);

            List<AsmData> allData = ReadReferences(asmNameToFileMap, loadedAssemblies, fileToAsmDataMap, missingAssemblies, parameters.GacAssemblies);
            
            StringBuilder graph = GenerateGraph(allData, missingAssemblies);
            var report = CreateReport(allData);
            
            Trace.WriteLine("Returning Graph:");
            Trace.WriteLine(graph.ToString());
            var result = new DirReferenceScanResult { GraphText = graph.ToString(), DirectoryReport = report, MissingAssemblies = missingAssemblies };
            return result;
        }

        private void LoadAssemblies(DirectoryReferenceAnalyzerParameters parameters, out Dictionary<string, string> asmNameToFileMap, out List<Assembly> loadedAssemblies, out Dictionary<string, AsmData> fileToAsmDataMap)
        {
            BgWorker.ReportProgress(0, "Reading directory");
            var files = Analyzer.GetAssemblies(parameters.Directory, false, delegate { return BgWorker.CancellationPending; },
                delegate(int progress, string text) { BgWorker.ReportProgress(progress, text); });

            Dictionary<string, string> fileToAsmNameMap = new Dictionary<string, string>();
            asmNameToFileMap = new Dictionary<string, string>();
            loadedAssemblies = new List<Assembly>();
            fileToAsmDataMap = new Dictionary<string, AsmData>();
            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.ReflectionOnlyLoadFrom(file);
                    var asmName = assembly.GetName();
                    var asmData = new AsmData(asmName.FullName, file);
                    fileToAsmNameMap.Add(file, asmName.FullName);
                    asmNameToFileMap.Add(asmName.FullName, file);
                    loadedAssemblies.Add(assembly);
                    fileToAsmDataMap.Add(file, asmData);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed to do ReflectionLoad on " + file);
                    Trace.WriteLine(ex);
                }
            }
        }

        private List<AsmData> ReadReferences(Dictionary<string, string> asmNameToFileMap, List<Assembly> loadedAssemblies, Dictionary<string, AsmData> fileToAsmDataMap, List<MissingAssemblyDescriptor> missingAssemblies, List<AsmData> gacAssemblies)
        {
            BgWorker.ReportProgress(30, "Reading references");
            List<AsmData> allData = new List<AsmData>();
            foreach (var assembly in loadedAssemblies)
            {
                string mainFile = asmNameToFileMap[assembly.GetName().FullName];
                AsmData mainData = fileToAsmDataMap[mainFile];
               
                foreach (var referencedName in assembly.GetReferencedAssemblies())
                {
                    if (asmNameToFileMap.ContainsKey(referencedName.FullName))
                    {
                        string file = asmNameToFileMap[referencedName.FullName];
                        AsmData referenced = fileToAsmDataMap[file];
                        mainData.References.Add(referenced);
                    }
                    else
                    {
                        bool found = false;
                        foreach(var gacAsm in gacAssemblies)
                        {
                            if (gacAsm.AssemblyFullName.Contains(referencedName.FullName))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            missingAssemblies.Add(
                            new MissingAssemblyDescriptor
                            {
                                Parent = mainData.Path,
                                Missing = referencedName.Name
                            });
                        }
                    }
                }
               
                allData.Add(mainData);
            }

            return allData;
        }

        private static Dictionary<string, DirReferenceReportItem> CreateReport(List<AsmData> allData)
        {
            var report = new Dictionary<string, DirReferenceReportItem>(); // first number in tuple is a number of references, the second is number showing how many assemblies reference this assembly

            foreach (var asm in allData)
            {
                var referencing = System.IO.Path.GetFileName(asm.Path);
                if (!report.ContainsKey(referencing))
                {
                    report.Add(referencing, new DirReferenceReportItem() { ShortName = referencing});
                }

                foreach (var reference in asm.References)
                {
                    var referenced = System.IO.Path.GetFileName(reference.Path);
                    if (!report.ContainsKey(referenced))
                    {
                        report.Add(referenced, new DirReferenceReportItem() { ShortName = referenced});
                    }

                    report[referencing].ReferencesCount++;
                    report[referenced].ReferencingCount++;
                }
            }

            return report;
        }

        private StringBuilder GenerateGraph(List<AsmData> allData, List<MissingAssemblyDescriptor> missingAssemblies)
        {
            BgWorker.ReportProgress(60, "Generating graph");
            StringBuilder graph = new StringBuilder();
            graph.AppendLine("digraph G {\r\nnode[shape=box];");
            foreach (var asm in allData)
            {
                foreach (var reference in asm.References)
                {
                    graph.AppendLine(string.Format("\"{0}\" -> \"{1}\";", GetShortName(asm.Path), GetShortName(reference.Path)));
                }
            }
            foreach(var missing in missingAssemblies)
            {
                graph.AppendLine(string.Format("\"{0}\" -> \"{1}\";", GetShortName(missing.Parent), GetShortName(missing.Missing)));
            }
            graph.AppendLine("}");
            return graph;
        }

        private static string GetShortName(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                return "<unknown>";
            }
            return System.IO.Path.GetFileName(assemblyName);
        }
    }
}
