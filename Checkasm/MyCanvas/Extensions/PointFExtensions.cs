using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Amberfish.Canvas.Extensions
{
    static class PointFExtensions
    {
        public static Point ToPoint(this PointF pointF)
        {
            return new Point((int)pointF.X, (int)pointF.Y);
        }

        public static PointF ToPoint(this Point point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }
    }
}
