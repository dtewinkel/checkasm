using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckAsm.Licensing
{
    class Free : LicenseTypeBase
    {
        public override void LogAndDisplayLicenseTypeMessage()
        {
            Trace.WriteLine("This is a free version");
            ComparisonMatrix dialog = new ComparisonMatrix();
            dialog.ResourceName = "matrix_free";
            dialog.ShowDialog();
        }

        public override void Validate()
        {
            return;
        }
    }
}
