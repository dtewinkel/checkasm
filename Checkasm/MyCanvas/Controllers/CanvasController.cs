using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amberfish.Canvas.Model;
using System.Drawing;
using Amberfish.Canvas.Parsers;

namespace Amberfish.Canvas.Controllers
{
    public class CanvasController
    {
        public event EventHandler InvalidateRequired;

        public CanvasModel Model { get; private set; }

        private CanvasController() { }

        public CanvasController(CanvasModel model)
        {
            Model = model;
        }

        public Guid AddNode(PointF location, string text)
        {
            var node = new NodeModel();
            node.Location = location;
            node.Text = text;
            node.RaiseEvents = true;
            
            Model.AddNode(node);
            node.LocationChanged += new EventHandler(node_LocationChanged);
            node.EdgeColorChanged += new EventHandler(node_EdgeColorChanged);
            return node.Id;
        }

        void node_EdgeColorChanged(object sender, EventArgs e)
        {
            OnInvalidateRequired();
        }

        public Guid AddNode(PointF location)
        {
            return AddNode(location, Guid.NewGuid().ToString());
        }

        public void CreateGraphFromString(string graph)
        {
            Dictionary<string, Guid> nodes = new Dictionary<string, Guid>();
            DotParser parser = new DotParser();
            parser.Load(graph);
            
            var locx = 50;
            var locy = 100;

            foreach (var n in parser.GetNodes())
            {
                var g = AddNode(new PointF(locx, locy), n);
                nodes.Add(n, g);
                locx += 200;
                if (locx > 1000)
                {
                    locx = 50;
                    locy += 100;
                }
            }

            foreach (var edge in parser.GetEdges())
            {
                AddEdge(nodes[edge.Item1], nodes[edge.Item2]);
            }

        }

        void node_LocationChanged(object sender, EventArgs e)
        {
            OnInvalidateRequired();
        }

        public void AddEdge(Guid startPointId, Guid endPointId)
        {
            Model.AddEdge(startPointId, endPointId);
        }

        protected void OnInvalidateRequired()
        {
            var handler = InvalidateRequired;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }



        public int ApplyDirectedForceLayout()
        {
            return Model.ApplyDirectedForceLayout();           
        }

        public void ApplyHierarchicalLayout()
        {
            Model.ApplyHierarchicalLayout();
        }

        public void ApplyRingLayout()
        {
            Model.ApplyRingLayout();
        }

        public void ApplyCircularLayout()
        {
            Model.ApplyCircularLayout();
        }

        public void BringToView(int availableWidth, int availableHeight)
        {
            Model.BringToView(availableWidth, availableHeight);
        }

        public void HighlightNeighbors(Guid nodeId, Color neighborColor, Color nodeColor)
        {
            Model.IgnoreNodeEdgeColor = true;
            Model.ClearHighlight(neighborColor);
            Model.HighlightNeighbors(nodeId, neighborColor);
            Model.SetNodeColor(nodeId, nodeColor);
        }

        public void SetNodeColor(Guid nodeId, Color color)
        {
            Model.IgnoreNodeEdgeColor = false;
            Model.SetNodeColor(nodeId, color);
        }

        public void UpdateShadowNodes(IEnumerable<string> shadowNodeNames)
        {
            foreach (var shadow in shadowNodeNames)
            {
                var node = Model.GetNodeModelByName(shadow);
                if(node != null)
                {
                    node.IsShadowNode = true;
                }
            }
        }
    }
}
