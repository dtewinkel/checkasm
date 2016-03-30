using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Amberfish.Canvas.Model;
using Amberfish.Canvas.Extensions;
using Amberfish.Canvas.Controllers;
using Amberfish.Canvas.Properties;
using System.Drawing.Drawing2D;

namespace Amberfish.Canvas.Views
{
    public partial class NodeView : UserControl
    {
        private NodeModel model;
        private Log log = new Log();

        public NodeController Controller { get; private set; }

        bool dragged = false;

        private readonly List<Color> backgroundColorSet = new List<Color>
            {
                Color.FromArgb(164,69,63),
                Color.FromArgb(67,112,72),
                Color.FromArgb(67,81,157),
                Color.FromArgb(159,106,67),
                Color.FromArgb(105,105,105),
            };

        private readonly List<Color> edgeColorSet = new List<Color>
            {
                Color.FromArgb(250,69,63),
                Color.FromArgb(67,180,72),
                Color.FromArgb(67,81,250),
                Color.FromArgb(180,180,67),
                Color.FromArgb(105,105,105),
            };
        private int colorIndex = 0;

        private Color nodeColor;
        public Color NodeColor
        {
            get
            {
                return nodeColor;
            }
            set
            {
                nodeColor = value;
                backgroundColorSet.Clear();
                edgeColorSet.Clear();
                BackColor = nodeColor; 
                model.EdgeColor = nodeColor;
            }
        }

        public NodeModel Model
        {
            get { return model; }
            set 
            {
                model = value;
                if (model != null)
                {
                    Controller = new NodeController(model);
                    model.View = this;
                    model.EdgeColor = this.BackColor;
                    model.LocationChanged += new EventHandler(model_LocationChanged);
                    model.TextChanged += new EventHandler(model_TextChanged);
                    model.DragStarted += new EventHandler(model_DragStarted);
                    model.DragFinished += new EventHandler(model_DragFinished);
                    model.IsShadowNodeChanged += model_IsShadowNodeChanged;

                    model.Connectors.Clear();

                    Model.Connectors.Add(new PointF(Width / 2, 0)); //up
                    Model.Connectors.Add(new PointF(Width, Height / 2)); //right
                    Model.Connectors.Add(new PointF(Width / 2, Height)); //bottom
                    Model.Connectors.Add(new PointF(0, Height / 2)); //left
                }
            }
        }

        void model_IsShadowNodeChanged(object sender, EventArgs e)
        {
            this.label1.ForeColor = model.IsShadowNode ? Color.Blue : Color.White;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (model.IsShadowNode)
            {
                var gr = e.Graphics;
                gr.FillRectangle(new HatchBrush(HatchStyle.LightUpwardDiagonal, Color.White, Color.FromArgb(252,207,208)), ClientRectangle);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        void model_DragFinished(object sender, EventArgs e)
        {
           //BackColor = Color.White;
        }

        void model_DragStarted(object sender, EventArgs e)
        {
            //BackColor = Color.Aqua;
        }

        void model_TextChanged(object sender, EventArgs e)
        {
            label1.Text = model.Text;
        }

        void model_LocationChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() =>
                {
                    Location = CalculateViewLocation(model.Location);
                }));
            }
            else
            {
                Location = CalculateViewLocation(model.Location);
            }
        }

        public NodeView()
        {
            InitializeComponent();
        }

        public void UpdateView()
        {
            log.Info("UpdateView");
            Location = CalculateViewLocation(model.Location);
            label1.Text = model.Text;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            log.Info("OnLoad");
            UpdateView();
        }

        private Point CalculateViewLocation(PointF modelLocation)
        {
            return (new PointF(modelLocation.X - Width / 2, modelLocation.Y - Height / 2)).ToPoint();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            log.Info("OnMouseDown");
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Controller.StartDrag();
            }
            BringToFront();
            dragged = false;
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                log.Info("OnMouseUp");
            }
            Controller.EndDrag();
            
            if (e.Button == System.Windows.Forms.MouseButtons.Left && 
                !dragged && 
                backgroundColorSet.Count > 0)
            {
                colorIndex++;
                if(colorIndex >= backgroundColorSet.Count)
                {
                    colorIndex = 0;
                }
                this.BackColor = backgroundColorSet[colorIndex];
                model.EdgeColor = edgeColorSet[colorIndex];
                nodeColor = BackColor;
            }
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            var loc = Parent.PointToClient(PointToScreen(e.Location));
            dragged = true;
            
            Controller.DragTo(loc);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            //this.BackgroundImage = Resources.BackgroundImage_Blue;
        }
        
        public void SetDefaultColor()
        {
            NodeColor = Color.FromArgb(105, 105, 105);
        }
    }
}
