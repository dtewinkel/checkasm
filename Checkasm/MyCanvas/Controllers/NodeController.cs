using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amberfish.Canvas.Model;
using System.Drawing;

namespace Amberfish.Canvas.Controllers
{
    public class NodeController
    {
        Log log = new Log();
        public NodeModel Model { get; private set; }

        private NodeController() { }



        public NodeController(NodeModel model)
        {
            Model = model;
        }

        internal void StartDrag()
        {
            log.Info("StartDrag");
            Model.IsDragging = true;
        }

        internal void EndDrag()
        {
            log.Info("EndDrag");
            Model.IsDragging = false;
        }

        public void DragTo(PointF location)
        {
            if (Model.IsDragging)
            {
                log.Info("DragTo: " + location);
                Model.Location = location;
            }
        }
    }
}
