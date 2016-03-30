using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;

namespace CheckAsm
{
    internal class FrameworkRedirectionsScanner : MarshalByRefObject
    {

        TextWriterTraceListener listener = new TextWriterTraceListener(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\checkasm.log", "CheckAsm logging");

        /// <summary>
        /// reads registry and parses GAC for .net framework assemblies and creates a list of redirections according to .net framework version
        /// </summary>
        /// <param name="assembliesInGac"></param>
        /// <returns></returns>
        public Dictionary<string, List<Redirection>> GetFrameworkRedirections(List<AsmData> assembliesInGac, ProgressDialog progressDialog)
        {
            Dictionary<string, List<Redirection>> redirections = new Dictionary<string, List<Redirection>>();
            Trace.Listeners.Add(listener);
            try
            {
                Trace.WriteLine("Checking .NET Framework libraries redirections");
                progressDialog.ReportProgress(0, "Checking .NET Framework libraries...");
                int assembliesCount = 0;

                RegistryKey upgrades = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\Policy\Upgrades");

                if (upgrades == null)
                {
                    Trace.WriteLine("no upgrades available");
                    listener.Flush();
                    listener.Close();
                    return redirections;
                }
                List<BindingRedirect> bindingRedirects = new List<BindingRedirect>();

                string[] targetVersions = upgrades.GetValueNames();
                foreach (string targetVersion in targetVersions)
                {
                    string sourceVersion = upgrades.GetValue(targetVersion) as string;
                    BindingRedirect redirect = new BindingRedirect();
                    redirect.NewVersion = new Version(targetVersion);
                    if (sourceVersion.Contains("-"))
                    {
                        string[] versions = sourceVersion.Split('-');
                        redirect.OldVersionMin = new Version(versions[0]);
                        redirect.OldVersionMax = new Version(versions[1]);
                    }
                    else
                    {
                        redirect.OldVersionMax = new Version(sourceVersion);
                        redirect.OldVersionMin = new Version(sourceVersion);
                    }
                    bindingRedirects.Add(redirect);
                }
                upgrades.Close();

                foreach (AsmData assemblyDescription in assembliesInGac)
                {
                    Assembly asm = null;
                    try
                    {
                        Debug.WriteLine("Loading " + assemblyDescription.Name);
                        asm = Assembly.Load(assemblyDescription.Name);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Skipping " + assemblyDescription.Name + ". Error: " + ex.Message);
                        continue;
                    }

                    AssemblyName assemblyName = asm.GetName(false);
                    if (redirections.ContainsKey(assemblyName.Name))
                        continue;

                    object[] attributes = null;
                    try
                    {
                        attributes = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Failed to read custom attributes: " + ex.Message);
                    }
                    if (attributes != null && attributes.Length > 0)
                    {
                        AssemblyProductAttribute productAttribute = attributes[0] as AssemblyProductAttribute;
                        if (productAttribute != null && productAttribute.Product == "Microsoft® .NET Framework")
                        {
                            foreach (BindingRedirect bindingRedirect in bindingRedirects)
                            {
                                Redirection redirection = new Redirection();
                                redirection.AssemblyIdentity = assemblyName;
                                redirection.BindingRedirection = bindingRedirect;
                                if (assemblyName.Version <= redirection.BindingRedirection.NewVersion)
                                {
                                    redirection.BindingRedirection.NewVersion = assemblyName.Version;
                                }
                                if (redirections.ContainsKey(redirection.AssemblyIdentity.Name))
                                {
                                    redirections[redirection.AssemblyIdentity.Name].Add(redirection);
                                }
                                else
                                {
                                    redirections.Add(redirection.AssemblyIdentity.Name, new List<Redirection> { redirection });
                                }
                                Trace.WriteLine("Added redirection " + redirection.AssemblyIdentity.Name + ": " + redirection.BindingRedirection.OldVersionMin + " - " + redirection.BindingRedirection.OldVersionMax + " -> " + redirection.BindingRedirection.NewVersion);
                            }
                        }
                    }
                    assembliesCount++;
                    progressDialog.ReportProgress((int)(100.0 * assembliesCount / (double)assembliesInGac.Count), "Checking .NET Framework libraries...");
                    if (progressDialog.CancellationPending)
                    {
                        Trace.WriteLine("Cancelled");
                        redirections.Clear();
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            listener.Flush();
            listener.Close();
            return redirections;
        }
    } 
}
