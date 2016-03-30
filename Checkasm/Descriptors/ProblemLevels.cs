using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm.Descriptors
{
    static class ProblemLevels
    {
        public static ProblemLevelBase Error
        {
            get
            {
                return new ProblemError();
            }
        }

       

        public static ProblemLevelBase Warning
        {
            get
            {
                return new ProblemWarning();
            }
        }

        public static ProblemLevelBase Info
        {
            get
            {
                return new ProblemInfo();
            }
        }
    }
}
