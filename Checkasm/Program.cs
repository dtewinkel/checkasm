using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;
using CheckAsm.Properties;
using Microsoft.Win32;
using CheckAsm.Licensing;

namespace CheckAsm
{
    
    static class Program
    {

        public static readonly string LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\checkasm.log";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                StartLogging();
                RunSelfDiagnostics(args);

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var licence = Licensing.LicenseManager.Instance.GetLicense();
                licence.LogAndDisplayLicenseTypeMessage();
                try
                {
                    licence.Validate();
                }
                catch (LicenseValidationException ex)
                {
                    Trace.Write("License validation failed: " + ex);
                    Trace.Flush();
                    MessageBox.Show(ex.UserMessage, "License validation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Settings.Default.Upgrade();
                if (!Settings.Default.EulaAccepted)
                {
                    using (var form = new EulaForm())
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            Settings.Default.EulaAccepted = true;
                            Settings.Default.Save();
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                Trace.WriteLine("Starting CheckAsm " + Assembly.GetExecutingAssembly().GetName().Version);
                Application.Run(new MainDialog(args));
            }
            finally
            {
                StopLogging();
            }
        }

        private static void RunSelfDiagnostics(string[] args)
        {
            //check win32 support
            Trace.WriteLine("Verifying win32support.dll");
            Win32Support.VerifyStatus();            
        }

        public static bool IsX64
        {
            get
            {
                return (Marshal.SizeOf(typeof(IntPtr)) == 8);
            }
        }

        private static void RunElevated(string[] arguments)
        {
            StringBuilder args = new StringBuilder();
            foreach(var a in arguments)
            {
                args.AppendFormat("\"{0}\" ", a);
            }
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = "checkasm.exe";
            processInfo.Arguments = args.ToString().Trim();
            try
            {
                Process.Start(processInfo);
            }
            catch (Win32Exception)
            {
                //Do nothing. Probably the user canceled the UAC window
            }
        }


        static TextWriterTraceListener listener;

        public static void StartLogging()
        {
            listener = new TextWriterTraceListener(LogFilePath, "CheckAsm logging");
            Trace.Listeners.Add(listener);
            listener.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;
        }

        public static void StopLogging()
        {
            Trace.Flush();
            Trace.Listeners.Remove(listener);
            listener.Close();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorMessageBox.Show("CheckAsm has encountered an error from which is unable to recover. Try to restart the application and repeat your action again.", e.ExceptionObject.ToString());
            StopLogging();
            Environment.Exit(1);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Trace.Flush();
            ErrorMessageBox.Show("CheckAsm has encountered an unexpected error. " + e.Exception.Message, e.Exception.ToString());
        }
    }
}