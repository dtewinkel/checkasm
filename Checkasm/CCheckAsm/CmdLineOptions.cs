using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCheckAsm
{
    [Serializable]
    class CmdLineOptions
    {
        public bool LoggingEnabled { get; set; }
        public string File { get; set; }
        public string OutputFile { get; set; }

        public static CmdLineOptions Parse(string[] args)
        {
            var options = new CmdLineOptions();
            options.File = args[0];
            options.LoggingEnabled = args.Contains("-l", StringComparer.InvariantCultureIgnoreCase);

            foreach (var arg in args)
            {
                if (arg.StartsWith("-o:", StringComparison.InvariantCultureIgnoreCase))
                {
                    options.OutputFile = arg.Substring(3).Trim('\"');
                }
                
            }

            return options;
        }
    }
}
