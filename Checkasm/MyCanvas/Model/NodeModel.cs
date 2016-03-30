using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Amberfish.Canvas.Physics;
using Amberfish.Canvas.Views;
using System.Xml.Serialization;

namespace Amberfish.Canvas.Model
{
    public class NodeModel
    {
        Log log = new Log();

        PointF location;
        string text;
        private bool isDragging;

        public event EventHandler LocationChanged;
        public event EventHandler TextChanged;
        public event EventHandler DragStarted;
        public event EventHandler DragFinished;
        public event EventHandler EdgeColorChanged;
        public event EventHandler IsShadowNodeChanged;

        public bool RaiseEvents { get; set; }

        private bool isShadowNode;

        public bool IsShadowNode
        {
            get
            {
                return isShadowNode;
            }
            set
            {
                bool raise = value != isShadowNode;
                isShadowNode = value;
                if (raise)
                {
                    var handler = IsShadowNodeChanged;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            }
        }

        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlIgnore]
        public Vector Velocity { get; set; }
        [XmlIgnore]
        public Vector NetForce { get; set; }

        private readonly List<PointF> connectors = new List<PointF>();
        [XmlIgnore]
        public List<PointF> Connectors { get { return connectors; } }
        [XmlIgnore]
        public NodeView View { get; set; }
        [XmlIgnore]
        public int LayoutLevelValue { get; set; }

        private Color edgeColor;

        [XmlElement("edgeColor")]
        public string EdgeColorHtml
        {
            get { return ColorTranslator.ToHtml(edgeColor); }
            set { EdgeColor = ColorTranslator.FromHtml(value); }
        }

        [XmlIgnore]
        public Color EdgeColor
        {
            get { return edgeColor; }
            set
            {
                bool raise = value != edgeColor;
                edgeColor = value;
                if (raise)
                {
                    var handler = EdgeColorChanged;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            }
        }

        public NodeModel()
        {
            Id = Guid.NewGuid();
            NetForce = new Vector(0,0);
            Velocity = new Vector(0,0);
            LayoutLevelValue = 0;
            EdgeColor = Color.Black;
        }
        [XmlIgnore]
        public bool IsDragging
        {
            get { return isDragging; }
            set 
            {
                bool raise = RaiseEvents && isDragging != value;
                isDragging = value;
                if (raise)
                {
                    if (isDragging)
                        OnDragStarted();
                    else
                        OnDragFinished();
                }
                log.Info("IsDragging set: " + isDragging);
            }
        }

        [XmlElement("location")]
        public PointF Location 
        {
            get { return location; } 
            set 
            {
                var original = location;
                location = value;
                if (original.X != location.X || original.Y != location.Y)
                {
                    OnLocationChanged();
                }
                log.Info("Location set: " + location);
            } 
        }

        [XmlElement("text")]
        public string Text
        {
            get { return text; }
            set
            {
                bool raise = RaiseEvents && text == value;
                text = value;
                if (raise)
                {
                    OnTextChanged();
                }
                log.Info("Text set: " + text);
            }
        }


        protected void OnLocationChanged()
        {
            if (!RaiseEvents)
                return;
            log.Info("OnLocationChanged");
            var handler = LocationChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnTextChanged()
        {
            if (!RaiseEvents)
                return;
            log.Info("OnTextChanged");
            var handler = TextChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnDragStarted()
        {
            if (!RaiseEvents)
                return;
            log.Info("OnDragStarted");
            var handler = DragStarted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnDragFinished()
        {
            if (!RaiseEvents)
                return;
            log.Info("OnDragFinished");
            var handler = DragFinished;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
