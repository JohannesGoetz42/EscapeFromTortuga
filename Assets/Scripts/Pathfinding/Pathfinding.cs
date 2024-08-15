using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Pathfinding : MonoBehaviour
{
    /** The grid to use. If left empty, a grid on the same gameobject will be selected */
    [SerializeField] PathfindingGrid grid;
    [SerializeField] Transform seeker;
    [SerializeField] Transform target;

    void Awake()
    {
        if (grid == null)
        {
            grid = GetComponent<PathfindingGrid>();
        }
    }

    private void Update()
    {
        if (seeker != null && target != null)
        {
            FindPath(seeker.position, target.position);
        }
    }

    public void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Profiler.BeginSample("Pathfinding");
        List<PathfindingNode> openSet = new List<PathfindingNode>();
        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();


        PathfindingNode startNode = grid.GetNodeAtWorldPosition(startPosition);
        PathfindingNode targetNode = grid.GetNodeAtWorldPosition(targetPosition);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            PathfindingNode currentNode = openSet[0];

            foreach (PathfindingNode node in openSet)
            {
                if (node.IsCheaperThan(currentNode))
                {
                    currentNode = node;
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // the target has been found
            if (currentNode == targetNode)
            {
                Profiler.EndSample();
                RetracePath(startNode, targetNode);
                return;
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

        Debug.LogError("Could not find a valid path!");
        Profiler.EndSample();
    }

    void RetracePath(PathfindingNode startNode, PathfindingNode endNode)
    {
        grid.path = new List<PathfindingNode>();
        PathfindingNode currentNode = endNode;

        while (currentNode != startNode)
        {
            grid.path.Add(currentNode);
            currentNode = currentNode.pathPredecessor;
        }

        grid.path.Reverse();
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
