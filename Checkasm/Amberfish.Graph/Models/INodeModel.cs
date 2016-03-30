using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Amberfish.Graph.Models
{
    public interface INodeModel:ICloneable
    {
        event EventHandler ColorChanged;
        event EventHandler RefreshRequired;
        event EventHandler ConnectionsChanged;
        event EventHandler TextChanged;

        string Text { get; set; }
        Color Color { get; set; }

        int Id { get; }

        bool CanConnectTo(INodeModel node);
    }
}
