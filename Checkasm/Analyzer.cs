using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using CheckAsm.Properties;
using CheckAsm.Descriptors;

namespace CheckAsm
{
    public class Analyzer:MarshalByRefObject, ISponsor, CheckAsm.IAnalyzer
    {
        string workingDir;
        bool circularDependencyWarningShown = false;
        int totalReferencedAssemblies;
        BackgroundWorker bgWorker;
        int assembliesFinished;
        ProcessorArchitecture currentLoadedArchitecture;

        private readonly Dictionary<string, string> assemblyNameToPathMap = new Dictionary<string, string>();

        static bool[,] architectureCompatibilityMatrix = { { false, false, false, false, false }, { false, true, true, true, true }, { false, true, true, false, false }, { false, true, false, true, false }, { false, true, false, false, true} }; //[parent, child]

        /// <summary>
        /// contains info on probing directories. Key is assembly name, value is path
        /// </summary>
        private readonly Dictionary<string, PathRedirection> probingPaths = new Dictionary<string, PathRedirection>();
        //public Dictionary<string, List<Redirection>> Redirections { get; set; }

        /// <summary>
        /// Assembly redirections full path-redirections
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, List<Redirection>>> allVersionRedirections = new Dictionary<string, Dictionary<string, List<Redirection>>>();
        
        Stack<string> parentsStack;
        List<AsmData> gac;
        Dictionary<string, AsmData> cache;

        TextWriterTraceListener listener = new TextWriterTraceListener(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\checkasm.log", "CheckAsm logging");

        /// <summary>
        /// List of assemblies in GAC
        /// </summary>
        public List<AsmData> Gac
        {
            get { return gac; }
            set { gac = value; }
        }

        /// <summary>
        /// BackgroundWorker processing request
        /// </summary>
        public BackgroundWorker BgWorker
        {
            set { bgWorker = value; }
            get { return bgWorker; }
        }

        /// <summary>
        /// Checks whether file is valid assembly
        /// </summary>
        /// <param name="path">path to the file</param>
        /// <returns>If file is valid assembly, returns empty string; otherwise returns error message.</returns>
        public bool IsValidAssembly(string path, out string error)
        {
            error = string.Empty;
            try
            {
                Assembly asm = Assembly.ReflectionOnlyLoadFrom(path);
                totalReferencedAssemblies = asm.GetReferencedAssemblies().Length;
            }
            catch (FileLoadException ex)
            {
                error = "This file could not be loaded. \r\nException details: " + ex.FusionLog;
            }
            catch (BadImageFormatException ex)
            {
                error = "This file is not a valid assembly or this assembly is built with later version of CLR\r\nPlease update your CLR to the latest version. \r\nException details: " + ex.FusionLog;
            }
            catch (SecurityException ex)
            {
                error = "A security problem has occurred while loading the assembly.\r\nFailed permission: " + ex.FirstPermissionThatFailed;
            }
            catch (PathTooLongException)
            {
                error = "Given path is too long.";
            }
            return string.IsNullOrEmpty(error);
        }

        public static List<string> GetAssemblies(string directory, bool recursive, Func<bool> cancellationPendingCallback, Action<int, string> progressReportCallback)
        {
            if (progressReportCallback != null)
            {
                progressReportCallback(0, "Reading directory " + directory);
            }
            string[] files = Directory.GetFiles(directory);
            List<string> assemblies = new List<string>();
            foreach (string file in files)
            {
                if (cancellationPendingCallback != null && cancellationPendingCallback())
                {
                    return assemblies;
                }
                try
                {
                    if (Analyzer.IsAssembly(file))
                    {
                        assemblies.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    MessageBox.Show(string.Format("Could not read file {0}. {1}", file, ex.Message), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (recursive)
            {
                string[] directories = Directory.GetDirectories(directory);
                foreach (var dir in directories)
                {
                    assemblies.AddRange(GetAssemblies(dir, recursive, cancellationPendingCallback, progressReportCallback));
                }
            }
            return assemblies;
        }

        /// <summary>
        /// entry point from main form
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public AnalyzerResult AnalyzeRootAssembly(string assemblyName)
        {
            Trace.Listeners.Add(listener);
            Trace.Indent();
            Trace.WriteLine("AnalyzeRootAssembly " + assemblyName);
            AnalyzerResult ret = AnalyzeRootAssembly(assemblyName, false);

            if (Settings.Default.EnableStatusRollup)
            {
                RollupChildStatus(null, ret.RootAssembly);
            }
            Trace.Unindent();
            listener.Flush();
            listener.Close();
            return ret;
        }

        void RollupChildStatus(AsmData asmParent, AsmData asm)
        {
            foreach (var reference in asm.References)
            {
                RollupChildStatus(asm, reference);
            }
            if ((asmParent != null) &&
            (asm.Validity == AsmData.AsmValidity.ReferencesOnly || asm.Validity == AsmData.AsmValidity.Invalid) &&
            (asmParent.Validity == AsmData.AsmValidity.Valid || asmParent.Validity == AsmData.AsmValidity.Redirected || asmParent.Validity == AsmData.AsmValidity.CircularDependency || asmParent.Validity == AsmData.AsmValidity.ReferencesOnly))
            {
                asmParent.Validity = asm.Validity;
                asmParent.InvalidAssemblyDetails += "There is an issue with referenced assembly " + asm.AssemblyFullName + "\r\n";
            }
        }

        /// <summary>
        /// Analyzes assembly's references
        /// </summary>
        /// <param name="assemblyName">Full path to the assembly</param>
        /// <param name="throwWhenMissing">throws an exception when assembly reference is missing</param>
        /// <returns>AsmData object containing the information on given assembly's references</returns>
        /// <remarks>entry point from DirectoryScanner</remarks>
        public AnalyzerResult AnalyzeRootAssembly(string assemblyName, bool throwWhenMissing)
        {
            var result = new AnalyzerResult();
            Trace.Indent();
            Trace.WriteLine("AnalyzeRootAssembly " + assemblyName + ". Throw: " + throwWhenMissing);
            cache = new Dictionary<string, AsmData>();
            circularDependencyWarningShown = false;
            parentsStack = new Stack<string>();
            workingDir = Environment.CurrentDirectory;
            result.RootAssembly = new AsmData(assemblyName, Path.GetFullPath(assemblyName));
            if(!File.Exists(assemblyName))
            {
                Trace.WriteLine(assemblyName + " does not exist");
                result.RootAssembly.Path = "";
                result.RootAssembly.Validity = AsmData.AsmValidity.Invalid;
                result.RootAssembly.AssemblyFullName = "";
                Trace.Unindent();
                result.Problems.Add(new AssemblyNotFoundProblemDescriptor("{F5A8031A-1BC0-4647-8510-A4D8156E2CBD}", result.RootAssembly, assemblyName));
                return result;
            }
            
            Assembly asm = null;
            string fullPath = Path.GetFullPath(assemblyName);
            try
            {
                Trace.WriteLine("Loading from full path " + fullPath);
                asm = Assembly.LoadFrom(fullPath);
                result.RootAssembly.Validity = AsmData.AsmValidity.Valid;
            }
            catch (System.Exception ex)
            {
                var problem = new ExceptionProblemDescriptor("{5504E66C-4EB5-42A3-ABF9-8A9FBF4E7115}", result.RootAssembly, ex);
                problem.Detail = "Could not load the assembly using Assembly.LoadFrom(path). This is often caused by a failure to load a referenced assembly.";
                result.Problems.Add(problem);
                try
                {
                    Trace.WriteLine("trying ReflectionOnlyLoadFrom on " + fullPath);
                    asm = Assembly.ReflectionOnlyLoadFrom(fullPath);
                    result.RootAssembly.Validity = AsmData.AsmValidity.ReferencesOnly;
                    Trace.WriteLine("setting ReferencesOnly");
                }
                catch (Exception ex2)
                {
                    Trace.WriteLine("failed to load assembly");
                    asm = null;
                    result.RootAssembly.Validity = AsmData.AsmValidity.Invalid;
                    var problem2 = new ExceptionProblemDescriptor("{4C012401-A1CB-40D9-B0A6-1E0028A3C6BB}", result.RootAssembly, ex2);
                    problem2.Detail = "Could not load the assembly using Assembly.ReflectionOnlyLoadFrom(path)";
                    result.Problems.Add(problem2);
                }
            }
            if(asm!= null)
            {
                result.RootAssembly.AssemblyFullName = asm.FullName;
                result.RootAssembly.Path = asm.Location;
                result.RootAssembly.Architecture = asm.GetName().ProcessorArchitecture;
                Trace.WriteLine("Runtime version: " + asm.ImageRuntimeVersion);
                result.RootAssembly.RuntimeVersion = asm.ImageRuntimeVersion;
                currentLoadedArchitecture = result.RootAssembly.Architecture;

                var tempName = asm.GetName().Name;
                if (!assemblyNameToPathMap.ContainsKey(tempName))
                {
                    assemblyNameToPathMap.Add(tempName, asm.Location);
                }
                else
                {
                    Debug.Assert(assemblyNameToPathMap[tempName] == asm.Location);
                }
                Trace.WriteLine("Loading config & redirections");
                string cfgFilePath = Redirection.FindConfigFile(result.RootAssembly.Path);
                if (!string.IsNullOrEmpty(cfgFilePath) && !allVersionRedirections.ContainsKey(fullPath))
                {
                    var versionRedirections = Redirection.GetVersionRedirections(cfgFilePath);
                    var pathRedirections = Redirection.GetPathRedirections(result.RootAssembly.AssemblyFullName, cfgFilePath, result);
                    allVersionRedirections.Add(fullPath, versionRedirections);
                    probingPaths.Add(result.RootAssembly.AssemblyFullName, pathRedirections);
                    Trace.WriteLine("Loaded");
                }
                else
                {
                    Trace.WriteLine("Skipped");
                }
                AssemblyName[] references = asm.GetReferencedAssemblies();
                totalReferencedAssemblies = references.Length;
                parentsStack.Push(result.RootAssembly.AssemblyFullName);
                Trace.WriteLine("references:");
                foreach(AssemblyName asmName in references)
                {
                    AnalyzeAssembly(asmName, result.RootAssembly, throwWhenMissing, result);
                     //report progress
                    assembliesFinished++;
                    if(bgWorker != null)
                        bgWorker.ReportProgress(100 * (assembliesFinished)/ totalReferencedAssemblies);
                }
            }
            if (File.Exists(result.RootAssembly.Path))
            {
                Trace.WriteLine("trying imports");
                try
                {
                    Trace.WriteLine("trying imports");
                    result.RootAssembly.Imports = Win32Support.GetDllImports(result.RootAssembly.Path);
                }
                catch (System.Exception ex)
                {
                    Trace.WriteLine("Could not read imports of " + result.RootAssembly.Name + ".\r\nDetails: " + ex.Message);
                    //MessageBox.Show("Could not read imports of " + result.RootAssembly.Name + ".\r\nDetails: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    var importsProblem = new ExceptionProblemDescriptor("{8932131F-4B50-4071-AD7D-1601C69DBAE2}", result.RootAssembly, ex);
                    importsProblem.Detail = "Could not read imports table";
                    result.Problems.Add(importsProblem);
                }
            }
            parentsStack.Pop();
            Trace.Unindent();
            return result;
        }

     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processorArchitecture"></param>
        /// <returns></returns>
        private bool ApplyArchitecture(ProcessorArchitecture processorArchitecture)
        {
            if (currentLoadedArchitecture == ProcessorArchitecture.Amd64 ||
                currentLoadedArchitecture == ProcessorArchitecture.IA64 ||
                currentLoadedArchitecture == ProcessorArchitecture.X86)
            {
                //currentLoadedArchitecture stays from parent
                return IsCompatible(currentLoadedArchitecture, processorArchitecture);
            }
            if (currentLoadedArchitecture == ProcessorArchitecture.MSIL)
            {
                currentLoadedArchitecture = processorArchitecture;
                if (processorArchitecture == ProcessorArchitecture.None)
                    return false;
                return true;
            }
            return false;
        }

        private static bool IsCompatible(ProcessorArchitecture parent, ProcessorArchitecture child)
        {
            return architectureCompatibilityMatrix[(int)parent, (int)child];
        }

        /// <summary>
        /// Checks whether file is a .NET assembly
        /// </summary>
        /// <param name="path">path to the file</param>
        /// <returns>If file is valid assembly, returns true; otherwise returns false.</returns>
        public static bool IsAssembly(string fileName)
        {
             uint rva15value = 0;
             bool invalid = false;
             using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
             {
                 using (BinaryReader reader = new BinaryReader(fileStream))
                     {                         
                             if (0x3C > fileStream.Length)
                                 return false;

                             fileStream.Position = 0x3C; //PE Header start offset
                             uint headerOffset = reader.ReadUInt32();

                             fileStream.Position = headerOffset + 0x18;
                             if (fileStream.Position > fileStream.Length)
                             {
                                 return false;
                             }
                             UInt16 magicNumber = reader.ReadUInt16();

                             int dictionaryOffset;
                             switch (magicNumber)
                             {
                                 case 0x010B:
                                     dictionaryOffset = 0x60;
                                     break;
                                 case 0x020B:
                                     dictionaryOffset = 0x70;
                                     break;
                                 default:
                                     //Invalid Image Format
                                     invalid = true;
                                     dictionaryOffset = 0;
                                     break;
                             }
                             if (!invalid)
                             {//position to RVA 15
                                 fileStream.Position = headerOffset + 0x18 + dictionaryOffset + 0x70;

                                 //Read the value
                                 rva15value = reader.ReadUInt32();
                             }   
                     }
             }
             return rva15value != 0 && !invalid;
        }

        /// <summary>
        /// Checks for circular dependencies in all parents of given assembly name
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns>true if circular dependency was found</returns>
        private bool CheckCircularDependency(string fullName)
        {
            bool ret = false;
            ret = parentsStack.Contains(fullName);
            return ret;
            
        }

        /// <summary>
        /// used for reporting of circular dependency. Constructs string containing list of assemblies
        /// </summary>
        /// <returns></returns>
        private string GetParentsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string asm in parentsStack)
            {
                sb.Insert(0,asm + "\r\n");
            }
            return sb.ToString();
        }

        Dictionary<string, List<Redirection>> GetRedirections(string asmName)
        {
            string key;
            if (assemblyNameToPathMap.TryGetValue(asmName, out key))
            {
                if (allVersionRedirections.ContainsKey(key))
                {
                    return allVersionRedirections[key];
                }
            }
            else if(allVersionRedirections.ContainsKey(asmName))
            {
                return allVersionRedirections[asmName];
            }
     
            return null;
        }

        /// <summary>
        /// Analyzes assembly references. To be called only from AnalyzeRootAssembly(string,bool). This method uses recursion
        /// </summary>
        /// <param name="asmName"></param>
        /// <param name="parent"></param>
        private void AnalyzeAssembly(AssemblyName asmName, AsmData parent, bool throwWhenMissing, AnalyzerResult result)
        {
            Trace.Indent();
            bool redirectionApplied = false;
            string additionalInfo = string.Empty;
            AssemblyName analyzedAssembly = asmName;
            Trace.WriteLine("Analyzing " + analyzedAssembly.FullName);

            var asmRedirections = GetRedirections(parent.Name);
            if (asmRedirections != null)
            {
                analyzedAssembly = Redirection.GetCorrectAssemblyName(asmName, asmRedirections);
                redirectionApplied = analyzedAssembly.Version != asmName.Version;
            }

            bool invalid = false;   //switch valid/references only
            bool isInGac = false;
            AsmData gacAssemblyData = null;
            Assembly asm = null;
            string file = analyzedAssembly.FullName;
            bool isDifferentVersionInGac = false;
            string gacAssemblyName = string.Empty;
            string expectedVersion = string.Empty;
            try
            {
                foreach(AsmData item in gac)
                {
                    if(item.AssemblyFullName.Contains(analyzedAssembly.FullName))
                    {
                        //AsmData asmData = new AsmData(item.Name, item.Path);
                        //asmData.AssemblyFullName = item.AssemblyFullName;
                        //asmData.Validity = AsmData.AsmValidity.Valid;
                        //parent.References.Add(asmData);
                        Trace.WriteLine("Assembly is in GAC");
                        isInGac = true;
                        gacAssemblyData = item;
                        break;
                    }
                    isDifferentVersionInGac = isDifferentVersionInGac || IsSameAsmName(item.AssemblyFullName, analyzedAssembly.FullName);
                    if(isDifferentVersionInGac)
                    {
                        gacAssemblyName = item.AssemblyFullName;
                        expectedVersion = analyzedAssembly.FullName;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Trace.WriteLine("Exception thrown while iterating through GAC: " + ex);
            }//ignore all exceptions, this is only GAC check

            if (cache.ContainsKey(analyzedAssembly.FullName) && !parentsStack.Contains(analyzedAssembly.FullName)) //look in cache
            {
                AsmData cachedItem = cache[analyzedAssembly.FullName];
                parent.References.Add(cachedItem);
                Trace.WriteLine("Retrieving from cache.");
                Trace.Unindent();
                return;
            }

            AsmData currentAsmData = null;
            bool gacAssemblySet = false; //if true, will ignore checking for versions and such
            var currentProblems = new List<ProblemDescriptor>();
            if (!isInGac)        //if not loaded
            {
                string extAdd = "";     //get assembly file name
                if (file.LastIndexOf(", Version=") != -1)
                {
                    file = file.Substring(0, file.LastIndexOf(", Version="));
                }
                if (Path.GetExtension(file) != ".exe" && Path.GetExtension(file) != ".dll")
                {
                    extAdd = ".dll";
                }
                var tmpPath = FindPath(parent, file, new [] {".dll", ".exe"}, analyzedAssembly.FullName);
                if (!string.IsNullOrEmpty(tmpPath) && File.Exists(tmpPath))
                {
                    var localProblems = new List<ProblemDescriptor>();
                    TryLoad(ref additionalInfo, ref invalid, ref asm, tmpPath, out localProblems);
                    currentProblems.AddRange(localProblems);
                }
                else
                {
                    file += extAdd;
                    Trace.WriteLine("setting name to " + file);
                    var problem = new AssemblyNotFoundProblemDescriptor("{A5808A8C-1B89-4BE2-BD3C-CD6B88DBC1FC}", null, workingDir);
                    currentProblems.Add(problem);
                }
            }
            else // GAC assembly
            {
                try
                {
                    Trace.WriteLine("trying GAC assembly " + asmName);
                    //asm = Assembly.Load(asmName);
                    asm = Assembly.LoadFrom(gacAssemblyData.Path);
                    if (!gacAssemblyData.AssemblyFullName.Contains(asm.FullName) && (!asm.FullName.Contains(gacAssemblyData.AssemblyFullName)))
                    {
                        //loaded a different version given by a system redirection. Ignore this and mark as valid.
                        currentAsmData = gacAssemblyData;
                        gacAssemblySet = true;
                        asm = null;
                        var descriptor = new NotificationProblemDescriptor("{2B06CE46-3816-4B7A-9804-25D255B41CEF}", currentAsmData, "Version mismatch", "Referenced assembly was " + asmName + " but " +currentAsmData.AssemblyFullName +" got loaded.");
                        result.Problems.Add(descriptor);
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Trace.WriteLine("FileNotFoundException");
                    //assign no value, asm is null and invalid false, so the target validity is invalid.
                    additionalInfo = "File " + ex.FileName + " could not be found.";
                    var problem = new AssemblyNotFoundProblemDescriptor("{6CE23FA5-1CD6-434C-91D2-54BF726DCFA2}", null, ex.FileName);
                    currentProblems.Add(problem);
                }
                catch (FileLoadException ex)
                {
                    Trace.WriteLine("FileLoadException");
                    //assign no value, asm is null and invalid false, so the target validity is invalid.
                    additionalInfo = "File " + ex.FileName + " could not be loaded. " + ex.FusionLog;
                    var problem = new ExceptionProblemDescriptor("{F17DA9D6-C6BB-4D18-ADD5-D4824EF545CF}", null, ex);
                    currentProblems.Add(problem);
                }
                catch (BadImageFormatException ex)
                {
                    Trace.WriteLine("BadImageFormatException: " + ex);
                    //assign no value, asm is null and invalid false, so the target validity is invalid.
                    additionalInfo = "Bad image format. " + ex.ToString() + "\r\n" + ex.FusionLog;
                    var problem = new ExceptionProblemDescriptor("{EB9F988E-FBDA-464F-8AF1-6D690ABE7A54}", null, ex);
                    problem.Detail = "Bad image format.";
                    currentProblems.Add(problem);
                }
            }

            if (currentAsmData == null)
            {
                currentAsmData = new AsmData(analyzedAssembly.Name, asm == null ? "" : Path.GetFullPath(asm.Location));
                currentAsmData.AssemblyFullName = analyzedAssembly.FullName;
                currentAsmData.Validity = AsmData.AsmValidity.Invalid;
                currentAsmData.InvalidAssemblyDetails = additionalInfo;

                currentAsmData.Architecture = GetArchitecture(currentAsmData.Path);
            }
            foreach (var p in currentProblems)
            {
                p.Source = currentAsmData;
                result.Problems.Add(p);
            }

            //check for asm attributes, whether they are as same as attributes of asmName
            if (!gacAssemblySet && asm!= null && analyzedAssembly.Version != asm.GetName().Version)
            {
                string message = "Assembly was found with version " + asm.GetName().Version + " but parent references " + analyzedAssembly.Version;
                currentAsmData.AdditionalInfo = message;
                asm = null;
                Trace.WriteLine(message);
                var problem = new NotificationProblemDescriptor("{53980048-F604-4B70-8CDF-B47307C65941}", currentAsmData, "Version mismatch", message);
                result.Problems.Add(problem);
            }

            if (!gacAssemblySet && asm != null && !invalid)
            {
                try
                {
                    object[] attributes = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attributes.Length > 0)
                    {
                        currentAsmData.AssemblyProductName = ((AssemblyProductAttribute)attributes[0]).Product;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    currentAsmData.AssemblyProductName = "Product name could not be read.";
                    var problem = new NotificationProblemDescriptor("{BA2CECA2-928B-41D5-AA39-2F3E366CDDE6}", currentAsmData, "Product name could not be read.", ex.Message);
                    result.Problems.Add(problem);
                }
                catch (FileNotFoundException ex)
                {
                    currentAsmData.AssemblyProductName = "Product name could not be read. Assembly was loaded but later could not be found.";
                    var problem = new NotificationProblemDescriptor("{14CD7AA2-B375-4120-A92F-BFA24E9F7670}", currentAsmData, "Product name could not be read.", ex.Message);
                    result.Problems.Add(problem);
                }
                catch (Exception ex)
                {
                    currentAsmData.AssemblyProductName = "Product name could not be read. Error: " + ex.Message;
                    var problem = new ExceptionProblemDescriptor("{F9DE5828-DA04-437B-8405-E4D467389F59}", currentAsmData, ex);
                    problem.Detail = "Product name could not be read from the assembly.";
                    result.Problems.Add(problem);
                }
            }
            
            parent.References.Add(currentAsmData);
            if (invalid)
            {
                currentAsmData.Validity = AsmData.AsmValidity.ReferencesOnly;
                Trace.WriteLine("Valid: References only");
            }
            if (parentsStack.Contains(analyzedAssembly.FullName))
            {
                string circ = GetParentsString();
                Trace.WriteLine("Circular dependency: "+ analyzedAssembly.FullName + "\r\n\r\n" + circ + analyzedAssembly.FullName);
                currentAsmData.Validity = AsmData.AsmValidity.CircularDependency;
                if (!circularDependencyWarningShown)
                {
                    //this can be left out as we now have the "List of problems";
                    //MessageBox.Show("Circular dependency found: " + analyzedAssembly.FullName + "\r\n\r\n" + circ + analyzedAssembly.FullName + "\r\n\r\nPlease note this warning is shown only once during analysis.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    circularDependencyWarningShown = true;
                }
                var problem = new WarningProblemDescriptor("{E41A3EBF-6AAC-4BC2-8425-781655D01C2B}", currentAsmData, "Circular dependency detected", "This assembly references itself via one or more of its referenced assemblies.");
                result.Problems.Add(problem);
                Trace.Unindent();
                return;
            }
            if (asm != null)
            {
                currentAsmData.Path = asm.Location;
                currentAsmData.RuntimeVersion = asm.ImageRuntimeVersion;
                currentAsmData.AssemblyFullName = asm.FullName;
                if (!invalid)
                {
                    currentAsmData.Validity = redirectionApplied? AsmData.AsmValidity.Redirected : AsmData.AsmValidity.Valid;
                    currentAsmData.OriginalVersion = redirectionApplied ? asmName.Version.ToString() : string.Empty;
                    Trace.WriteLine("Valid: " + currentAsmData.Validity);
                }
                if (asm.CodeBase != null && (RuntimeEnvironment.FromGlobalAccessCache(asm)) && currentAsmData.AssemblyProductName == "Microsoft® .NET Framework") //do not analyze .NET framework assemblies
                {
                    Trace.WriteLine("Seems like .NET framework library. Don't scan");
                    Trace.Unindent();
                    return;
                }
                if(currentAsmData.Validity != AsmData.AsmValidity.Invalid && !ApplyArchitecture(currentAsmData.Architecture))
                {
                    currentAsmData.Validity = AsmData.AsmValidity.ReferencesOnly;
                    currentAsmData.AdditionalInfo += "\r\nProcessorArchitecture mismatch";
                    var problem = new WarningProblemDescriptor("{C3035851-6E70-4335-8B20-C5766641EAAF}", currentAsmData, "Architecture Mismatch", "The Architecture of this assembly is not compatible with the other assemblies loaded before this one.");
                    result.Problems.Add(problem);
                }

                parentsStack.Push(currentAsmData.AssemblyFullName);
                cache.Add(analyzedAssembly.FullName, currentAsmData);
                AssemblyName[] referenced = asm.GetReferencedAssemblies();
                foreach (AssemblyName n in referenced)
                {
                    AnalyzeAssembly(n, currentAsmData, throwWhenMissing, result);
                }
                parentsStack.Pop();
                if (File.Exists(currentAsmData.Path))
                {
                    try
                    {
                        Trace.WriteLine("trying imports");
                        currentAsmData.Imports = Win32Support.GetDllImports(currentAsmData.Path);
                    }
                    catch (System.Exception ex)
                    {
                        Trace.WriteLine("Could not read imports of " + currentAsmData.Name + ".\r\nDetails: " + ex.Message);
                        var problem = new WarningProblemDescriptor("{C8EEAC33-5DF7-448B-8EB7-5FBE4733D518}", currentAsmData, "Failed to read DLL imports", "An exception has been thrown while reading DLL imports: " + ex.Message);
                        result.Problems.Add(problem);
                    }
                }
            }
            else if (isDifferentVersionInGac && !gacAssemblySet)
            {
                currentAsmData.Validity = AsmData.AsmValidity.ReferencesOnly;
                currentAsmData.AdditionalInfo += "An assembly with the specified version could not be found. GAC contains an assembly with the same name, but different version.";
                currentAsmData.AdditionalInfo += "\r\nExpected: " + expectedVersion;
                currentAsmData.AdditionalInfo += "\r\nActual: " + gacAssemblyName;
                var problem = new NotificationProblemDescriptor("{798D856C-A895-4C7F-9261-E5FE21605DD4}", currentAsmData, "Version mismatch", currentAsmData.AdditionalInfo);
                result.Problems.Add(problem);
            }
            else if (throwWhenMissing && !gacAssemblySet)
            {
                Trace.Unindent();
                var problem = new ExceptionProblemDescriptor("{24F12AFD-8C9A-40C8-9053-43641122BC11}", currentAsmData, new Exception("returning from analysis"));
                problem.Detail = "Something went terribly wrong...";
                result.Problems.Add(problem);
                throw new Exception("returning from analysis");
            }
            else
            {
                //Directory scanner goes here. Do nothing, this is valid reference
            }
            Trace.Unindent();
        }

        private static bool IsSameAsmName(string assembly1, string assembly2)
        {
            var index = assembly2.IndexOf(",");
            if (index == -1)
                return false;

            return assembly1.StartsWith(assembly2.Substring(0, index));
        }

        private static ProcessorArchitecture GetArchitecture(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var descriptor = AssemblyName.GetAssemblyName(path);
                    if (descriptor != null)
                    {
                        return descriptor.ProcessorArchitecture;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed to get architecture for " + path);
                    Trace.WriteLine("Error:" + ex);
                }
            }
            return ProcessorArchitecture.None;
        }

        private string FindPath(AsmData parent, string file, string[] extAdd, string currentFullName)
        {
            string ret = "";
            try
            {
                Trace.Indent();
                foreach (var ext in extAdd)
                {
                    Trace.WriteLine("FindPath " + currentFullName);
                    var parentDir = workingDir;
                    if (parent != null && !string.IsNullOrEmpty(parent.Path))
                    {
                        parentDir = Path.GetDirectoryName(parent.Path);
                    }
                    Trace.WriteLine("parentDir: " + parentDir);
                    var tmpPath = file + ext;
                    if (File.Exists(tmpPath))
                    {
                        Trace.WriteLine("File exists in " + tmpPath);
                        return tmpPath;
                    }
                    tmpPath = Path.Combine(parentDir, file + ext);
                    if (File.Exists(tmpPath))
                    {
                        Trace.WriteLine("File exists in " + tmpPath);
                        return tmpPath;
                    }
                    PathRedirection redirects;
                    ret = Path.Combine(parentDir, file + ext);
                    if (!probingPaths.TryGetValue(parent.AssemblyFullName, out redirects))
                    {
                        continue;
                    }

                    foreach (var currentDir in redirects.Directories)
                    {
                        string targetDir = currentDir;
                        if (!Path.IsPathRooted(currentDir))
                        {
                            targetDir = Path.Combine(parentDir, currentDir);
                        }
                        if (File.Exists(Path.Combine(targetDir, file + ext)))
                        {
                            var targetPath = Path.Combine(targetDir, file + ext);
                            Trace.WriteLine("Found redirection. Target path is " + targetPath);
                            return targetPath;
                        }
                    }
                }
            }
            finally
            {
                Trace.Unindent();
            }
            ret = "";//Path.ChangeExtension(ret, ".dll");
            Trace.WriteLine("The assembly is most likely missing. Returning empty string.");
            return ret;
        }

        private static void TryLoad(ref string additionalInfo, ref bool invalid, ref Assembly asm, string tmpPath, out List<ProblemDescriptor> problems)
        {
            problems = new List<ProblemDescriptor>();
            try    //try to load file
            {
                Trace.WriteLine("trying " + tmpPath);
                asm = Assembly.LoadFrom(tmpPath);
            }
            catch (Exception ex)    //file is not OK, load only reflection information
            {
                var problem = new ExceptionProblemDescriptor("{9E8D5F38-CFE6-42C6-826A-2AFBBD860FC3}", null, ex);
                problem.Detail = "Could not load the assembly using Assembly.LoadFrom(path). This is often caused by a failure to load a referenced assembly.";
                problems.Add(problem);

                Trace.WriteLine("Try load first error: " + ex);
                invalid = true;
                Trace.WriteLine("trying ReflectionOnlyLoadFrom " + tmpPath);
                try
                {
                    asm = Assembly.ReflectionOnlyLoadFrom(tmpPath);
                }
                catch (FileLoadException ex2)
                {
                    Trace.WriteLine("FileLoadException");
                    //assign no value, asm is null and invalid false, so the target validity is invalid.
                    additionalInfo = "File " + ex2.FileName + " could not be loaded. " + ex2.FusionLog;
                    var problem2 = new ExceptionProblemDescriptor("{9C151988-DA3C-4A9D-A974-3FF3E5FA8B7D}", null, ex2);
                    problems.Add(problem2);
                }
                catch (BadImageFormatException ex2)
                {
                    Trace.WriteLine("BadImageFormatException: " + ex);
                    //assign no value, asm is null and invalid false, so the target validity is invalid.
                    additionalInfo = "Bad image format. " + ex2.ToString() + "\r\n" + ex2.FusionLog;
                    problem = new ExceptionProblemDescriptor("{EB9F988E-FBDA-464F-8AF1-6D690ABE7A54}", null, ex2);
                    problem.Detail = "Bad image format.";
                    problems.Add(problem);
                }
                catch (Exception ex2)
                {
                    Trace.WriteLine("ReflectionOnly load failed: " + ex2);
                    additionalInfo = "Unexpected error. " + ex2 + "\r\n";
                    problem = new ExceptionProblemDescriptor("{650D53DC-B77B-4C47-8CFB-012DB1F12BE2}", null, ex2);
                    problem.Detail = "Unexpected error occured when loading the assembly.";
                    problems.Add(problem);
                }
            }
        }

        public override object InitializeLifetimeService()
        {
            ILease tmp = (ILease)base.InitializeLifetimeService();
            if (tmp.CurrentState == LeaseState.Initial)
            {
                tmp.InitialLeaseTime = TimeSpan.FromSeconds(10);
                tmp.RenewOnCallTime = TimeSpan.FromSeconds(10);
            }
            return tmp;
        }

        TimeSpan ISponsor.Renewal(ILease lease)
        {
            return TimeSpan.FromSeconds(2);
        }
    }
}
