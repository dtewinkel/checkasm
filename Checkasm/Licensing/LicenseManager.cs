using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm.Licensing
{
    class LicenseManager
    {
        private static readonly LicenseManager _instance = new LicenseManager();
        
        private LicenseManager()
        {

        }

        public static LicenseManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public LicenseTypeBase GetLicense()
        {
            LicenseTypeBase license = new Full();

#if EVAL
            license = new Eval();
#endif
#if FREE
            license = new Free();
#endif

            return license;
        }
    }
}
