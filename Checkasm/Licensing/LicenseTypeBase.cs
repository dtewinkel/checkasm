using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm.Licensing
{
    abstract class LicenseTypeBase
    {
        public abstract void LogAndDisplayLicenseTypeMessage();
        public abstract void Validate();
    }
}
