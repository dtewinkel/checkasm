using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Amberfish.Graph.Physics
{
    /// <summary>
    /// Line represented by Ax + By + C = 0
    /// </summary>
    class Line
    {
        private Point point1;
        private Point point2;

        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public Line() { }

        /// <summary>
        /// Creates a line based on a,b,c
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Line(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Creates a line from points
        /// </summary>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        public Line(double ax, double ay, double bx, double by)
        {
            A = by - ay;
            B = ax - bx;
            C = bx * ay - by * ax;
        }

        public Line(Point point1, Point point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public virtual bool IntersectsWith(Line s)
        {
            return GetIntersection(s) != null;
        }

        public virtual Point? GetIntersection(Line s)
        {
            try
            {
                double Y = (((s.A * C) / A) - s.C) / (s.B - (s.A * B / A));
                double X = (-1 * C - B * Y) / A;
                return new Point(X, Y);
            }
            catch (DivideByZeroException)
            {
                return null;
            }

        }

        public override string ToString()
        {
            return string.Format("{0}x + {1}y + {2} = 0", A, B, C);
        }
    }
}
