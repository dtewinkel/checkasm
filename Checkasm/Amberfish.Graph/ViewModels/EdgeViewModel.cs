using System.ComponentModel;
using System.Diagnostics;

namespace Amberfish.Graph.ViewModels
{
    class EdgeViewModel:INotifyPropertyChanged
    {
        bool isSelected;
        NodeViewModel source;
        NodeViewModel destination;
        double x1;
        double x2;
        double y1;
        double y2;

        /// <summary>
        /// Raised when a property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the source node of the edge - the node where the edge starts.
        /// </summary>
        public NodeViewModel Source
        {
            get { return source; }
            set
            {
                if (value == null && source != null)
                {
                    source.PropertyChanged -= Source_PropertyChanged;
                }
                source = value;
                if(source != null)
                {
                    RecalculateConnectors();

                    source.PropertyChanged += Source_PropertyChanged;
                }
                OnPropertyChanged("Source");
            }
        }

        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Y":
                case "X":
                    RecalculateConnectors();
                    break;
                default:
                    break;
            }
        }

        private void Destination_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Y":
                case "X":
                    RecalculateConnectors();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the destination node of the edge - the node where the edge ends.
        /// </summary>
        public NodeViewModel Destination
        {
            get { return destination; }
            set
            {
                if (value == null && destination != null)
                {
                    destination.PropertyChanged -= Destination_PropertyChanged;
                }
                destination = value;
                if (destination != null)
                {
                    RecalculateConnectors();
                    destination.PropertyChanged += Destination_PropertyChanged;
                }
                OnPropertyChanged("Destination");
            }
        }

        public double X1
        {
            get
            {
                return x1;
            }
        }

        public double Y1
        {
            get
            {
                return y1;
            }
        }

        public double X2
        {
            get
            {
                return x2;
            }
        }

        public double Y2
        {
            get
            {
                return y2;
            }
        }

        /// <summary>
        /// Gets or sets the value specifying whether the edge is selected (highlighted by user).
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

        private void OnPropertyChanged(string name)
        {
            
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void RecalculateConnectors()
        {
            if (source != null && destination != null)
            {
                var newConnector = Source.GetOptimalConnector(destination.CenterX, destination.CenterY);
                x1 = newConnector.Item1;
                y1 = newConnector.Item2;
                OnPropertyChanged("Y1");
                OnPropertyChanged("X1"); 
           
                newConnector = Destination.GetOptimalConnector(source.CenterX, source.CenterY);
                x2 = newConnector.Item1;
                y2 = newConnector.Item2;
                OnPropertyChanged("Y2");
                OnPropertyChanged("X2");
            }
        }
    }
}
