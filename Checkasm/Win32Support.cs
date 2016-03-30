using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace CheckAsm
{
    class Win32Support
    {
        private static bool win32supportPresent;
        
        public static void VerifyStatus()
        {
            win32supportPresent = true;
            string dir = Path.GetDirectoryName(Application.ExecutablePath);
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr));
            if (size == 8) //x64
            {
                Trace.WriteLine("CheckAsm does not fully support x64 platform. Reading DLL imports is now disabled.");
            }
            else
                if (!File.Exists(dir + "\\win32support.dll"))
                {
                    win32supportPresent = false;
                    Trace.WriteLine("CheckAsm can not find file win32support.dll. Some functions may not be available.\r\n Update to the most recent version is recommended.");
                }
        }

        /// <summary>
        /// looks for specified file
        /// </summary>
        /// <param name="fileName">file to look for</param>
        /// <param name="path">(output) full path of the file if found.</param>
        /// <returns>true, if file exists; otherwise returns false.</returns>
        private static bool FindFile(string fileName, out string path)
        {
            path = "";
            if (File.Exists(fileName))
            {
                path = Path.GetFullPath(fileName);
            }

            if (String.IsNullOrEmpty(path)) path = LookInKnownDlls(fileName);
            if (String.IsNullOrEmpty(path)) path = LookInSystemDir(fileName);
            if (String.IsNullOrEmpty(path)) path = LookInPath(fileName);

            return !String.IsNullOrEmpty(path);
        }

        [DllImport("win32support.dll")]
        private static extern void GetDllImports([MarshalAs(UnmanagedType.BStr)]ref string path, [MarshalAs(UnmanagedType.BStr)]ref string output);

        /// <summary>
        /// Gets list of modules imported by a specified module (PE file)
        /// </summary>
        /// <param name="dllFile">Full path to the file.</param>
        /// <returns>Array of LibraryImport objects</returns>
        /// <remarks>if win32support.dll library is missing, empty array is returned.</remarks>
        public static LibraryImport[] GetDllImports(string dllFile)
        {
            if (!win32supportPresent)
                return new LibraryImport[] { };

            List<LibraryImport> ret = new List<LibraryImport>();
            SxsComponent[] sxsComponents = new SxsComponent[] { };
            string rawImports = "";
            try
            {
                GetDllImports(ref dllFile, ref rawImports);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Could not read DLL imports of file " + dllFile + ". Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new LibraryImport[] { }; 
            }

            rawImports = rawImports.TrimEnd('*');
            string[] imports = rawImports.Split('*');
            XmlDocument manifest = GetManifest(dllFile);
            if (manifest != null)
            {
                sxsComponents = GetManifestDependencies(manifest);
            }
            foreach (string import in imports)
            {
                string filePath;
                bool found = FindFile(import, out filePath);
                ret.Add(new LibraryImport(import, filePath, found));
            }
            foreach (LibraryImport li in ret)
            {
                if (!li.Exists)
                {
                    foreach (SxsComponent sxs in sxsComponents)
                    {
                        string dir = sxs.GetFullPath();
                        if (!String.IsNullOrEmpty(dir))
                        {
                            string[] files = Directory.GetFiles(dir);
                            foreach (string f in files)
                            {
                                if (Path.GetFileName(f).ToLower() == li.FileName.ToLower())
                                {
                                    li.Exists = true;
                                    li.FullPath = sxs.GetFullPath() + "\\" + li.FileName;
                                    break;
                                }
                            }
                            if (li.Exists)
                                break;
                        }
                    }
                }
            }
            return ret.ToArray();
        }

        /// <summary>
        /// Gets manifest from a PE file
        /// </summary>
        /// <param name="file">Full path to the file.</param>
        /// <returns>Xmldocument object containing the manifest. If file does not contain manifest, return value is null.</returns>
        private static XmlDocument GetManifest(string file)
        {
            XmlDocument doc = new XmlDocument();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbMissing = new StringBuilder();
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            List<byte> fileContent = new List<byte>();
            long parts = fs.Length / (long)1204;
            for (long i = 0; i < parts; i++)
            {
                byte[] bytes = new byte[1204];
                bytes = br.ReadBytes(1204);
                fileContent.AddRange(bytes);
            }
            fileContent.AddRange(br.ReadBytes((int)(fs.Length / (long)1204)));
            br.Close();
            fs.Close();

            byte[] fileContentBytes = fileContent.ToArray();
            List<char> manifestBytes = new List<char>();
            bool manifestComplete = false;
            for (long i = 0; i < fileContentBytes.Length && !manifestComplete; i++)
            {
                //look for "<assembly" (from the end of the word) - find beginning of manifest
                if (fileContentBytes[i] == Convert.ToByte('y') &&
                   fileContentBytes[i - 1] == Convert.ToByte('l') &&
                   fileContentBytes[i - 2] == Convert.ToByte('b') &&
                   fileContentBytes[i - 3] == Convert.ToByte('m') &&
                   fileContentBytes[i - 4] == Convert.ToByte('e') &&
                   fileContentBytes[i - 5] == Convert.ToByte('s') &&
                   fileContentBytes[i - 6] == Convert.ToByte('s') &&
                   fileContentBytes[i - 7] == Convert.ToByte('a') &&
                   fileContentBytes[i - 8] == Convert.ToByte('<'))
                {
                    manifestBytes.Clear();
                    long startIndex = i - 8;
                    for (long j = startIndex; j < fileContentBytes.Length; j++) //find end of manifest and load it
                    {
                        //</assembly>
                        manifestBytes.Add(Convert.ToChar(fileContentBytes[j]));
                        if (fileContentBytes[j] == Convert.ToByte('>') &&
                           fileContentBytes[j - 1] == Convert.ToByte('y') &&
                           fileContentBytes[j - 2] == Convert.ToByte('l') &&
                           fileContentBytes[j - 3] == Convert.ToByte('b') &&
                           fileContentBytes[j - 4] == Convert.ToByte('m') &&
                           fileContentBytes[j - 5] == Convert.ToByte('e') &&
                           fileContentBytes[j - 6] == Convert.ToByte('s') &&
                           fileContentBytes[j - 7] == Convert.ToByte('s') &&
                           fileContentBytes[j - 8] == Convert.ToByte('a') &&
                           fileContentBytes[j - 9] == Convert.ToByte('/') &&
                           fileContentBytes[j - 10] == Convert.ToByte('<'))
                        {
                            StringBuilder sb2 = new StringBuilder();
                            foreach (char c in manifestBytes)
                            {
                                sb2.Append(c);
                            }
                            string manifestString = sb2.ToString();
                            try
                            {
                                doc.LoadXml(manifestString);
                                manifestComplete = true;
                                break;
                            }
                            catch (XmlException) { }
                        }
                    }
                }
            }
            if (!manifestComplete)
            {
                doc = null;
            }
            return doc;
        }

        /// <summary>
        /// Gets list of side-by-side assemblies imported by a specified PE file.
        /// </summary>
        /// <param name="manifest">PE file manifest</param>
        /// <returns>Array of SxsComponent objects</returns>
        private static SxsComponent[] GetManifestDependencies(XmlDocument manifest)
        {
            List<SxsComponent> ret = new List<SxsComponent>();
            XmlNodeList assemblyIndentities = manifest.GetElementsByTagName("assemblyIdentity");
            foreach (XmlNode asmIdentity in assemblyIndentities)
            {
                string name = "";
                if(asmIdentity.Attributes["name"] != null) name = asmIdentity.Attributes["name"].Value;
                string version = "";
                if(asmIdentity.Attributes["version"] != null) version = asmIdentity.Attributes["version"].Value;
                string architecture = "";
                if(asmIdentity.Attributes["processorArchitecture"] !=null) architecture = asmIdentity.Attributes["processorArchitecture"].Value;

                SxsComponent sxs = new SxsComponent(name, version, architecture);
                ret.Add(sxs);
            }
            return ret.ToArray();
        }

        /// <summary>
        /// Looks for a specified file in "known Dlls" list
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>If file is on the "known Dlls" list, return string contains full path to the file.</returns>
        private static string LookInKnownDlls(string fileName)
        {
            RegistryKey knownDlls = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Session Manager\KnownDLLs");
            if (knownDlls != null)
            {
                object oDir = knownDlls.GetValue("DllDirectory");
                if (oDir != null)
                {
                    string dir = Environment.ExpandEnvironmentVariables(oDir.ToString());
                    fileName = dir + "\\" + Path.GetFileName(fileName);
                    if (File.Exists(fileName))
                    {
                        return fileName;
                    }
                }
                knownDlls.Close();
            }
            return "";
        }

        /// <summary>
        /// Looks for a specified file in all directories specified by %PATH% system variable
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>If file is found, return string contains full path to the file.</returns>
        private static string LookInPath(string fileName)
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            List<string> dirs = new List<string>(path.Split(';'));
            foreach (string dir in dirs)
            {
                if (File.Exists(dir + "\\" + fileName))
                    return dir + "\\" + fileName;
            }
            return "";
        }

        /// <summary>
        /// Looks for a specified file in system, system32 and Windows directory
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>If file is found, return string contains full path to the file.</returns>
        private static string LookInSystemDir(string fileName)
        {
            string dir = Environment.ExpandEnvironmentVariables("%SystemRoot%\\system32");
            if (File.Exists(dir + "\\" + fileName))
                return dir + "\\" + fileName;

            dir = Environment.ExpandEnvironmentVariables("%SystemRoot%\\system");
            if (File.Exists(dir + "\\" + fileName))
                return dir + "\\" + fileName;

            dir = Environment.ExpandEnvironmentVariables("%SystemRoot%");
            if (File.Exists(dir + "\\" + fileName))
                return dir + "\\" + fileName;

            return "";
        }
    }
}
