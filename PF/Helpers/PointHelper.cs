using System.Numerics;
using System.Windows;

namespace PF.Helpers
{
    public static class PointHelper
    {
        public static Vector2 ToVector2(this Point point)
        {
            return new((float)point.X, (float)point.Y);
        }
    }
}
