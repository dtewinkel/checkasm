using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm
{
    /// <summary>
    /// Contains information on binding redirection
    /// </summary>
    [Serializable]
    public class BindingRedirect
    {
        Version oldVersionMin;
        Version oldVersionMax;
        Version newVersion;

        /// <summary>
        /// Minimum old version
        /// </summary>
        public Version OldVersionMin
        {
            get { return oldVersionMin; }
            set { oldVersionMin = value; }
        }

        /// <summary>
        /// Maximum old version
        /// </summary>
        public Version OldVersionMax
        {
            get { return oldVersionMax; }
            set { oldVersionMax = value; }
        }

        /// <summary>
        /// New version
        /// </summary>
        public Version NewVersion
        {
            get { return newVersion; }
            set { newVersion = value; }
        }
    }
}
