using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amberfish.Canvas.Model
{
    public class EdgeEventArgs:EventArgs
    {
        public EdgeModel NewEdge { get; private set; }

        private EdgeEventArgs() { }

        public EdgeEventArgs(EdgeModel newEdge)
        {
            NewEdge = newEdge;
        }
    }
}
