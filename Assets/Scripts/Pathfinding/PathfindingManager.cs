using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

struct PathResult
{
    public PathResult(IBehaviorTreeUser _user, Vector3[] _path, bool _success, Action<Vector3[], bool, IBehaviorTreeUser> _callback)
    {
        user = _user;
        path = _path;
        success = _success;
        callback = _callback;
    }

    public IBehaviorTreeUser user;
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool, IBehaviorTreeUser> callback;
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
                currentResult.callback(currentResult.path, currentResult.success, currentResult.user);
            }
        }
    }

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
        PathResult result = new PathResult(originalRequest.user, path, success, originalRequest.callback);
        lock (_instance._pathResults)
        {
            _instance._pathResults.Enqueue(result);
        }
    }
}
