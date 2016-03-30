using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using CheckAsm.Descriptors;
using System.Security;

namespace CheckAsm
{
    /// <summary>
    /// Contains information on assembly redirection and provides methods for reading the information from app config files.
    /// </summary>
    [Serializable]
    public class Redirection
    {

        AssemblyName assemblyIdentity;
        BindingRedirect bindingRedirection;

        /// <summary>
        /// Original assembly identity
        /// </summary>
        public AssemblyName AssemblyIdentity
        {
            get { return assemblyIdentity; }
            set { assemblyIdentity = value; }
        }

        /// <summary>
        /// Assembly redirection information
        /// </summary>
        public BindingRedirect BindingRedirection
        {
            get { return bindingRedirection; }
            set { bindingRedirection = value; }
        }

        /// <summary>
        /// Gets a path to an application configuration file.
        /// </summary>
        /// <param name="appFilePath">Path to the application executable file.</param>
        /// <returns>Full path to the application configuration file. If file does not exist, return value is null.</returns>
        public static string FindConfigFile(string appFilePath)
        {
            string fileName = Path.GetFileName(appFilePath);
            string directory = Path.GetDirectoryName(appFilePath);

            string configName = string.Format("{0}\\{1}.config", Path.GetDirectoryName(appFilePath), Path.GetFileName(appFilePath)).Trim('\\');
            if (File.Exists(configName))
                return configName;
            else
            {
                configName = string.Format("{0}\\web.config", Path.GetDirectoryName(appFilePath));
                if (File.Exists(configName))
                    return configName;
                else
                    return null;
            }
                
        }

        /// <summary>
        /// Gets list of redirections with all .NET Framework DLLs
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<Redirection>> GetFrameworkRedirections(List<AsmData> assembliesInGac, ProgressDialog progressDialog)
        {
            AppDomain redirectionsScanDomain;
            string callingDomainName = Thread.GetDomain().FriendlyName;
            string exeAssembly = Assembly.GetEntryAssembly().FullName;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Environment.CurrentDirectory;
            setup.DisallowBindingRedirects = false;
            setup.DisallowCodeDownload = true;
            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            redirectionsScanDomain = AppDomain.CreateDomain("Framework Redirections");
            redirectionsScanDomain.UnhandledException += redirectionsScanDomain_UnhandledException;
            FrameworkRedirectionsScanner scanner = (FrameworkRedirectionsScanner)redirectionsScanDomain.CreateInstanceAndUnwrap(exeAssembly, typeof(FrameworkRedirectionsScanner).FullName);
            //Dictionary<string, List<Redirection>> redirections = scanner.GetFrameworkRedirections(assembliesInGac, progressDialog);
            AppDomain.Unload(redirectionsScanDomain);
            return new Dictionary<string, List<Redirection>>();//redirections;
        }

        static void redirectionsScanDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorMessageBox.Show("An unhandled exception occured while parsing .NET Framework assemblies. We are sorry for any inconvenience caused.\r\nError message: " + ((Exception)e.ExceptionObject).Message, e.ExceptionObject.ToString());
        }

        public static PathRedirection GetPathRedirections(string assemblyName, string configFile, AnalyzerResult result)
        {
            if(!File.Exists(configFile))
                return new PathRedirection{AssemblyName = assemblyName};            

            var redirect = new PathRedirection { AssemblyName = assemblyName };

            try
            {
                XmlDocument config = new XmlDocument();
                config.Load(configFile);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(config.NameTable);
                nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:asm.v1");

                var nav = config.CreateNavigator();
                var probingNode = nav.SelectSingleNode("/configuration/runtime/x:assemblyBinding/x:probing", nsmgr);
                if (probingNode != null)
                {
                    var privatePath = probingNode.GetAttribute("privatePath", string.Empty);
                    if (!string.IsNullOrEmpty(privatePath))
                    {
                        string[] paths = privatePath.Split(';');
                        foreach (var p in paths)
                        {
                            try
                            {
                                redirect.Directories.Add(Path.GetFullPath(p));
                            }
                            catch(ArgumentException ex)
                            {
                                var problem = new ConfigFileFormatProblemDescriptor("{74E13704-6E2F-429F-A89F-9A52905CEE25}", null, configFile, ex);
                                problem.Detail = "Assembly configuration file contains one or more errors in probingPath configuration.";
                                result.Problems.Add(problem);
                            }
                        }
                    }
                }
                return redirect;
            }
            catch(XmlException ex)
            {
                result.Problems.Add(new ConfigFileFormatProblemDescriptor("{58DF63AC-F68C-4673-805B-760AEC044C29}", null, configFile, ex));
                return new PathRedirection { AssemblyName = assemblyName }; 
            }
            catch(IOException ex)
            {
                result.Problems.Add(new ExceptionProblemDescriptor("{750F7DF3-0901-426D-8CDB-016BF628A9A6}", null, ex));
                return new PathRedirection { AssemblyName = assemblyName }; 
            }
            catch(SecurityException ex)
            {
                result.Problems.Add(new ExceptionProblemDescriptor("{A8F0144E-5B74-4DF0-AF9D-A18BC28CEC52}", null, ex));
                return new PathRedirection { AssemblyName = assemblyName }; 
            }
            catch(Exception ex)
            {
                result.Problems.Add(new ExceptionProblemDescriptor("{D5F16243-D6F7-4B49-AE8E-A351F9D69E8E}", null, ex));
                return new PathRedirection { AssemblyName = assemblyName }; 
            }
        }

        /// <summary>
        /// Parses config file and reads redirection information from it.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Dictionary&lt;string, Redirection&gt; where key is simple assembly name. If redirection information is empty or not valid, return value is null.</returns>
        public static Dictionary<string, List<Redirection>> GetVersionRedirections(string fileName)
        {
           
                Dictionary<string, List<Redirection>> ret = new Dictionary<string, List<Redirection>>();
                XmlDocument config = new XmlDocument();
                config.Load(fileName);
                XmlNodeList dependentAssemblyTags = config.GetElementsByTagName("dependentAssembly");
                foreach (XmlNode dependentAssemblyTag in dependentAssemblyTags)
                {

                    if (!(dependentAssemblyTag.ParentNode.Name == "assemblyBinding" && dependentAssemblyTag.ParentNode.ParentNode != null && dependentAssemblyTag.ParentNode.ParentNode.Name == "runtime"))
                        continue;

                    Redirection red = new Redirection();
                    foreach (XmlNode node in dependentAssemblyTag.ChildNodes)
                    {
                        if (node.Name == "assemblyIdentity")
                        {
                            AssemblyName name = new AssemblyName(node.Attributes["name"].Value);
                            if (node.Attributes["processorArchitecture"] != null)
                            {
                                name.ProcessorArchitecture = (ProcessorArchitecture)Enum.Parse(typeof(ProcessorArchitecture), node.Attributes["processorArchitecture"].Value, true);
                            }

                            red.AssemblyIdentity = name;

                        }
                        else if (node.Name == "bindingRedirect")
                        {
                            BindingRedirect redirect = new BindingRedirect();
                            if (node.Attributes["oldVersion"] != null)
                            {
                                XmlAttribute attr = node.Attributes["oldVersion"];
                                if (attr.Value.Contains("-"))
                                {
                                    string[] versions = attr.Value.Split('-');
                                    redirect.OldVersionMin = new Version(versions[0]);
                                    redirect.OldVersionMax = new Version(versions[1]);
                                }
                                else
                                {
                                    redirect.OldVersionMax = new Version(attr.Value);
                                    redirect.OldVersionMin = new Version(attr.Value);
                                }
                            }
                            if (node.Attributes["newVersion"] != null)
                            {
                                redirect.NewVersion = new Version(node.Attributes["newVersion"].Value);
                            }
                            red.BindingRedirection = redirect;
                        }
                    }
                    if (ret.ContainsKey(red.AssemblyIdentity.Name))
                    {
                        ret[red.AssemblyIdentity.Name].Add(red);
                    }
                    else
                    {
                        ret.Add(red.AssemblyIdentity.Name, new List<Redirection> { red });
                    }
                }

                if (ret.Count > 0)
                    return ret;

                return null;
           
        }

        /// <summary>
        /// Gets AssemblyName object according to Redirection information.
        /// </summary>
        /// <param name="original">AssemblyName</param>
        /// <param name="dic">Dictionary object containing the redirection information. Use ParseConfigFile method to get this object</param>
        /// <returns>AssemblyName as required by the application configuration file</returns>
        public static AssemblyName GetCorrectAssemblyName(AssemblyName original, Dictionary<string, List<Redirection>> dic)
        {
            if (dic.ContainsKey(original.Name))
            {
                foreach (Redirection redirection in dic[original.Name])
                {
                    if (redirection.BindingRedirection == null)
                    {
                        Trace.WriteLine("Redirection data is invalid: " + redirection.AssemblyIdentity);
                        continue;
                    }
                    Version redirectVersionMin = redirection.BindingRedirection.OldVersionMin;
                    Version redirectVersionMax = redirection.BindingRedirection.OldVersionMax;

                    if (original.Version >= redirectVersionMin && original.Version <= redirectVersionMax)
                    {   //do the redirection
                        AssemblyName name = new AssemblyName(original.FullName);
                        name.Version = redirection.BindingRedirection.NewVersion;
                        name.ProcessorArchitecture = original.ProcessorArchitecture;
                        name.SetPublicKeyToken(original.GetPublicKeyToken());
                        return name;
                    }
                }
            }
            return original;
        }



        class NamespaceResolver : IXmlNamespaceResolver
        {
            public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
            {
                return null;
            }

            public string LookupNamespace(string prefix)
            {
                return null;
            }

            public string LookupPrefix(string namespaceName)
            {
                return null;
            }
        }


    }
    
}
