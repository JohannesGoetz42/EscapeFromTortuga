using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Profiling;
using System.Collections;

public class Pathfinding : MonoBehaviour
{
    /** The grid to use. If left empty, a grid on the same gameobject will be selected */
    [SerializeField] PathfindingGrid grid;

    void Awake()
    {
        if (grid == null)
        {
            grid = GetComponent<PathfindingGrid>();
        }
    }

    public IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition, PathfindingManager pathfindingManager)
    {
        Profiler.BeginSample("Pathfinding");
        PathfindingNode startNode = grid.GetNodeAtWorldPosition(startPosition);
        PathfindingNode targetNode = grid.GetNodeAtWorldPosition(targetPosition);

        if (!startNode.isWalkable || !targetNode.isWalkable)
        {
            yield return null;
        }

        Vector3[] waypoints = new Vector3[0];
        bool success = false;

        Heap<PathfindingNode> openSet = new Heap<PathfindingNode>(grid.MaxGridSize);
        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();


        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            PathfindingNode currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);

            // the target has been found
            if (currentNode == targetNode)
            {
                success = true;
                break;
            }

            foreach (PathfindingNode neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.pathPredecessor = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }

        }

        yield return null;
        if (success)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        pathfindingManager.FinishedProcessingPath(waypoints, success);
        Profiler.EndSample();
    }

    Vector3[] RetracePath(PathfindingNode startNode, PathfindingNode endNode)
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.pathPredecessor;
        }

        Vector3[] result = SimplifyPath(path);
        Array.Reverse(result);
        return result;
    }

    Vector3[] SimplifyPath(List<PathfindingNode> path)
    {
        List<Vector3> result = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (!directionNew.Equals(directionOld))
            {
                result.Add(path[i].worldPosition);
            }

            directionOld = directionNew;
        }

        return result.ToArray();
    }

    int GetDistance(PathfindingNode nodeA, PathfindingNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
