using System.Windows;

namespace PF.Helpers
{
    public static class GeometryHelper
    {
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
    }
}
