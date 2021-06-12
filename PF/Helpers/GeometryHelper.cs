using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;

namespace PF.Helpers
{
    public static class GeometryHelper
    {
        public static Point lineIntersect(Point a1, Point a2, Point b1, Point b2)
        {
            double dx, dy, da, db, t, s;

            dx = a2.X - a1.X;
            dy = a2.Y - a1.Y;
            da = b2.X - b1.X;
            db = b2.Y - b1.Y;

            if (da * dy - db * dx == 0)
            {
                // The segments are parallel.
                return default(Point);
            }

            s = (dx * (b1.Y - a1.Y) + dy * (a1.X - b1.X)) / (da * dy - db * dx);
            t = (da * (a1.Y - b1.Y) + db * (b1.X - a1.X)) / (db * dx - da * dy);

            if ((s >= 0) & (s <= 1) & (t >= 0) & (t <= 1))
                return new Point((int)(a1.X + t * dx), (int)(a1.Y + t * dy));
            else
                return default(Point);
        }

        public static Point FindIntersection(Point p1, Point p2, Point p3, Point p4, double tolerance = 0.00000000000000000001)
        {
            double x1 = p1.X, y1 = p1.Y;
            double x2 = p2.X, y2 = p2.Y;

            double x3 = p3.X, y3 = p3.Y;
            double x4 = p4.X, y4 = p4.Y;

            // equations of the form x = c (two vertical lines)
            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance)
            {
                throw new Exception("Both lines overlap vertically, ambiguous intersection points.");
            }

            //equations of the form y=c (two horizontal lines)
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance)
            {
                throw new Exception("Both lines overlap horizontally, ambiguous intersection points.");
            }

            //equations of the form x=c (two vertical lines)
            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance)
            {
                return default(Point);
            }

            //equations of the form y=c (two horizontal lines)
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance)
            {
                return default(Point);
            }

            //general equation of line is y = mx + c where m is the slope
            //assume equation of line 1 as y1 = m1x1 + c1 
            //=> -m1x1 + y1 = c1 ----(1)
            //assume equation of line 2 as y2 = m2x2 + c2
            //=> -m2x2 + y2 = c2 -----(2)
            //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection point
            //so we will get below two equations 
            //-m1x + y = c1 --------(3)
            //-m2x + y = c2 --------(4)

            double x, y;

            //lineA is vertical x1 = x2
            //slope will be infinity
            //so lets derive another solution
            if (Math.Abs(x1 - x2) < tolerance)
            {
                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x1=c1=x
                //subsitute x=x1 in (4) => -m2x1 + y = c2
                // => y = c2 + m2x1 
                x = x1;
                y = c2 + m2 * x1;
            }
            //lineB is vertical x3 = x4
            //slope will be infinity
            //so lets derive another solution
            else if (Math.Abs(x3 - x4) < tolerance)
            {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x3=c3=x
                //subsitute x=x3 in (3) => -m1x3 + y = c1
                // => y = c1 + m1x3 
                x = x3;
                y = c1 + m1 * x3;
            }
            //lineA & lineB are not vertical 
            //(could be horizontal we can handle it with slope = 0)
            else
            {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
                //plugging x value in equation (4) => y = c2 + m2 * x
                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;

                //verify by plugging intersection point (x, y)
                //in orginal equations (1) & (2) to see if they intersect
                //otherwise x,y values will not be finite and will fail this check
                if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                    && Math.Abs(-m2 * x + y - c2) < tolerance))
                {
                    return default(Point);
                }
            }

            //x,y can intersect outside the line segment since line is infinitely long
            //so finally check if x, y is within both the line segments
            //if (IsInsideLine(p1, p2, x, y) &&
            //    IsInsideLine(p3, p4, x, y))
            //{
            //    return new Point { X = x, Y = y };
            //}

            //return default null (no intersection)
            return default(Point);

        }

        private static bool IsInsideLine(Point p1, Point p2, double x, double y)
        {
            return (x >= p1.X && x <= p2.X
                    || x >= p2.X && x <= p1.X)
                   && (y >= p2.Y && y <= p2.Y
                       || y >= p2.Y && y <= p2.Y);
        }

        //public static bool FindIntersection(
        //    Point p1, Point p2, Point p3, Point p4)
        //{
        //    // Get the segments' parameters.
        //    double dx12 = p2.X - p1.X;
        //    double dy12 = p2.Y - p1.Y;
        //    double dx34 = p4.X - p3.X;
        //    double dy34 = p4.Y - p3.Y;

        //    // Solve for t1 and t2
        //    double denominator = dy12 * dx34 - dx12 * dy34;

        //    double t1 =
        //        ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
        //        / denominator;
        //    if (double.IsInfinity(t1))
        //        // The lines are parallel (or close enough to it). 
        //        return false;
        //    return true;
        //}

        public static bool LineSegmentsCross(this Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float denominator = (b.X - a.X) * (d.Y - c.Y) - (b.Y - a.Y) * (d.X - c.X);

            if (denominator == 0) return false;

            float numerator1 = (a.Y - c.Y) * (d.X - c.X) - (a.X - c.X) * (d.Y - c.Y);

            float numerator2 = (a.Y - c.Y) * (b.X - a.X) - (a.X - c.X) * (b.Y - a.Y);

            if (numerator1 == 0 || numerator2 == 0) return false;

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return r is > 0 and < 1 && s is > 0 and < 1;
        }

        public static bool IsVertexConcave(this IList<Vector2> vertices, int vertex)
        {
            Vector2 current = vertices[vertex];
            Vector2 next = vertices[(vertex + 1) % vertices.Count];
            Vector2 previous = vertices[vertex == 0 ? vertices.Count - 1 : vertex - 1];

            Vector2 left = new(current.X - previous.X, current.Y - previous.Y);
            Vector2 right = new(next.X - current.X, next.Y - current.Y);

            float cross = left.X * right.Y - left.Y * right.X;

            return cross < 0;
        }

        public static bool Inside(this IList<Vector2> polygon, Vector2 position, bool toleranceOnOutside = true)
        {
            Vector2 point = position;

            const float epsilon = 0.5f;

            bool inside = false;

            // Must have 3 or more edges
            if (polygon.Count < 3) return false;

            Vector2 oldPoint = polygon[^1];
            float oldSqDist = Vector2.DistanceSquared(oldPoint, point);

            foreach (Vector2 newPoint in polygon)
            {
                float newSqDist = Vector2.DistanceSquared(newPoint, point);

                if (oldSqDist + newSqDist + 2.0f * Math.Sqrt(oldSqDist * newSqDist) -
                    Vector2.DistanceSquared(newPoint, oldPoint) < epsilon)
                    return toleranceOnOutside;

                Vector2 left;
                Vector2 right;

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

                if (left.X < point.X && point.X <= right.X && (point.Y - left.Y) * (right.X - left.X) <
                    (right.Y - left.Y) * (point.X - left.X))
                    inside = !inside;

                oldPoint = newPoint;
                oldSqDist = newSqDist;
            }

            return inside;
        }

        public static bool InLineOfSight(this List<Vector2> polygon, Vector2 start, Vector2 end)
        {
            const float epsilon = float.MinValue;

            if (polygon.Inside(start) || polygon.Inside(end)) return false;

            if (Vector2.Distance(start, end) < epsilon) return false;

            for (int i = 0; i < polygon.Count - 1; i++)
                if (LineSegmentsCross(start, end, polygon[i], polygon[i + 1]))
                    return false;

            return polygon.Inside((start + end) / 2f);
        }
    }
}
