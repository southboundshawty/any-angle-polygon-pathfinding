using PF.Helpers;
using PF.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            //List<List<Point>> scaledAreaVertexes = areas
            //    .Select(area => area.GetScaledVertexes(1.8)).ToList();

            List<Point> allWayPoints =
                areas
                    .SelectMany(a => a.Points).ToList();

            allWayPoints.Add(goal);

            List<(PathNode point, double distance)> pathNodes = new();

            foreach (Point wayPoint in allWayPoints)
            {
                if (wayPoint == pathNode.Position)
                    continue;

                bool intersect = false;

                foreach (Area area in areas)
                {
                    List<ShapePoint> shapePoints = area.ShapePoints.ToList();

                    for (int k = 0; k < shapePoints.Count; k++)
                    {
                        Point p1 = shapePoints[k].Position;
                        Point p2 = shapePoints[(k + 1) % shapePoints.Count].Position;

                        Point sideIntersection = GeometryHelper.lineIntersect(pathNode.Position, wayPoint, p1, p2);

                        if (sideIntersection != default(Point))
                        {
                            var a = $"{pathNode.Position} and {wayPoint} intersects: {p1} and {p2} \nat {sideIntersection}";

                           // MessageBox.Show(a);

                           if (sideIntersection != pathNode.Position &&
                               sideIntersection != wayPoint &&
                               sideIntersection != p1 &&
                               sideIntersection != p2)
                           {
                               intersect = true;

                               break;
                            }
                        }
                        //else
                        //{
                        //    var a = $"{pathNode.Position} and {wayPoint} NOT: {p1} and {p2} \nat {sideIntersection}";

                        //    MessageBox.Show(a);
                        //}
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

                pathNodes.Add((neighbourNode, GetDistanceBetweenNeighbours(pathNode.Position, neighbourNode.Position)));
            }

            //int i = 4;

            //do
            //{
                result = pathNodes
                    .OrderBy(i => i.distance)
                    .ThenBy(p => p.point.HeuristicEstimatePathLength)
                    .Select(p => p.point)
                    .ToList();

            //    i++;
            //} while (result.All(p => p.EstimateFullPathLength > pathNode.EstimateFullPathLength) &&
            //         i < pathNodes.Count);

            return result;
        }

        public List<Point> FindPath(List<Area> areas, Point start, Point goal)
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
                    node.HeuristicEstimatePathLength).First();

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
                    else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
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
