using System;
using System.Collections.Generic;
using System.Linq;
using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using UnityEngine;

namespace Pathfinding._Scripts
{
    public static class Pathfinding
    {
        private static readonly Color PathColor = new Color(0.65f, 0.35f, 0.35f);
        private static readonly Color OpenColor = new Color(.4f, .6f, .4f);
        private static readonly Color ClosedColor = new Color(0.35f, 0.4f, 0.5f);

        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
        {
            var toSearch = new List<NodeBase>() { startNode };
            var processed = new List<NodeBase>();

            while (toSearch.Any())
            {
                var current = toSearch[0];
                foreach (var t in toSearch)
                    if (t.F < current.F || t.F == current.F && t.H < current.H) current = t;

                processed.Add(current);
                toSearch.Remove(current);

                current.SetColor(ClosedColor);

                if (current == targetNode)
                {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    var count = 100;
                    while (currentPathTile != startNode)
                    {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection;
                        count--;
                        if (count < 0) throw new Exception();
                    }

                    foreach (var tile in path) tile.SetColor(PathColor);
                    startNode.SetColor(PathColor);
                    return path;
                }

                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && t.tileUnit == null && !processed.Contains(t)))
                {
                    var inSearch = toSearch.Contains(neighbor);

                    var costToNeighbor = current.G + current.GetDistance(neighbor);

                    if (!inSearch || costToNeighbor < neighbor.G)
                    {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);

                        if (!inSearch)
                        {
                            neighbor.SetH(neighbor.GetDistance(targetNode));
                            toSearch.Add(neighbor);
                            neighbor.SetColor(OpenColor);
                        }
                    }
                }
            }
            return null;
        }

        public static List<NodeBase> MarkReachableNodes(NodeBase startNode, int maxCost)
        {
            var toSearch = new Queue<(NodeBase node, float currentCost)>();
            var processed = new HashSet<NodeBase>();
            var reachableNodes = new List<NodeBase>();

            toSearch.Enqueue((startNode, 0));
            processed.Add(startNode);

            while (toSearch.Any())
            {
                var (current, currentCost) = toSearch.Dequeue();

                if (currentCost > maxCost) continue;

                current.SetColor(Color.red);
                current._isInRange = true;
                reachableNodes.Add(current);

                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && t.tileUnit == null && !processed.Contains(t)))
                {
                    var costToNeighbor = currentCost + neighbor.tileWalkValue;
                    if (costToNeighbor <= maxCost)
                    {
                        processed.Add(neighbor);
                        toSearch.Enqueue((neighbor, costToNeighbor));
                    }
                }
            }

            return reachableNodes;
        }

        public static List<NodeBase> IsReachableNodes(NodeBase startNode, int maxCost)
        {
            var toSearch = new Queue<(NodeBase node, float currentCost)>();
            var processed = new HashSet<NodeBase>();
            var reachableNodes = new List<NodeBase>();

            toSearch.Enqueue((startNode, 0));
            processed.Add(startNode);

            while (toSearch.Any())
            {
                var (current, currentCost) = toSearch.Dequeue();

                if (currentCost > maxCost) continue;

                reachableNodes.Add(current);

                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && t.tileUnit == null && !processed.Contains(t)))
                {
                    var costToNeighbor = currentCost + neighbor.tileWalkValue;
                    if (costToNeighbor <= maxCost)
                    {
                        processed.Add(neighbor);
                        toSearch.Enqueue((neighbor, costToNeighbor));
                    }
                }
            }

            return reachableNodes;
        }

        public static void MarkReachableNodesInFourDirections(NodeBase startNode, int maxSteps)
        {
            var directions = new List<Vector2>
            {
                new Vector2(0, 1),  // Up
                new Vector2(0, -1), // Down
                new Vector2(1, 0),  // Right
                new Vector2(-1, 0)  // Left
            };

            var processed = new HashSet<NodeBase>();

            foreach (var direction in directions)
            {
                var toSearch = new Queue<(NodeBase node, int steps)>();
                toSearch.Enqueue((startNode, 0));
                processed.Add(startNode);

                while (toSearch.Any())
                {
                    var (current, currentSteps) = toSearch.Dequeue();

                    if (currentSteps >= maxSteps) continue;

                    var nextPos = current.Coords.Pos + direction;
                    var neighbor = GetNeighborAtPosition(nextPos);

                    if (neighbor != null && neighbor.Walkable && neighbor.tileUnit == null && !processed.Contains(neighbor))
                    {
                        neighbor.SetColor(Color.red);
                        processed.Add(neighbor);
                        toSearch.Enqueue((neighbor, currentSteps + 1));
                    }
                }
            }
        }

        private static NodeBase GetNeighborAtPosition(Vector2 position)
        {
            return GridManager.Instance.GetTileAtPosition(position);
        }

        public static List<NodeBase> FindNearestEnemyNode(NodeBase startNode, Unit[] units)
        {
            NodeBase targetNode = null;
            float minDistance = Mathf.Infinity;

            foreach (Unit unit in units)
            {
                if(unit != startNode.tileUnit)
                {
                    float distance = Vector3.Distance(startNode.gameObject.transform.position, unit.transform.position);
                    if (distance < minDistance)
                    {
                        targetNode = unit._actualNode;
                        minDistance = distance;
                    }
                }
            }

            var toSearch = new List<NodeBase>() { startNode };
            var processed = new List<NodeBase>();

            while (toSearch.Any())
            {
                var current = toSearch[0];
                foreach (var t in toSearch)
                    if (t.F < current.F || t.F == current.F && t.H < current.H) current = t;

                processed.Add(current);
                toSearch.Remove(current);

                current.SetColor(ClosedColor);

                if (current == targetNode)
                {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    var count = 100;
                    while (currentPathTile != startNode)
                    {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection;
                        count--;
                        if (count < 0) throw new Exception();
                    }

                    foreach (var tile in path) tile.SetColor(PathColor);
                    startNode.SetColor(PathColor);
                    return path;
                }

                foreach (var neighbor in current.Neighbors.Where(t => !processed.Contains(t)))
                {
                    var inSearch = toSearch.Contains(neighbor);

                    var costToNeighbor = current.G + current.GetDistance(neighbor);

                    if (!inSearch || costToNeighbor < neighbor.G)
                    {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);

                        if (!inSearch)
                        {
                            neighbor.SetH(neighbor.GetDistance(targetNode));
                            toSearch.Add(neighbor);
                            neighbor.SetColor(OpenColor);
                        }
                    }
                }
            }
            return null;
        }
    }
}
