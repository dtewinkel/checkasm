using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amberfish.Canvas.Model
{
    public class NodeEventArgs : EventArgs
    {
        public NodeModel NewNode { get; private set; }

        private NodeEventArgs() { }

        public NodeEventArgs(NodeModel newNode)
        {
            NewNode = newNode;
        }
    }
}
