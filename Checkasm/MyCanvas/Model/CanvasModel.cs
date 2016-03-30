using Amberfish.Canvas.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Amberfish.Canvas.Model
{
    public class CanvasModel
    {
        Log log = new Log();
        private readonly Dictionary<Guid, NodeModel> nodes = new Dictionary<Guid, NodeModel>();
        private readonly Dictionary<Guid, EdgeModel> edges = new Dictionary<Guid, EdgeModel>();

        private readonly HashSet<Guid> coloredEdges = new HashSet<Guid>();

        public event EventHandler<NodeEventArgs> NodeAdded;
        public event EventHandler<EdgeEventArgs> EdgeAddded;

        public bool IgnoreNodeEdgeColor { get; set; }

        public void AddNode(NodeModel node)
        {
            nodes.Add(node.Id, node);
            OnNodeAdedd(node);
        }

        public void AddEdge(Guid startPoint, Guid endPoint)
        {
            var edge = new EdgeModel()
            {
                StartPoint = nodes[startPoint],
                EndPoint = nodes[endPoint]
            };
            edges.Add(edge.Id, edge);
            OnEdgeAdedd(edge);
        }

        protected void OnNodeAdedd(NodeModel node)
        {
            var handler = NodeAdded;
            if (handler != null)
            {
                handler(this, new NodeEventArgs(node));
            }
        }

        protected void OnEdgeAdedd(EdgeModel edge)
        {
            var handler = EdgeAddded;
            if (handler != null)
            {
                handler(this, new EdgeEventArgs(edge));
            }
        }

        internal void ApplyHierarchicalLayout()
        {
            var nodeList = new List<NodeModel>(nodes.Values);
            foreach (var node in nodeList)
            {
                node.RaiseEvents = false;
                node.LayoutLevelValue = 0;
            }
            var nodesToLayout = new Dictionary<Guid, NodeModel>();

            foreach (var edge in edges.Values) //go through all edges and collect list of nodes, calculate level
            {
                if (!nodesToLayout.ContainsKey(edge.StartPoint.Id))
                {
                    nodesToLayout.Add(edge.StartPoint.Id, edge.StartPoint);
                }
                if (!nodesToLayout.ContainsKey(edge.EndPoint.Id))
                {
                    nodesToLayout.Add(edge.EndPoint.Id, edge.EndPoint);
                }
                edge.StartPoint.LayoutLevelValue++;
                //edge.EndPoint.LayoutLevelValue++;
            }

            var levelsList = new List<int>();
            foreach (var n in nodeList)
            {
                if (!levelsList.Contains(n.LayoutLevelValue))
                {
                    levelsList.Add(n.LayoutLevelValue);
                }
            }
            levelsList.Sort();

            foreach (var n in nodeList)
            {
                n.LayoutLevelValue = levelsList.IndexOf(n.LayoutLevelValue);
            }

            var levels = new Dictionary<int, int>(); //level number, node count

            var xBaseOffset = 0;

            foreach (var node in nodesToLayout.Values)
            {
                var yOffset = node.LayoutLevelValue * 120; //increase for more vertical space
                var xOffset = 0;
                if (levels.ContainsKey(node.LayoutLevelValue))
                {
                    xOffset = 270 * levels[node.LayoutLevelValue]; //increase for more horizontal space
                    levels[node.LayoutLevelValue]++;
                    xBaseOffset += 15;
                }
                else
                {
                    levels.Add(node.LayoutLevelValue, 1);
                    xBaseOffset += 15;
                }
                node.Location = new System.Drawing.PointF(xOffset + xBaseOffset, yOffset);
            }


            foreach (var node in nodeList)
            {
                node.RaiseEvents = true;
            }
        }

        public int CalculateIntersections()
        {
            List<LineSegment> segments = new List<LineSegment>();
            foreach (var edge in edges.Values)
            {
                segments.Add(new LineSegment(new Tuple<double, double>(edge.StartPoint.Location.X, edge.StartPoint.Location.Y), new Tuple<double, double>(edge.EndPoint.Location.X, edge.EndPoint.Location.Y)));
            }
            int intersections = 0;
            for (int i = 0; i < segments.Count - 1; i++)
            {
                for (int j = i + 1; j < segments.Count; j++)
                {
                    if (segments[i].IntersectsWith(segments[j]))
                        intersections++;
                }
            }

            return intersections;
        }

        internal void ApplyRingLayout()
        {
            var nodeList = new List<NodeModel>(nodes.Values);
            foreach (var node in nodeList)
            {
                node.RaiseEvents = false;
                node.LayoutLevelValue = 0;
            }

            var root = FindRoot(nodeList);

            foreach (var node in nodeList)
            {
                node.RaiseEvents = true;
            }
        }

        private NodeModel FindRoot(List<NodeModel> nodeList)
        {
            int maxRetries = 1;
            var cache = new HashSet<Guid>();
            var random = new Random();
            for (int i = 0; i < maxRetries; i++)
            {
                
            }
            throw new NotImplementedException();
        }

        internal void ApplyCircularLayout()
        {
            var nodeList = new List<NodeModel>(nodes.Values);
            foreach (var node in nodeList)
            {
                node.RaiseEvents = false;
                node.LayoutLevelValue = 0;
            }
            var nodesToLayout = new Dictionary<Guid, NodeModel>();
            var maxLevel = 0;

            foreach (var edge in edges.Values) //go through all edges and collect list of nodes, calculate level
            {
                if (!nodesToLayout.ContainsKey(edge.StartPoint.Id))
                {
                    nodesToLayout.Add(edge.StartPoint.Id, edge.StartPoint);
                }
                if (!nodesToLayout.ContainsKey(edge.EndPoint.Id))
                {
                    nodesToLayout.Add(edge.EndPoint.Id, edge.EndPoint);
                }
                edge.StartPoint.LayoutLevelValue++;
                //edge.EndPoint.LayoutLevelValue++;
                
            }

            nodeList.Sort(new Comparison<NodeModel>((a, b) => { return a.LayoutLevelValue.CompareTo(b.LayoutLevelValue); }));

            var angle = (2 * Math.PI) / nodeList.Count;
            var radius = GetRadius(nodeList.Count);

            var currentAngle = 0.0;
            foreach (var node in nodeList)
            {
                node.Location = GetNodePosition(new PointF(0, 0), radius, currentAngle);
                currentAngle += angle;
            }

            //optimize intersections
            var currentBest = CalculateIntersections();
            if (currentBest > 0)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    for (int j = 1; j < nodeList.Count; j++)
                    {
                        if (nodeList[i].Id != nodeList[j].Id)
                        {
                            ExchangeNodesLocation(nodeList[i], nodeList[j]);
                            var intersections = CalculateIntersections();
                            Trace.WriteLine(string.Format("{0},{1},{2}, {3}", i , j, intersections, currentBest));
                            if (intersections <= currentBest)
                            {
                                currentBest = intersections;
                                Trace.WriteLine(currentBest);
                            }
                            else
                            {
                                ExchangeNodesLocation(nodeList[i], nodeList[j]);
                            }
                        }
                    }
                }
            }

            foreach (var node in nodeList)
            {
                node.RaiseEvents = true;
            }
        }

        private void ExchangeNodesLocation(NodeModel nodeModel1, NodeModel nodeModel2)
        {
            var tmp = nodeModel1.Location;
            nodeModel1.Location = nodeModel2.Location;
            nodeModel2.Location = tmp;
        }

        private float GetRadius(int count)
        {
            var s = 150.0;
            return (float)((s * count) / (2 * Math.PI));
        }

        private PointF GetNodePosition(PointF center, double radius, double angle)
        {
            var x = radius * Math.Cos(angle);
            var y = radius * Math.Sin(angle);
            return new PointF((float)x, (float)y);
        }

        internal int ApplyDirectedForceLayout()
        {
            var nodeList = new List<NodeModel>(nodes.Values);
            var edgeList = new List<EdgeModel>(edges.Values);
            var n = nodeList.Count;
            foreach (var node in nodeList)
            {
                node.RaiseEvents = false;
                node.View.Visible = false;
            }
            var totalVelocityX = 0.0;
           
            for(int i = 0; i < n; i++) // loop through vertices
            {
                var v = nodeList[i];
                NodeModel u;
                
                v.NetForce.X = v.NetForce.Y = 0;
                for(int j = 0; j < n; j++) // loop through other vertices
                {
                    if(i == j)
                        continue;
                    u = nodeList[j]; 
                    // squared distance between "u" and "v" in 2D space
                    double rsq = ((v.Location.X-u.Location.X)*(v.Location.X-u.Location.X)+(v.Location.Y-u.Location.Y)*(v.Location.Y-u.Location.Y));
                    
                    // counting the repulsion between two vertices 
                    v.NetForce.X += 1600 * (v.Location.X-u.Location.X) /rsq;
                    v.NetForce.Y += 1600 * (v.Location.Y-u.Location.Y) /rsq;
                }

                for(int j = 0; j < n; j++) // loop through edges
                {
                    u = nodeList[j];
                    var match = from e in edgeList where e.StartPoint == v && e.EndPoint == u select e;
                    if (match.Count() == 0)
                        continue;
                    
                    // counting the attraction
                    v.NetForce.X += 0.06*(u.Location.X - v.Location.X);
                    v.NetForce.Y += 0.06*(u.Location.Y - v.Location.Y);
                }
                // counting the velocity (with damping 0.85)
                v.Velocity.X = (v.Velocity.X + v.NetForce.X)*0.93; 
                v.Velocity.Y = (v.Velocity.Y + v.NetForce.Y)*0.90;
                totalVelocityX += Math.Sqrt(v.Velocity.X * v.Velocity.X + v.Velocity.Y * v.Velocity.Y);
            }

            for(int i = 0; i < n; i++) // set new positions
            {
                var v = nodeList[i];
                v.Location = new System.Drawing.PointF((float)(v.Velocity.X), (float)(v.Velocity.Y));
                
            }

            log.Info("total velocity: " + totalVelocityX );
            foreach (var node in nodeList)
            {
                node.RaiseEvents = true;
                node.View.Visible = true;
            }
            return CalculateIntersections();
        }

        public string GetState()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<NodeModel>));
            using (var memStream = new MemoryStream())
            {
                serializer.Serialize(memStream, nodes.Values.ToList());
                return Encoding.Default.GetString(memStream.GetBuffer());
            }
        }

        public void LoadState(string state)
        {
            using (var memStream = new MemoryStream(Encoding.Default.GetBytes(state)))
            {
                var serializer = new XmlSerializer(typeof(List<NodeModel>));
                var list = (List<NodeModel>)serializer.Deserialize(memStream);

                foreach (var node in list)
                {
                    nodes[node.Id].Location = node.Location;
                }
            }
        }

        public void BringToView(double availableWidth, double availableHeight)
        {
            var nodeList = new List<NodeModel>(nodes.Values);
            var n = nodeList.Count;

            var minX = 0.0;
            var minY = 0.0;
            var maxX = 0.0;
            var maxY = 0.0;

            for (int i = 0; i < n; i++)
            {
                var v = nodeList[i];
                if (v.Location.X < minX)
                    minX = v.Location.X;
                if (v.Location.Y < minY)
                    minY = v.Location.Y;

                if (v.Location.X > maxX)
                    maxX = v.Location.X;
                if (v.Location.Y > maxY)
                    maxY = v.Location.Y;
            }

            log.Info("Min location: " + minX + " " + minY);

           // var xOffset = (availableWidth - Math.Abs(maxX - minX)) / 2 + 100;
           // var yOffset = (availableHeight - Math.Abs(maxY - minY)) / 2 + 100;

            var xOffset = 0 - minX + 100;
            var yOffset = 0 - minY + 50 ;

            for (int i = 0; i < n; i++) // set new positions
            {
                var v = nodeList[i];
                v.Location = new System.Drawing.PointF((float)(v.Location.X + xOffset), (float)(v.Location.Y + yOffset));

            }
        }

        internal void ClearHighlight(Color color)
        {
            foreach(var node in nodes.Values)
            {
                node.View.SetDefaultColor();
            }
        }

        internal void HighlightNeighbors(Guid nodeId, Color color)
        {
            coloredEdges.Clear();
            foreach(var e in edges.Values)
            {
                if (e.EndPoint.Id == nodeId)
                {
                    e.StartPoint.View.NodeColor = color;
                    coloredEdges.Add(e.Id);
                }
            }
        }

        internal void SetNodeColor(Guid nodeId, Color nodeColor)
        {
            nodes[nodeId].View.NodeColor = nodeColor;
        }

        public bool ShouldBeColored(Guid edgeId)
        {
            return coloredEdges.Contains(edgeId);
        }

        public Guid GetNodeIdByName(string name)
        {
            var id = (from n in nodes where n.Value.Text == name select n.Key).FirstOrDefault();
            return id;
        }

        public NodeModel GetNodeModelByName(string name)
        {
            var model = (from n in nodes where n.Value.Text == name select n.Value).FirstOrDefault();
            return model;
        }
    }
}
