using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

struct PathResult
{
    public PathResult(Vector3[] _path, bool _success, Action<Vector3[], bool> _callback)
    {
        path = _path;
        success = _success;
        callback = _callback;
    }

    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;
}

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager _instance { get; private set; }
    private static Pathfinding _pathFinder;

    private Queue<PathResult> _pathResults = new Queue<PathResult>();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        _pathFinder = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        lock (_pathResults)
        {
            PathResult currentResult;
            while (_pathResults.TryDequeue(out currentResult))
            {
                currentResult.callback(currentResult.path, currentResult.success);
            }
        }
    }

    /** 
     * Request the path with transforms
     * Prefer transforms over Vector3, since it will be updated while waiting in the queue
     */
    public static void RequestPath(PathRequest request)
    {
        if (_pathFinder)
        {
            ThreadStart thread = delegate { _pathFinder.FindPath(request, _instance.FinishedProcessingPath); };
            thread.Invoke();
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success, PathRequest originalRequest)
    {
        PathResult result = new PathResult(path, success, originalRequest.callback);
        lock (_instance._pathResults)
        {
            _instance._pathResults.Enqueue(result);
        }
    }
}
