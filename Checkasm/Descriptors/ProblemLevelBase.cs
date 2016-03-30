using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CheckAsm.Descriptors
{
    [Serializable]
    public abstract class ProblemLevelBase
    {
        public Icon Icon { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public Color Color { get; set; }
    }

   

    [Serializable]
    public class ProblemError : ProblemLevelBase
    {
        public ProblemError()
        {
            Icon = null;
            Name = "Error";
            Level = 1;
            Color = Color.OrangeRed;
        }
    }

    [Serializable]
    public class ProblemWarning : ProblemLevelBase
    {
        public ProblemWarning()
        {
            Icon = null;
            Name = "Warning";
            Level = 2;
            Color = Color.Orange;
        }
    }

    [Serializable]
    public class ProblemInfo : ProblemLevelBase
    {
        public ProblemInfo()
        {
            Icon = null;
            Name = "Info";
            Level = 3;
            Color = Color.Blue;
        }
    }
}
