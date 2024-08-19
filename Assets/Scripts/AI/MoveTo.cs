using UnityEngine;
using System.Collections.Generic;

public class MoveTo : MonoBehaviour
{
    [SerializeField] NPCController controller;
    [SerializeField] float acceptanceRadius = 0.1f;
    /** 
     * the interval at which the path will be updated
     * will never update the path when interval is negative;
     */
    [SerializeField] float updatePathInterval = -1.0f;
    [SerializeField] float updateIntervalRange = 0.3f;

    private Queue<Vector3> path;
    private bool hasFoundPath;
    private float timeSincePathUpdate;
    /** randomize the update interval to avoid updating every instance at the same time */
    private float nextUpdateInterval;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<NPCController>();
        // if no controller is found, suppress updates
        if (controller == null)
        {
            nextUpdateInterval = -1.0f;
            Debug.LogError(string.Format("MoveTo script on '{0}' could not find an NPCController!", gameObject.name));
            return;
        }

        if (updatePathInterval > 0.0f)
        {
            nextUpdateInterval = Random.Range(updatePathInterval - updateIntervalRange, updatePathInterval + updateIntervalRange);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.movementTarget == null)
        {
            return;
        }

        // update the path if the interval is positive and the time since the last update exceeds the interval
        if (nextUpdateInterval > 0.0f)
        {
            timeSincePathUpdate += Time.deltaTime;
            if (timeSincePathUpdate > nextUpdateInterval)
            {
                PathfindingManager.RequestPath(new PathRequest(transform.position, controller.movementTarget.position, OnPathFound));
                timeSincePathUpdate = 0;
                nextUpdateInterval = Random.Range(updatePathInterval - updateIntervalRange, updatePathInterval + updateIntervalRange);
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
