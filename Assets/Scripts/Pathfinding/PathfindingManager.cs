using System;
using System.Collections.Generic;
using UnityEngine;

struct PathRequest
{
    public Transform startTransform;
    public Transform targetTransform;
    public Action<Vector3[], bool> callback;

    public PathRequest(Transform _startTransform, Transform _targetTransform, Action<Vector3[], bool> _callback)
    {
        startTransform = _startTransform;
        targetTransform = _targetTransform;
        callback = _callback;
    }
}

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager _instance { get; private set; }
    private static Pathfinding _pathFinder;

    private Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
    PathRequest currentRequest;
    private float queueProcessingInterval = 0.5f;
    private bool isProcessing;

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

    /** 
     * Request the path with transforms
     * Prefer transforms over Vector3, since it will be updated while waiting in the queue
     */
    public static void RequestPath(Transform startTransform, Transform targetTransform, Action<Vector3[], bool> callback)
    {
        PathRequest request = new PathRequest(startTransform, targetTransform, callback);
        _instance._requestQueue.Enqueue(request);

        // invoke with delay to ensure everything is set up
        _instance.Invoke(nameof(TryProcessNextInQueue), _instance.queueProcessingInterval);
    }

    void TryProcessNextInQueue()
    {
        if (isProcessing || _requestQueue.Count == 0)
        {
            return;
        }

        isProcessing = true;
        currentRequest = _requestQueue.Dequeue();
        StartCoroutine(_pathFinder.FindPath(currentRequest.startTransform.position, currentRequest.targetTransform.position, this));
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentRequest.callback(path, success);
        isProcessing = false;
        if (_requestQueue.Count > 0)
        {
            Invoke(nameof(TryProcessNextInQueue), queueProcessingInterval);
        }
    }
}
