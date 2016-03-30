using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CheckAsm.Licensing
{
    class Full:LicenseTypeBase
    {
        public override void LogAndDisplayLicenseTypeMessage()
        {
            Trace.WriteLine("This is a full version");
        }

        public override void Validate()
        {
            return;
        }
    }
}
