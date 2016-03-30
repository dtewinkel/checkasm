using CheckAsm.Licensing.Security;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckAsm.Licensing
{
    class Eval : LicenseTypeBase
    {
        public override void LogAndDisplayLicenseTypeMessage()
        {
            Trace.WriteLine("This is an evaluation version");
            ComparisonMatrix dialog = new ComparisonMatrix();
            dialog.ResourceName = "matrix_eval";
            dialog.ShowDialog();
        }

        public override void Validate()
        {
            Trace.WriteLine("Reading evaluation status");
            DateTime installedDate;
            var isEval = GetEvalStatus(out installedDate);
            if (isEval)
            {
                //verify installed date
                var store = new SecureStorage();
                try
                {
                    var fileContent = store.LoadFile("amberfish.lic");
                    var str = ASCIIEncoding.ASCII.GetString(fileContent);
                    if (str != installedDate.ToString("MM-dd-yyyy"))
                    {
                        throw new LicenseValidationException("License validation failed", "The evaluation license is not valid");
                    }
                }
                catch(FileNotFoundException)
                {
                    store.SaveFile("amberfish.lic", Encoding.ASCII.GetBytes(installedDate.ToString("MM-dd-yyyy")));
                }
                Trace.WriteLine("Evaluation period is active since " + installedDate);
                if (installedDate > DateTime.Now.Date || ((DateTime.Now - installedDate).Days >= 5))
                {
                    throw new LicenseValidationException("Eval expired", "The evaluation period has expired. Please visit www.amberfish.net to get a free or full version.");
                }
                return;
            }
            throw new LicenseValidationException("Evaluation registration info not found.");
        }

        private static bool GetEvalStatus(out DateTime installedDate)
        {
            installedDate = DateTime.MinValue;
            var key = Registry.LocalMachine.OpenSubKey("Software\\amberfishnet\\checkasm");
            if (key == null)
                return false;
            var installValue = key.GetValue("install");
            if (installValue == null)
                return false;

            var dt = installValue.ToString();
            var parts = dt.Split('-');
            try
            {
                installedDate = new DateTime(int.Parse(parts[2]), int.Parse(parts[0]), int.Parse(parts[1]));
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }
    }
}
