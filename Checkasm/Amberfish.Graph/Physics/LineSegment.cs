using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Amberfish.Graph.Physics
{
    class LineSegment : Line
    {
        Point PointA { get; set; }
        Point PointB { get; set; }

        public LineSegment() : base() { }

        public LineSegment(Point a, Point b) : base(a.X, a.Y, b.X, b.Y)
        {
            PointA = a;
            PointB = b;
        }

        public override bool IntersectsWith(Line s)
        {
            var intersection = base.GetIntersection(s);
            if (intersection == null)
                return false;

            if (PointA.X <= PointB.X)
            {
                if (PointA.Y <= PointB.Y)
                {
                    return intersection.Value.X >= PointA.X && intersection.Value.X <= PointB.X && intersection.Value.Y >= PointA.Y && intersection.Value.Y <= PointB.Y;
                }
                else
                {
                    return intersection.Value.X >= PointA.X && intersection.Value.X <= PointB.X && intersection.Value.Y <= PointA.Y && intersection.Value.Y >= PointB.Y;
                }
            }
            else
            {
                if (PointA.Y <= PointB.Y)
                {
                    return intersection.Value.X <= PointA.X && intersection.Value.X >= PointB.X && intersection.Value.Y >= PointA.Y && intersection.Value.Y <= PointB.Y;
                }
                else
                {
                    return intersection.Value.X <= PointA.X && intersection.Value.X >= PointB.X && intersection.Value.Y <= PointA.Y && intersection.Value.Y >= PointB.Y;
                }
            }
        }
        public override string ToString()
        {
            return base.ToString() + string.Format(";A[{0},{1}] B[{2},{3}]", PointA.X, PointA.Y, PointB.X, PointB.Y);
        }
    }
}
