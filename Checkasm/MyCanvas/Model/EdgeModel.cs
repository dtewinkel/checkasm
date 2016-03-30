using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amberfish.Canvas.Model
{
    public class EdgeModel
    {
        NodeModel startPoint;
        NodeModel endPoint;

        public Guid Id { get; private set; }

        public NodeModel StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }
     
        public NodeModel EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        public EdgeModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
