using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amberfish.Canvas.Physics
{
    class LineSegment:Line
    {
        Tuple<double, double> PointA { get; set; }
        Tuple<double, double> PointB { get; set; }

        public LineSegment() : base() { }

        public LineSegment(Tuple<double, double> a, Tuple<double, double> b):base(a.Item1, a.Item2, b.Item1, b.Item2)
        {
            PointA = a;
            PointB = b;
        }

        public override bool IntersectsWith(Line s)
        {
            var intersection = base.GetIntersection(s);
            if (PointA.Item1 <= PointB.Item1)
            {
                if (PointA.Item2 <= PointB.Item2)
                {
                    return intersection.Item1 >= PointA.Item1 && intersection.Item1 <= PointB.Item1 && intersection.Item2 >= PointA.Item2 && intersection.Item2 <= PointB.Item2;
                }
                else
                {
                    return intersection.Item1 >= PointA.Item1 && intersection.Item1 <= PointB.Item1 && intersection.Item2 <= PointA.Item2 && intersection.Item2 >= PointB.Item2;
                }
            }
            else
            {
                if (PointA.Item2 <= PointB.Item2)
                {
                    return intersection.Item1 <= PointA.Item1 && intersection.Item1 >= PointB.Item1 && intersection.Item2 >= PointA.Item2 && intersection.Item2 <= PointB.Item2;
                }
                else
                {
                    return intersection.Item1 <= PointA.Item1 && intersection.Item1 >= PointB.Item1 && intersection.Item2 <= PointA.Item2 && intersection.Item2 >= PointB.Item2;
                }
            }
        }
        public override string ToString()
        {
            return base.ToString() + string.Format(";A[{0},{1}] B[{2},{3}]", PointA.Item1, PointA.Item2, PointB.Item1, PointB.Item2);
        }
    }
}
