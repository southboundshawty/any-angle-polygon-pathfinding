using PF.Helpers;
using PF.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PF.PathFinding.AStar
{
    public class AStarAlgorithm
    {
        private static double GetDistanceBetweenNeighbours(Point from, Point to)
        {
            return Point.Subtract(from, to).Length;
        }

        private static double GetHeuristicPathLength(Point from, Point to)
        {
            double deltaX = Math.Abs(from.X - to.X);
            double deltaY = Math.Abs(from.Y - to.Y);

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        private static List<PathNode> GetNeighbours(PathNode pathNode,
            Point goal, List<Area> areas)
        {
            List<PathNode> result = new();

            List<List<Point>> scaledAreaVertexes = areas
                .Select(area => area.GetScaledVertexes(2)).ToList();

            List<Point> allWayPoints =
                scaledAreaVertexes
                    .SelectMany(a => a).ToList();

            allWayPoints.Add(goal);
            
            foreach (Point wayPoint in allWayPoints)
            {
                if (wayPoint == pathNode.Position)
                    continue;

                bool intersect = false;

                foreach (Area area in areas)
                {
                    var shapePoints = area.ShapePoints.Select(p => p.Position).ToList();

                    bool haveIntersection = shapePoints.LinePolygonCross(pathNode.Position, wayPoint);

                    if (haveIntersection)
                    {
                        intersect = true;

                        break;
                    }
                }

                if (intersect)
                    continue;

                PathNode neighbourNode = new()
                {
                    Position = wayPoint,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart +
                                          GetDistanceBetweenNeighbours(pathNode.Position, wayPoint),
                    HeuristicEstimatePathLength = GetHeuristicPathLength(wayPoint, goal)
                };

                result.Add(neighbourNode);
            }

            return result;
        }

        public static List<Point> FindPath(List<Area> areas, Point start, Point goal)
        {
            Collection<PathNode> closedSet = new();
            Collection<PathNode> openSet = new();

            PathNode startNode = new()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
            };

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet.OrderBy(node =>
                    node.EstimateFullPathLength).First();

                if (currentNode.Position == goal)
                    return GetPathForNode(currentNode);

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                List<PathNode> neighbours = GetNeighbours(currentNode, goal, areas);

                foreach (PathNode neighbourNode in neighbours)
                {
                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                        continue;
                    PathNode openNode = openSet.FirstOrDefault(node =>
                        node.Position == neighbourNode.Position);

                    if (openNode == null)
                    {
                        openSet.Add(neighbourNode);
                    }
                    else if (openNode.EstimateFullPathLength > neighbourNode.EstimateFullPathLength)
                    {
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }

            return null;
        }

        private static List<Point> GetPathForNode(PathNode pathNode)
        {
            List<Point> result = new();
            PathNode currentNode = pathNode;

            while (currentNode != null)
            {
                result.Add(currentNode.Position);

                currentNode = currentNode.CameFrom;
            }

            result.Reverse();

            return result;
        }

        private class PathNode
        {
            public double PathLengthFromStart { get; set; }

            public double HeuristicEstimatePathLength { get; set; }

            public double EstimateFullPathLength => PathLengthFromStart + HeuristicEstimatePathLength;

            public PathNode CameFrom { get; set; }

            public Point Position { get; set; }
        }
    }
}
