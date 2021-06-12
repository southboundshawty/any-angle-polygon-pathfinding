using PF.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PF.Helpers
{
    public static class AreaHelper
    {
        public static List<Point> GetScaledVertexes(this Area area, double scale = 1)
        {
            List<Point> shapePoints = area.ShapePoints
                .Select(areaPoint => areaPoint.Position).ToList();

            List<Point> result = new();

            foreach (Point shapePoint in shapePoints)
            {
                double totalX = shapePoints.Sum(p => p.X);
                double totalY = shapePoints.Sum(p => p.Y);

                double centerX = totalX / shapePoints.Count;
                double centerY = totalY / shapePoints.Count;

                Point point =
                    new(centerX + Math.Sqrt(scale) * (shapePoint.X - centerX), centerY +
                                                                               Math.Sqrt(scale) *
                                                                               (shapePoint.Y - centerY));

                result.Add(point);
            }

            return result;
        }
    }
}
