using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Amberfish.Graph.Models
{
    public class NodeModelBase : INodeModel
    {
        private Color color;
        private string text;
        private static int id;
        
        public int Id { get; private set; }

        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
                OnColorChanged();
                OnRefreshRequired();
            }
        }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                OnTextChanged();
                OnRefreshRequired();
            }
        }

        public event EventHandler ColorChanged;
        public event EventHandler ConnectionsChanged;
        public event EventHandler RefreshRequired;
        public event EventHandler TextChanged;

        public NodeModelBase()
        {
            Id = id++;
            text = id.ToString();
            color = Colors.White;
        }
        public object Clone()
        {
            return CreateClone();
        }

        protected virtual NodeModelBase CreateClone()
        {
            var clone = new NodeModelBase();
            clone.color = color;
            clone.text = text;
            return clone;
        }

        protected virtual void OnColorChanged()
        {
            var handler = ColorChanged;
            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnConnectionsChanged()
        {
            var handler = ConnectionsChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnRefreshRequired()
        {
            var handler = RefreshRequired;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        protected virtual void OnTextChanged()
        {
            var handler = TextChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public virtual bool CanConnectTo(INodeModel node)
        {
            return true;
        }
        
    }
}
