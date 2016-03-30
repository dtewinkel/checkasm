using Amberfish.Graph.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Amberfish.Graph.ViewModels
{
    class NodeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the default node height
        /// </summary>
        public static double DefaultHeight { get; private set; }

        /// <summary>
        /// Gets the default node width
        /// </summary>
        public static double DefaultWidth { get; private set; }
       
        /// <summary>
        /// this is for generating the default node name (text)
        /// </summary>
        public int Id
        {
            get { return model.Id; }
        }

        string text;
        double x;
        double y;
        double width;
        double height;
        Color color;
      

        INodeModel model;

        static List<Tuple<double, double>> connectors;

        bool isSelected;

        /// <summary>
        /// Raised when a property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when connections on this node change
        /// </summary>
        public event EventHandler ConnectionsChanged;

        /// <summary>
        /// Gets the node model holding the data about this node. Kind of. The line between view model and model is always a bit fuzzy.
        /// </summary>
        public INodeModel Model
        {
            get { return model; }
        }

        internal bool CanConnectTo(NodeViewModel source)
        {
            return model.CanConnectTo(source.Model);
        }
     
        /// <summary>
        /// Gets or sets the text displayed on the node
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }

        /// <summary>
        /// Gets or sets the node's visible color
        /// </summary>
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color == value)
                {
                    return;
                }
                color = value;
                OnPropertyChanged("Color");
                OnPropertyChanged("TextColor");
            }
        }

        /// <summary>
        /// Gets the human-readable name of the current color
        /// </summary>
        public string ColorName
        {
            get { return color.GetName(); }
        }


        /// <summary>
        /// Gets or sets the horizontal position of the node
        /// </summary>
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");

            }
        }

        /// <summary>
        /// Gets or sets the vertical position of the node
        /// </summary>
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");

            }
        }

        public double CenterX
        {
            get { return x + width / 2; }
        }

        public double CenterY
        {
            get { return y + height / 2; }
        }

        /// <summary>
        /// Gets or sets the width of the node
        /// </summary>
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                connectors = GetConnectors(Width, Height);
                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Gets or sets the height of the node
        /// </summary>
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                connectors = GetConnectors(Width, Height);
                OnPropertyChanged("Height");

            }
        }

        /// <summary>
        /// Gets or sets the value specifying whether the node has been selected (highlighted) by the user
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets the color of the node's text. 
        /// </summary>
        /// <remarks>Since this color basically depends on the actual Color property, change of the Color property notifies about the change of this property too.</remarks>
        public Color TextColor
        {
            get
            {
                return color.GetContrastColor();
            }
        }

        static NodeViewModel()
        {
            DefaultHeight = 50;
            DefaultWidth = 100;
            connectors = GetConnectors(DefaultWidth, DefaultHeight);
        }

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public NodeViewModel(INodeModel model)
        {
            this.model = model;
            Width = DefaultWidth;
            Height = DefaultHeight;
            text = model.Text;
            color = model.Color;
            model.ColorChanged += model_ColorChanged;
            model.RefreshRequired += model_RefreshRequired;
            model.ConnectionsChanged += model_ConnectionsChanged;
            model.TextChanged += Model_TextChanged;
        }

        public Tuple<double, double> GetOptimalConnector(double x, double y)
        {
            double minDistance;
            int minDistanceIndex;
            minDistance = double.MaxValue;
            minDistanceIndex = -1;
            for (int i = 0; i < connectors.Count; i++)
            {
                var distance = GetDistance(x, y, connectors[i].Item1+X, connectors[i].Item2+Y);
                if (distance <= minDistance)
                {
                    minDistance = distance;
                    minDistanceIndex = i;
                }
            }
            return new Tuple<double, double>(X + connectors[minDistanceIndex].Item1, Y + connectors[minDistanceIndex].Item2);
        }

        private double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        private static List<Tuple<double, double>> GetConnectors(double w, double h)
        {
            return new List<Tuple<double, double>>()
                {
                    
                   
                    new Tuple<double, double>(w/2,0),
                   
                    
                   
                    new Tuple<double, double>(w/2,h),
                   
                    new Tuple<double, double>(0,h/2),
                    
                  
                  
                    new Tuple<double, double>(w,h/2),
                    
                 
                };
        }

        private void Model_TextChanged(object sender, EventArgs e)
        {
            Text = model.Text;
        }

        void model_ConnectionsChanged(object sender, EventArgs e)
        {
            OnConnectionsChanged();
        }

        void model_RefreshRequired(object sender, EventArgs e)
        {
            Text = model.Text;
            Color = model.Color;
        }

        void model_ColorChanged(object sender, EventArgs e)
        {
            Color = model.Color;
        }

        private void OnConnectionsChanged()
        {
            var handler = ConnectionsChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
