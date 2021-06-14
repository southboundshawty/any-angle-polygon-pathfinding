using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;

namespace PF.Helpers
{
    public static class GeometryHelper
    {
        public static bool LinePolygonCross(this IList<Point> polygon, Point a, Point b)
        {
            bool intersect = false;

            for (int k = 0; k < polygon.Count; k++)
            {
                Point p1 = polygon[k];
                Point p2 = polygon[(k + 1) % polygon.Count];

                Point sideIntersection = LineIntersect(a, b, p1, p2);

                if (sideIntersection == default && !polygon.Inside(a) && !polygon.Inside(b)) 
                    continue;

                intersect = true;

                break;
            }

            return intersect;
        }

        public static double DistanceSquared(this Point p1, Point p2)
        {
            return Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2);
        }

        public static Point LineIntersect(Point a1, Point a2, Point b1, Point b2)
        {
            double dx = a2.X - a1.X;
            double dy = a2.Y - a1.Y;
            double da = b2.X - b1.X;
            double db = b2.Y - b1.Y;

            if (da * dy - db * dx == 0)
            {
                return default;
            }

            double s = (dx * (b1.Y - a1.Y) + dy * (a1.X - b1.X)) / (da * dy - db * dx);
            double t = (da * (a1.Y - b1.Y) + db * (b1.X - a1.X)) / (db * dx - da * dy);

            if ((s >= 0) & (s <= 1) & (t >= 0) & (t <= 1))
                return new Point((int)(a1.X + t * dx), (int)(a1.Y + t * dy));

            return default;
        }

        public static bool Inside(this IList<Point> polygon, Point position, bool toleranceOnOutside = false)
        {
            Point point = position;

            const float epsilon = float.MinValue;

            bool inside = false;

            if (polygon.Count() < 3) 
                return false;

            Point oldPoint = polygon[^1];

            double oldSqDist = oldPoint.DistanceSquared(point);

            foreach (Point newPoint in polygon)
            {
                double newSqDist = newPoint.DistanceSquared(point);

                if (oldSqDist + newSqDist + 2.0f * Math.Sqrt(oldSqDist * newSqDist) - newPoint.DistanceSquared(oldPoint) < epsilon)
                    return toleranceOnOutside;

                Point left;
                Point right;

                if (newPoint.X > oldPoint.X)
                {
                    left = oldPoint;
                    right = newPoint;
                }
                else
                {
                    left = newPoint;
                    right = oldPoint;
                }

                if (left.X < point.X && point.X <= right.X && (point.Y - left.Y) * (right.X - left.X) < (right.Y - left.Y) * (point.X - left.X))
                    inside = !inside;

                oldPoint = newPoint;
                oldSqDist = newSqDist;
            }

            return inside;
        }
    }
}
