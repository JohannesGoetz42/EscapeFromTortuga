using UnityEngine;
using System.Collections.Generic;

public class MoveTo : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] NPCController controller;
    [SerializeField] float acceptanceRadius = 0.1f;
    /** 
     * the interval at which the path will be updated
     * will never update the path when interval is negative;
     */
    [SerializeField] float updatePathInterval = -1.0f;

    private Queue<Vector3> path;
    private bool hasFoundPath;
    float timeSincePathUpdate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<NPCController>();
        if (controller != null)
        {
            PathfindingManager.RequestPath(transform, target, OnPathFound);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // update the path if the interval is non-negative and the time since the last update exceeds the interval
        if (updatePathInterval > 0)
        {
            timeSincePathUpdate += Time.deltaTime;
            if (timeSincePathUpdate > updatePathInterval)
            {
                PathfindingManager.RequestPath(transform, target, OnPathFound);
                timeSincePathUpdate = 0;
            }
        }

        if (hasFoundPath)
        {
            // handle path segment target reached
            if ((path.Peek() - transform.position).sqrMagnitude < acceptanceRadius * acceptanceRadius)
            {
                // target is reached
                if (path.Count == 1)
                {
                    hasFoundPath = false;
                    path.Clear();
                    return;
                }

                // start next path segment
                path.Dequeue();
            }

            Vector3 movementDirection = (path.Peek() - transform.position).normalized;
            controller.movementDirection = movementDirection;
        }
    }

    public void OnPathFound(Vector3[] pathResult, bool success)
    {
        hasFoundPath = success && pathResult.Length > 0;
        path = new Queue<Vector3>(pathResult);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (path == null)
        {
            return;
        }

        foreach (Vector3 waypoint in path.ToArray())
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(waypoint, 1);
        }
    }
#endif
}
