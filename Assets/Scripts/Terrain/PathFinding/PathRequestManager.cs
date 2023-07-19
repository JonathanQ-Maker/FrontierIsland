using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FrontierIsland
{
    public class PathRequestManager : MonoBehaviour
    {
        public static PathRequestManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("More than one instance of PathRequestManager exists");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private Queue<PathResult> results = new Queue<PathResult>();

        private void Start()
        {
            
        }

        void Update()
        {
            // constantly check for finished path requests
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();

                        if (!result.request.Cancelled)
                        {
                            result.callback(result.path, result.success);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Starts a new thread to compute resolve <paramref name="request"/>
        /// </summary>
        /// <param name="request"></param>
        public static void RequestPath(PathRequest request)
        {
            Task.Run(() => PathFinding.FindPath(request, Instance.FinishedProcessingPath));
        }

        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (PathFinding.grid == null) return;

            Gizmos.color = Color.yellow;
            foreach (PathFinding.Node node in PathFinding.grid)
            {
                if (node != null)
                    Gizmos.DrawCube(new Vector3Int(node.x, 0, node.z) + PathFinding.origin, new Vector3(0.3f, 0.3f, 0.3f));
            }
        }
#endif

    }

    public class PathResult
    {
        public Vector3Int[] path;
        public bool success;
        public Action<Vector3Int[], bool> callback;
        public readonly PathRequest request;

        public PathResult(Vector3Int[] path, bool success, Action<Vector3Int[], bool> callback, PathRequest request)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
            this.request = request;
        }
    }

    public class PathRequest
    {
        public int gridSize;
        public Vector3Int start;
        public Vector3Int end;
        public Action<Vector3Int[], bool> callback;
        private bool cancelled = false;

        public bool Cancelled
        {
            get { return cancelled; }
        }

        public PathRequest(Vector3Int start, Vector3Int end, int gridSize, Action<Vector3Int[], bool> callback)
        {
            this.start = start;
            this.end = end;
            this.callback = callback;
            this.gridSize = gridSize;
        }

        public void Cancel()
        {
            cancelled = true;
        }
    }
}
