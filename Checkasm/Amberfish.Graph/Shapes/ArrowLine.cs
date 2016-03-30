using Amberfish.Graph.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Amberfish.Graph.Shapes
{
    sealed class ArrowLine: Shape
    {/// <summary>
     /// X1 property
     /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary> 
        /// Gets or sets the x position of the start point
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double X1
        {
            get
            {
                return (double)GetValue(X1Property);
            }
            set
            {
                SetValue(X1Property, value);
            }
        }

        /// <summary> 
        /// Y1 property
        /// </summary> 
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary> 
        /// Gets or sets the y position of the start point
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double Y1
        {
            get
            {
                return (double)GetValue(Y1Property);
            }
            set
            {
                SetValue(Y1Property, value);
            }
        }

        /// <summary> 
        /// X2 property
        /// </summary> 
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary> 
        /// Gets or sets the x position of the end point
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double X2
        {
            get
            {
                return (double)GetValue(X2Property);
            }
            set
            {
                SetValue(X2Property, value);
            }
        }

        /// <summary>
        /// Y2 property
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary> 
        /// Gets or sets the y position of the end point
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double Y2
        {
            get
            {
                return (double)GetValue(Y2Property);
            }
            set
            {
                SetValue(Y2Property, value);
            }
        }

        /// <summary>
        /// ArrowLength property
        /// </summary>
        public static readonly DependencyProperty ArrowLengthProperty =
            DependencyProperty.Register("ArrowLength", typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary> 
        /// Gets or sets the "length" of the arrow at the end of the line.
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double ArrowLength
        {
            get
            {
                return (double)GetValue(ArrowLengthProperty);
            }
            set
            {
                SetValue(ArrowLengthProperty, value);
            }
        }

        /// <summary>
        /// ArrowWidth property
        /// </summary>
        public static readonly DependencyProperty ArrowWidthProperty =
            DependencyProperty.Register("ArrowWidth", typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary> 
        /// Gets or sets the "width" of the arrow at the end of the line.
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double ArrowWidth
        {
            get
            {
                return (double)GetValue(ArrowWidthProperty);
            }
            set
            {
                SetValue(ArrowWidthProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public ArrowLine()
        {
        }

        /// <summary> 
        /// Gets the line that defines this shape
        /// </summary> 
        protected override Geometry DefiningGeometry
        {
            get
            {
                //var b = Y2 - Y1;
                //var a = X2 - X1;
                //var beta = Math.Atan2(b, a);
                //var alpha = Math.PI / 2 - beta;

                //var t = NodeViewModel.DefaultHeight / 2;
                //var s = t * Math.Tan(alpha);
                //if(Math.Abs(s)>NodeViewModel.DefaultWidth)
                //{
                //    s = NodeViewModel.DefaultWidth / 2;
                //    t = Math.Tan(alpha) / s;
                //}

                var nx1 = X1;
                var ny1 = Y1;
                var nx2 = X2;
                var ny2 = Y2;

                double e = Math.Atan2(ny1 - ny2, nx1 - nx2);
                double sinE = Math.Sin(e);
                double cosE = Math.Cos(e);

                Point point1 = new Point(nx1, ny1);
                Point point2 = new Point(nx2, ny2);
                Point point3 = new Point(nx2 + (ArrowLength * cosE - ArrowWidth * sinE), ny2 + (ArrowLength * sinE + ArrowWidth * cosE));
                Point point4 = new Point(nx2 + (ArrowLength * cosE + ArrowWidth * sinE), ny2 - (ArrowWidth * cosE - ArrowLength * sinE));
                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(point1, true, true);
                    ctx.LineTo(point2, true, true);
                    ctx.LineTo(point3, true, true);
                    ctx.LineTo(point4, true, true);
                    ctx.LineTo(point2, true, true);
                }
                return geometry;
            }
        }
    }
}
