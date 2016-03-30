using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CheckAsm;
using System.IO;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;
using CheckAsm.ManagedGAC;

namespace CCheckAsm
{
    [Serializable]
    class Program
    {
        Dictionary<string, List<Redirection>> assemblyRedirections;
        Dictionary<string, List<Redirection>> gacFrameworkRedirections;
        List<AsmData> gacAssemblies = new List<AsmData>();
        bool loggingEnabled = true;

        string workingDir;
        string fileToAnalyze;
        AsmData rootAssembly;

        AppDomain analyzerDomain;
        IAnalyzer analyzer;

        BackgroundWorker bgWorker;

        CmdLineOptions cmdLineOptions;

        int result;

        static object syncRoot = new object();

        static int Main(string[] args)
        {
            Program p = new Program();
            if (args.Length == 0)
            {
                PrintUsage("This application requires parameters.");
                Environment.Exit(-10);
                return -10;
            }

            var options = CmdLineOptions.Parse(args);

            var retCode = p.OpenFile(options);
            Environment.Exit(retCode);
            return retCode;
        }

        private static void PrintUsage(string errorMessage)
        {
            var usage = new StringBuilder("Console CheckAsm\r\n");
            if (!string.IsNullOrEmpty(errorMessage))
            {
                usage.AppendLine(errorMessage + "\r\n");
            }
            usage.AppendLine("Usage: CCheckAsm <filename> [-l] [-o:output.xml]");
            usage.AppendLine("Options: -l: enable logging to console");
            usage.AppendLine("Options: -o: specify output file");
            usage.AppendLine("---------------------------------\r\n");
            usage.AppendLine("Examples: CCheckAsm myAssembly.dll -o:\"C:\\checkasm results\\myassemblydll.xml\"");
            usage.AppendLine("          CCheckAsm myAssembly.dll -l -o:myassemblydll.xml");

            Console.WriteLine(usage);
        }

        public Program()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
        }

        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Log(e.ProgressPercentage + "% " + e.UserState);
        }

        public void Log(string message)
        {
            if (loggingEnabled)
            {
                Console.WriteLine("X: " + message);
            }
        }

        /// <summary>
        /// worker start function (start of assembly analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            analyzer.Gac = gacAssemblies;
            //analyzer.Redirections = assemblyRedirections;
            var result = analyzer.AnalyzeRootAssembly(fileToAnalyze);
            rootAssembly = result.RootAssembly;
        }

        /// <summary>
        /// analysis finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("An error occured while processing assembly. " + e.Error.Message, e.Error.ToString());
                if (analyzer != null)
                {
                    AppDomain.Unload(analyzerDomain);
                    analyzerDomain = null;
                    analyzer = null;
                }
                lock (syncRoot)
                {
                    Monitor.Pulse(syncRoot);
                }
            }
            else
            {
                WorkCompleted();
            }
            
        }

        private void WorkCompleted()
        {
            result = -40;
            if (rootAssembly != null)
            {
                SaveOutput();
                result = 0 - (int)rootAssembly.Validity;
                if (result == 0)
                {
                    result = GetResult(rootAssembly.References);
                }
            }
            lock (syncRoot)
            {
                Monitor.Pulse(syncRoot);
            }
        }

        private int GetResult(List<AsmData> references)
        {
            var result = 0;
            foreach (var reference in references)
            {
                result = 0 - ((int)reference.Validity);
                if (result == 0)
                {
                    result = GetResult(reference.References);
                }
                else
                    return result;
            }
            return result;
        }

        private void SaveOutput()
        {
            var serializer = new XmlSerializer(typeof(AsmData));
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            serializer.Serialize(writer, rootAssembly);

            if (string.IsNullOrEmpty(cmdLineOptions.OutputFile))
            {
                Console.WriteLine(builder.ToString());
            }
            else
            {
                try
                {
                    using (var sw = new StreamWriter(cmdLineOptions.OutputFile))
                    {
                        sw.Write(builder.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Loads file and starts analysis
        /// </summary>
        /// <param name="file"></param>
        public int OpenFile(CmdLineOptions options)
        {
            cmdLineOptions = options;
            if (string.IsNullOrEmpty(options.File.Trim()) || !File.Exists(options.File))
            {
                Console.WriteLine("Invalid file name");
                return -50;
            }

            Log("Loading GAC");
            gacAssemblies = AssemblyCache.GetAssembliesInGac(null, 1000);

            Log("Loading Config file");
            string appConfigFile = Redirection.FindConfigFile(options.File);
            if (!string.IsNullOrEmpty(appConfigFile))
            {
                try
                {
                    assemblyRedirections = Redirection.GetVersionRedirections(appConfigFile);
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.ToString());
                    Console.WriteLine("Failed to read assembly config file " + ex);
                    Console.WriteLine("Application configuration file contains errors. Make sure it is valid XML.");
                    return -20;
                }
            }

            if (assemblyRedirections == null && gacFrameworkRedirections != null)
            {
                assemblyRedirections = new Dictionary<string, List<Redirection>>(gacFrameworkRedirections);
            }
            else if (gacFrameworkRedirections != null)
            {
                foreach (KeyValuePair<string, List<Redirection>> redirection in gacFrameworkRedirections)
                {
                    assemblyRedirections.Add(redirection.Key, redirection.Value);
                }
            }

            Log("Setting up appdomain");
            if (File.Exists(options.File))
            {
                workingDir = Path.GetDirectoryName(options.File);
                if (!string.IsNullOrEmpty(workingDir))
                {
                    Environment.CurrentDirectory = workingDir;
                }
                string callingDomainName = Thread.GetDomain().FriendlyName;
                string exeAssembly = Assembly.GetAssembly(typeof(Analyzer)).FullName;
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = Environment.CurrentDirectory;
                setup.DisallowBindingRedirects = false;
                setup.DisallowCodeDownload = true;
                setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                analyzerDomain = AppDomain.CreateDomain("Analyzer");
                analyzerDomain.UnhandledException += new UnhandledExceptionEventHandler(analyzerDomain_UnhandledException);
                analyzer = (Analyzer)analyzerDomain.CreateInstanceAndUnwrap(exeAssembly, typeof(Analyzer).FullName);
                analyzer.BgWorker = bgWorker;
                string message;
                if (!analyzer.IsValidAssembly(options.File, out message))
                {
                    Console.WriteLine(message);
                    if (analyzer != null)
                    {
                        AppDomain.Unload(analyzerDomain);
                        analyzerDomain = null;
                        analyzer = null;
                    }
                    return -30;
                }


                fileToAnalyze = options.File;
                
                bgWorker.RunWorkerAsync(fileToAnalyze);
                lock (syncRoot)
                {
                    Log("Initializing...");
                    Monitor.Wait(syncRoot);
                    return result;
                }
            }
            else
            {
                Console.WriteLine(string.Format("File {0} could not be found.", options.File));
                return 101;
            }
        }

        public void analyzerDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.ExceptionObject.ToString());
        }
    }
}
