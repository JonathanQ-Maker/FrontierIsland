
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrontierIsland
{
    /// <summary>
    /// Path Finding algorithm (A*) adapted from Sebastian Lague
    /// https://youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
    /// </summary>
    public static class PathFinding
    {
#if UNITY_EDITOR
        public
#else
        private
#endif
        class Node : IHeapItem<Node>
        {
            public Node parent;
            public readonly bool walkable;
            public readonly int x, z;
            public float gCost, hCost;

            public float fCost
            {
                get { return gCost + hCost; }
            }

            public int HeapIndex { get; set; }

            public Node(bool walkable, int x, int z)
            {
                this.walkable = walkable;
                this.x = x;
                this.z = z;
            }

            public int CompareTo(Node nodeToCompare)
            {
                int compare = fCost.CompareTo(nodeToCompare.fCost);
                if (compare == 0)
                {
                    compare = hCost.CompareTo(nodeToCompare.hCost);
                }
                return -compare;
            }
        }

#if UNITY_EDITOR
        // DEBUG CODE
        public static Node[,] grid;
        public static Vector3Int origin;
#endif

        /// <summary>
        /// Thread safe path finding with A* algorithm.
        /// 
        /// <br>
        /// Centers a search grid of size <see cref="PathRequest.gridSize"/> around <see cref="PathRequest.start"/>
        /// </br>
        /// <br>
        /// and clamps the grid within <see cref="Terrain"/> boundaries.
        /// </br>
        /// 
        /// <para>
        /// Calls <paramref name="callback"/> on exit.
        /// 
        /// <br>
        /// Exits if path is found, sending results in <see cref="PathResult.path"/> and <see cref="PathResult.success"/> = <see langword="true"/>
        /// </br>
        /// 
        /// <br>
        /// Exits if path is *NOT* found, with 0 length <see cref="PathResult.path"/> and <see cref="PathResult.success"/> = <see langword="false"/>
        /// </br>
        /// </para>
        /// 
        /// <para>
        /// <br>
        /// NOTE1: Path finder ignores walkabilty on the end node such that it can support 
        /// </br>
        /// <br>
        /// pathing to spcific block rather than just pathing to empty space.
        /// </br>
        /// </para>
        /// 
        /// <para>
        /// NOTE2: <see cref="PathRequest.end"/> will be clamped onto the search grid
        /// </para>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public static void FindPath(PathRequest request, Action<PathResult> callback)
        {
            Vector3Int start = request.start, end = request.end;
            int gridSize = request.gridSize;

            if (!Terrain.Instance.WithinBounds(end.x, end.z)
                || !Terrain.Instance.WithinBounds(start.x, start.z))
            {
                // invalid start or end
                callback(new PathResult(Array.Empty<Vector3Int>(), false, request.callback, request));
                return;
            }

            // bottom left corner
            Vector3Int origin = new Vector3Int(
                Mathf.Max(0, start.x - gridSize / 2), 0, 
                Mathf.Max(0, start.z - gridSize / 2));

            int gridSizeX = Mathf.Min(Terrain.Instance.WidthTiles, origin.x + gridSize) - origin.x;
            int gridSizeZ = Mathf.Min(Terrain.Instance.LengthTiles, origin.z + gridSize) - origin.z;

            end.x = Mathf.Clamp(end.x, origin.x, origin.x + gridSizeX - 1);
            end.z = Mathf.Clamp(end.z, origin.z, origin.z + gridSizeZ - 1);

            if (request.Cancelled) return;
            Node[,] grid            = new Node[gridSizeX, gridSizeZ];
            Heap<Node> openSet      = new Heap<Node>(grid.Length);
            HashSet<Node> closedSet = new HashSet<Node>();
            Vector3Int localStart   = start - origin;
            Vector3Int localEnd     = end - origin;
            Node startNode          = new Node(true, localStart.x, localStart.z);
            Node endNode            = new Node(true, localEnd.x, localEnd.z);

#if UNITY_EDITOR
            // DEBUG CODE
            PathFinding.grid = grid;
            PathFinding.origin = origin;
            // DEBUG CODE
#endif

            grid[startNode.x, startNode.z]    = startNode;
            grid[endNode.x, endNode.z]        = endNode;


            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                if (request.Cancelled) return;

                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    // path found
                    List<Node> path = RetracePath(startNode, currentNode);
                    callback(new PathResult(SimplifyPath(path, origin), true, request.callback, request));
                    return;
                }

                Node[] neighbours = GetNeighbours(grid, currentNode,
                    origin + new Vector3Int(currentNode.x, 0, currentNode.z));

                foreach (Node neighbour in neighbours)
                {
                    if (neighbour == null || !neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, endNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            // did not find path
            callback(new PathResult(Array.Empty<Vector3Int>(), false, request.callback, request));
        }

        private static List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Add(startNode);
            return path;
        }

        private static Vector3Int[] SimplifyPath(List<Node> path, Vector3Int origin)
        {
            List<Vector3Int> waypoints = new List<Vector3Int>();
            Vector2 directionOld = Vector2.zero;
            waypoints.Add(origin + new Vector3Int(path[0].x, 0, path[0].z));
            for (int i = 2; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].x - path[i].x, path[i - 1].z - path[i].z);
                if (directionNew != directionOld)
                {
                    waypoints.Add(origin + new Vector3Int(path[i - 1].x, 0, path[i - 1].z));
                }
                directionOld = directionNew;
            }

            //// Adds all nodes, no simplification
            //for (int i = 0; i < path.Count; ++i)
            //{
            //    waypoints.Add(origin + new Vector3Int(path[i].x, 0, path[i].z));
            //}

            waypoints.Reverse();
            return waypoints.ToArray();
        }

        private static Node[] GetNeighbours(Node[,] grid, Node centerNode, Vector3Int worldPos)
        {
            Node[] nodes = new Node[8];
            Node node;
            bool upWalkable = false, rightWalkable = false, downWalkable = false, leftWalkable = false;

            if (centerNode.z + 1 < grid.GetLength(1))
            {
                node = grid[centerNode.x, centerNode.z + 1];
                if (node == null)
                {
                    upWalkable = Terrain.Instance.Walkable(worldPos.x, worldPos.z + 1);
                    node = new Node(upWalkable, centerNode.x, centerNode.z + 1);
                    grid[centerNode.x, centerNode.z + 1] = node;
                }
                nodes[0] = node;
            }

            if (centerNode.x + 1 < grid.GetLength(0))
            {
                node = grid[centerNode.x + 1, centerNode.z];
                if (node == null)
                {
                    rightWalkable = Terrain.Instance.Walkable(worldPos.x + 1, worldPos.z);
                    node = new Node(rightWalkable, centerNode.x + 1, centerNode.z);
                    grid[centerNode.x + 1, centerNode.z] = node;
                }
                nodes[1] = node;
            }

            if (centerNode.z - 1 >= 0)
            {
                node = grid[centerNode.x, centerNode.z - 1];
                if (node == null)
                {
                    downWalkable = Terrain.Instance.Walkable(worldPos.x, worldPos.z - 1);
                    node = new Node(downWalkable, centerNode.x, centerNode.z - 1);
                    grid[centerNode.x, centerNode.z - 1] = node;
                }
                nodes[2] = node;
            }

            if (centerNode.x - 1 >= 0)
            {
                node = grid[centerNode.x - 1, centerNode.z];
                if (node == null)
                {
                    leftWalkable = Terrain.Instance.Walkable(worldPos.x - 1, worldPos.z);
                    node = new Node(leftWalkable, centerNode.x - 1, centerNode.z);
                    grid[centerNode.x - 1, centerNode.z] = node;
                }
                nodes[3] = node;
            }

            // Corners

            if (centerNode.z + 1 < grid.GetLength(1))
            {
                if (centerNode.x + 1 < grid.GetLength(0) && rightWalkable && upWalkable)
                {
                    node = grid[centerNode.x + 1, centerNode.z + 1];
                    if (node == null)
                    {
                        bool walkable = Terrain.Instance.Walkable(worldPos.x + 1, worldPos.z + 1);
                        node = new Node(walkable, centerNode.x + 1, centerNode.z + 1);
                        grid[centerNode.x + 1, centerNode.z + 1] = node;
                    }
                    nodes[4] = node;
                }

                if (centerNode.x - 1 >= 0 && leftWalkable && upWalkable)
                {
                    node = grid[centerNode.x - 1, centerNode.z + 1];
                    if (node == null)
                    {
                        bool walkable = Terrain.Instance.Walkable(worldPos.x - 1, worldPos.z + 1);
                        node = new Node(walkable, centerNode.x - 1, centerNode.z + 1);
                        grid[centerNode.x - 1, centerNode.z + 1] = node;
                    }
                    nodes[5] = node;
                }
            }

            if (centerNode.z - 1 >= 0)
            {
                if (centerNode.x + 1 < grid.GetLength(0) && rightWalkable && downWalkable)
                {
                    node = grid[centerNode.x + 1, centerNode.z - 1];
                    if (node == null)
                    {
                        bool walkable = Terrain.Instance.Walkable(worldPos.x + 1, worldPos.z - 1);
                        node = new Node(walkable, centerNode.x + 1, centerNode.z - 1);
                        grid[centerNode.x + 1, centerNode.z - 1] = node;
                    }
                    nodes[6] = node;
                }

                if (centerNode.x - 1 >= 0 && leftWalkable && downWalkable)
                {
                    node = grid[centerNode.x - 1, centerNode.z - 1];
                    if (node == null)
                    {
                        bool walkable = Terrain.Instance.Walkable(worldPos.x - 1, worldPos.z - 1);
                        node = new Node(walkable, centerNode.x - 1, centerNode.z - 1);
                        grid[centerNode.x - 1, centerNode.z - 1] = node;
                    }
                    nodes[7] = node;
                }
            }
            return nodes;
        }

        private static float GetDistance(Node a, Node b)
        {
            float deltaX = a.x - b.x;
            float deltaZ = a.z - b.z;

            // squared distance is faster and equivilant
            // to squared-root distance for comparison sake
            return deltaX * deltaX + deltaZ * deltaZ;
        }
    }
}
