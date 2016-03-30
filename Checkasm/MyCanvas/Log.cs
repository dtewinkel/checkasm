using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Amberfish.Canvas
{
    public class Log
    {
        public string Source { get; set; }

        public Log()
        {
            Source = new StackFrame(1, false).GetMethod().DeclaringType.ToString();
        }

        public Log(string source)
        {
            Source = source;
        }

        public void Info(string message)
        {
            Trace.WriteLine(Source + ":" + message);
        }
    }
}
