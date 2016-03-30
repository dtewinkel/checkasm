using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Amberfish.Canvas.Model;
using Amberfish.Canvas.Controllers;
using System.Drawing.Drawing2D;

namespace Amberfish.Canvas.Views
{
    public partial class CanvasView : UserControl
    {
        readonly Color defaultEdgeColor = Color.FromArgb(115, 115, 115);

        public CanvasModel Model { get; private set; }
        public CanvasController Controller { get; private set; }

        public Color HighlightedEdgeColor { get; set; }

        private readonly List<EdgeModel> edges = new List<EdgeModel>();

        public bool Suspend { get; set; }

        /// <summary>
        /// Context menu added to all new nodes
        /// </summary>
        [EditorBrowsable]
        public ContextMenuStrip NodeContextMenu { get; set; }

        public CanvasView()
        {
            InitializeComponent();

            Model = new CanvasModel();
            Controller = new CanvasController(Model);

            Model.NodeAdded += new EventHandler<NodeEventArgs>(Model_NodeAdded);
            Model.EdgeAddded += new EventHandler<EdgeEventArgs>(Model_EdgeAddded);
            Controller.InvalidateRequired += new EventHandler(Controller_InvalidateRequired);
            
            HighlightedEdgeColor = defaultEdgeColor;
        }

        public void Reset()
        {
            Model.NodeAdded -= new EventHandler<NodeEventArgs>(Model_NodeAdded);
            Model.EdgeAddded -= new EventHandler<EdgeEventArgs>(Model_EdgeAddded);
            Controller.InvalidateRequired -= new EventHandler(Controller_InvalidateRequired);

            Model = new CanvasModel();
            Controller = new CanvasController(Model);
            edges.Clear();
            Controls.Clear();

            Model.NodeAdded += new EventHandler<NodeEventArgs>(Model_NodeAdded);
            Model.EdgeAddded += new EventHandler<EdgeEventArgs>(Model_EdgeAddded);
            Controller.InvalidateRequired += new EventHandler(Controller_InvalidateRequired);
            Invalidate();
        }

        void Controller_InvalidateRequired(object sender, EventArgs e)
        {
            if (!Suspend)
            {
                Invalidate();
            }
        }

        void Model_EdgeAddded(object sender, EdgeEventArgs e)
        {
            edges.Add(e.NewEdge);
            if (!Suspend)
            {
                Invalidate();
            }
        }

        void Model_NodeAdded(object sender, NodeEventArgs e)
        {
            var view = new NodeView();
            view.Model = e.NewNode;
            view.ContextMenuStrip = this.NodeContextMenu;
            this.Controls.Add(view);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Suspend)
            {
                DrawEdges(e.Graphics);
            }
            base.OnPaint(e);
        }

        private void DrawEdges(Graphics gr)
        {
            foreach (var edge in edges)
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var penColor = edge.StartPoint.EdgeColor;
                if (Model.IgnoreNodeEdgeColor)
                {
                    penColor = defaultEdgeColor;
                    if(Model.ShouldBeColored(edge.Id))
                    {
                        penColor = HighlightedEdgeColor;
                    }
                }

                using (var pen = new Pen(penColor))
                {
                    AdjustableArrowCap cap = new AdjustableArrowCap(5, 8);
                    pen.CustomEndCap = cap;

                    var path = FindShortestPath(edge.StartPoint, edge.EndPoint);

                    var start = path.Item1;
                    var end = path.Item2;

                    var actualEndX = start.X + (end.X - start.X) * 1;
                    var actualEndY = start.Y + (end.Y - start.Y) * 1;

                    //gr.DrawLine(pen, edge.StartPoint.Location, new Point((int)(edge.EndPoint.Location.X) - 20, (int)(edge.EndPoint.Location.Y - 20)));
                    gr.DrawLine(pen, start, new Point((int)actualEndX, (int)actualEndY));
                }
            }
        }

        public void SaveImageAs(string fileName)
        {
            using (var bmp = new Bitmap(Width, Height))
            {
                using (var gr = Graphics.FromImage(bmp))
                {
                    DrawEdges(gr);
                    DrawToBitmap(bmp, new Rectangle(new Point(0, 0), Size));
                    bmp.Save(fileName);
                }
            }
        }

        public string GetState()
        {
            return Model.GetState();
        }

        public void LoadState(string state)
        {
            Model.LoadState(state);
        }

        private Tuple<PointF, PointF> FindShortestPath(NodeModel startPoint, NodeModel endPoint)
        {
            var startPoints = new List<PointF>();
            var endPoints = new List<PointF>();
            foreach(var connector in startPoint.Connectors)
            {
                startPoints.Add(GetCanvasCoordinates(startPoint, connector));
            }
            foreach (var connector in endPoint.Connectors)
            {
                endPoints.Add(GetCanvasCoordinates(endPoint, connector));
            }

            var shortestPath = double.MaxValue;
            PointF actualStartConnector = startPoints[0];
            PointF actualEndConnector = endPoints[0];
            foreach (var startConnector in startPoints)
            {
                foreach (var endConnector in endPoints)
                {
                    var distance = GetDistance(startConnector, endConnector);
                    if (distance < shortestPath)
                    {
                        shortestPath = distance;
                        actualStartConnector = startConnector;
                        actualEndConnector = endConnector;
                    }
                }
            }

            return new Tuple<PointF, PointF>(actualStartConnector, actualEndConnector);
        }

        private double GetDistance(PointF startPoint, PointF endPoint)
        {
            var a = startPoint.X - endPoint.X;
            var b = startPoint.Y - endPoint.Y;
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }

        private PointF GetCanvasCoordinates(NodeModel node, PointF nodeLocation)
        {
            return new PointF(node.View.Location.X + nodeLocation.X, node.View.Location.Y + nodeLocation.Y);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            //Controller.AddNode(new PointF(e.X, e.Y));
        }

    }
}
