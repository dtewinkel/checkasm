using Amberfish.Graph.Models;
using Amberfish.Graph.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Amberfish.Graph
{
    /// <summary>
    /// Interaction logic for ConnectedGraph.xaml
    /// </summary>
    public partial class ConnectedGraph : UserControl
    {
        GraphViewModel viewModel = new GraphViewModel(); //view model for this window

        public ConnectedGraph()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var nodeViewModel = (NodeViewModel)((FrameworkElement)e.OriginalSource).DataContext;
            viewModel.SelectNode(nodeViewModel);
        }

        /// <summary>
        /// Handles dragging of a node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var node = (NodeViewModel)thumb.DataContext;
            node.X += e.HorizontalChange;
            node.Y += e.VerticalChange;
        }

        public void AddNode(double x, double y, INodeModel model)
        {
            viewModel.PlaceNewNode(x, y, model);
        }

        /// <summary>
        /// Connects two nodes by an edge
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ConnectNodes(int sourceId, int targetId)
        {
            viewModel.CreateEdge(sourceId, targetId);

            viewModel.SelectNode(null);
        }
        public void LayoutCircular()
        {
            viewModel.LayoutCircular();
        }




        private void ArrowLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var edge = (EdgeViewModel)((FrameworkElement)sender).DataContext;
            viewModel.SelectEdge(edge);
            e.Handled = true;

        }
    }
}
