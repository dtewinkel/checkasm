using Amberfish.Graph.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using phys = Amberfish.Graph.Physics;

namespace Amberfish.Graph.ViewModels
{
    class GraphViewModel: INotifyPropertyChanged
    {
        readonly ObservableCollection<EdgeViewModel> edges = new ObservableCollection<EdgeViewModel>();
        readonly ObservableCollection<NodeViewModel> nodes = new ObservableCollection<NodeViewModel>();
        Color activeColor;
        NodeViewModel selectedNode;
        EdgeViewModel selectedEdge;
        INodeModel modelTemplate;

        /// <summary>
        /// Raised when a property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the list of nodes in the graph
        /// </summary>
        public ObservableCollection<NodeViewModel> Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// Gets or sets the list of edges in the graph
        /// </summary>
        public ObservableCollection<EdgeViewModel> Edges
        {
            get { return edges; }
        }

        /// <summary>
        /// Specifies the color that is used for newly created nodes.
        /// </summary>
        public Color ActiveColor
        {
            get { return activeColor; }
            set
            {
                activeColor = value;
                OnPropertyChanged("ActiveColor");
            }
        }

        /// <summary>
        /// Gets or sets the color of the selected node
        /// </summary>
        public Color SelectedNodeColor
        {
            get
            {
                if (selectedNode != null)
                    return selectedNode.Color;
                else
                {
                    return Colors.Transparent;
                }
            }
            set
            {
                if (selectedNode != null)
                {
                    selectedNode.Color = value;
                    OnPropertyChanged("SelectedNodeColor");
                }
            }
        }

        internal void LayoutCircular()
        {
            var nodesToLayout = new Dictionary<int, NodeViewModel>();
            var layoutLevelValue = new Dictionary<int, int>();
            foreach (var edge in edges) //go through all edges and collect list of nodes, calculate level
            {
                if (!nodesToLayout.ContainsKey(edge.Source.Id))
                {
                    nodesToLayout.Add(edge.Source.Id, edge.Source);
                    layoutLevelValue.Add(edge.Source.Id, 1);
                }
                if (!nodesToLayout.ContainsKey(edge.Destination.Id))
                {
                    nodesToLayout.Add(edge.Destination.Id, edge.Destination);
                    layoutLevelValue.Add(edge.Destination.Id, 0);
                }
            }

            var layoutList = nodesToLayout.Values.ToList();

            layoutList.Sort(new Comparison<NodeViewModel>((a, b) => { return layoutLevelValue[a.Id].CompareTo(layoutLevelValue[b.Id]); }));

            var angle = (2 * Math.PI) / layoutList.Count;
            var radius = GetRadius(layoutList.Count);

            var currentAngle = 0.0;
            foreach (var node in layoutList)
            {
                var position = GetNodePosition(new Point(0, 0), radius, currentAngle);
                node.X = position.X;
                node.Y = position.Y;
                currentAngle += angle;
            }

            //optimize intersections
            var currentBest = CalculateIntersections();
            if (currentBest > 0)
            {
                for (int i = 0; i < layoutList.Count; i++)
                {
                    for (int j = 1; j < layoutList.Count; j++)
                    {
                        if (layoutList[i].Id != layoutList[j].Id)
                        {
                            ExchangeNodesLocation(layoutList[i], layoutList[j]);
                            var intersections = CalculateIntersections();
                            if (intersections <= currentBest)
                            {
                                currentBest = intersections;
                            }
                            else
                            {
                                ExchangeNodesLocation(layoutList[i], layoutList[j]);
                            }
                        }
                    }
                }
            }
        }

        private void ExchangeNodesLocation(NodeViewModel nodeModel1, NodeViewModel nodeModel2)
        {
            var tmpx = nodeModel1.X;
            var tmpy = nodeModel1.Y;
            nodeModel1.X = nodeModel2.X;
            nodeModel1.Y = nodeModel2.Y;
            nodeModel2.X = tmpx;
            nodeModel2.Y = tmpy;
        }

        private int CalculateIntersections()
        {
            List<phys.LineSegment> segments = new List<phys.LineSegment>();
            foreach (var edge in edges)
            {
                segments.Add(new phys.LineSegment(new Point(edge.Source.X, edge.Source.Y), new Point(edge.Destination.X, edge.Destination.Y)));
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



        private Point GetNodePosition(Point center, double radius, double angle)
        {
            var x = radius * Math.Cos(angle);
            var y = radius * Math.Sin(angle);
            return new Point((float)x, (float)y);
        }


        private float GetRadius(int count)
        {
            var s = 150.0;
            return (float)((s * count) / (2 * Math.PI));
        }


        internal void CreateEdge(int sourceId, int targetId)
        {
            var source = GetNodeById(sourceId);
            var target = GetNodeById(targetId);
            if(ValidateNewConnection(source, target))
            {
                Edges.Add(new EdgeViewModel { Source = source, Destination = target });
            }
        }

        internal bool ValidateNewConnection(NodeViewModel source, NodeViewModel target)
        {
            return source.CanConnectTo(target) && target.CanConnectTo(source);
        }

        /// <summary>
        /// Gets the value specifying whether there is anything selected (highlighted) by the user.
        /// </summary>
        /// <returns></returns>
        public bool IsAnythingSelected
        {
            get
            {
                return selectedEdge != null || IsAnyNodeSelected;
            }
        }

        /// <summary>
        /// Gets the value specifying whether there is any node selected (highlighted) by the user.
        /// </summary>
        public bool IsAnyNodeSelected
        {
            get
            {
                return selectedNode != null;
            }
        }

        internal INodeModel ModelTemplate
        {
            get
            {
                return modelTemplate;
            }

            set
            {
                modelTemplate = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public GraphViewModel()
        {
            ActiveColor = Colors.PaleGoldenrod;
        }

        /// <summary>
        /// Adds a new node
        /// </summary>
        public void PlaceNewNode(double x, double y)
        {
            var node = new NodeViewModel((INodeModel)ModelTemplate.Clone())
            {
                X = x,
                Y = y,
            };
            node.ConnectionsChanged += Node_ConnectionsChanged;
            node.Color = ActiveColor;
            Nodes.Add(node);

            SelectNode(null);
        }

        /// <summary>
        /// Adds a new node
        /// </summary>
        public void PlaceNewNode(double x, double y, INodeModel model)
        {
            var node = new NodeViewModel(model)
            {
                X = x,
                Y = y,
            };
            node.ConnectionsChanged += Node_ConnectionsChanged;
            node.Color = ActiveColor;
            Nodes.Add(node);

            SelectNode(null);
        }

        /// <summary>
        /// Highlights a node
        /// </summary>
        /// <param name="node"></param>
        public void SelectNode(NodeViewModel node)
        {
            if (selectedNode != null)
                selectedNode.IsSelected = false;

            if (selectedEdge != null)
            {
                selectedEdge.IsSelected = false;
                selectedEdge = null;
            }

            if (node == null)
            {
                selectedNode = null;
            }
            else
            {
                node.IsSelected = true;
                selectedNode = node;
            }
            OnPropertyChanged("IsAnythingSelected");
            OnPropertyChanged("IsAnyNodeSelected");
            OnPropertyChanged("SelectedNodeColor");
        }

        /// <summary>
        /// Highlights an edge
        /// </summary>
        /// <param name="edge"></param>
        public void SelectEdge(EdgeViewModel edge)
        {
            if (selectedNode != null)
            {
                selectedNode.IsSelected = false;
                selectedNode = null;
            }

            if (selectedEdge != null)
                selectedEdge.IsSelected = false;

            if (edge != null)
            {
                edge.IsSelected = true;
                selectedEdge = edge;
            }
        }

        /// <summary>
        /// Deletes everything
        /// </summary>
        public void Clear()
        {
            Nodes.Clear();
            Edges.Clear();
            selectedNode = null;
        }

        /// <summary>
        /// Deletes selected (highlighted) objects
        /// </summary>
        public void DeleteSelectedObjects()
        {
            if (selectedNode != null)
            {
                var toRemove = new List<EdgeViewModel>();
                foreach (var edge in Edges)
                {
                    if (edge.Destination.Equals(selectedNode) || edge.Source.Equals(selectedNode))
                    {
                        toRemove.Add(edge);
                    }
                }
                toRemove.ForEach(edge => Edges.Remove(edge));
                Nodes.Remove(selectedNode);
                SelectNode(null);
            }
            if (selectedEdge != null)
            {
                Edges.Remove(selectedEdge);
                selectedEdge = null;
            }
        }

        private void Node_ConnectionsChanged(object sender, EventArgs e)
        {
            var model = (NodeViewModel)sender;
            var remove = new List<EdgeViewModel>();
            foreach (var edge in edges)
            {
                if (edge.Destination == model)
                {
                    remove.Add(edge);
                }
            }
            remove.ForEach(edge => edges.Remove(edge));
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private NodeViewModel GetNodeById(int id)
        {
            return nodes.First(n => n.Id == id);
        }
    }
}
