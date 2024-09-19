using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PathfindingGrid : MonoBehaviour
{
    [SerializeField]
    LayerMask unwalkableMask;
    [SerializeField]
    LayerMask walkableMask;
    [SerializeField]
    Vector3 gridWorldSize;
    [field: SerializeField]
    public float nodeRadius { get; private set; } = 0.5f;
#if UNITY_EDITOR
    [SerializeField]
    bool drawDebug;
#endif
    PathfindingNode[,] grid;
    int gridSizeX;
    int gridSizeY;

    public int MaxGridSize { get { return gridSizeX * gridSizeY; } }

    public PathfindingNode GetNodeAtWorldPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - transform.localPosition + gridWorldSize * 0.5f;

        int x = Mathf.FloorToInt(relativePosition.x / (2 * nodeRadius));
        int y = Mathf.FloorToInt(relativePosition.z / (2 * nodeRadius));

        if (x < grid.GetLength(0) || y < grid.GetLength(1))
        {
            return grid[x, y];
        }

        Debug.LogErrorFormat("Error finding pathfinding position at {0}", worldPosition.ToString());
        return grid[0, 0];
    }

    public List<PathfindingNode> GetNeighbors(PathfindingNode node)
    {
        List<PathfindingNode> result = new List<PathfindingNode>();
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                // skip self
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // if the current coordinate is in the grid, add it as result
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    result.Add(grid[checkX, checkY]);
                }
            }

        return result;
    }

    void Start()
    {
        float nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
        CreateGrid();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (drawDebug && !Application.isPlaying)
        {
            CreateGrid();
        }
    }
#endif

    void CreateGrid()
    {
        float nodeDiameter = nodeRadius * 2;
        grid = new PathfindingNode[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2;
        float diameterPlusRadius = nodeDiameter + nodeRadius;

        float maxFloorHeight = gridWorldSize.y * 0.5f + transform.position.y;
        // create nodes for every row
        for (int x = 0; x < gridSizeX; x++)
        {
            // create nodes for every column
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePosition = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);


                // try to find a floor
                Vector3 rayStart = nodePosition;
                rayStart.y = maxFloorHeight;

                RaycastHit floorHit;
                bool walkable = Physics.Raycast(rayStart, Vector3.down, out floorHit, gridWorldSize.y, walkableMask);

                // check if obstructed by unwalkable object
                if (walkable)
                {
                    nodePosition = floorHit.point;
                    walkable = !Physics.CheckSphere(nodePosition, nodeRadius, unwalkableMask);
                }

                grid[x, y] = new PathfindingNode(walkable, nodePosition, x, y);
            }
        }
    }

    internal PathfindingNode TryGetNodeAtCoordinates(int x, int y)
    {
        if (grid.GetLength(0) > x && grid.GetLength(1) > y)
        {
            return grid[x, y];
        }

        return null;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        if (drawDebug && grid != null)
        {
            foreach (PathfindingNode node in grid)
            {
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeRadius * 2.0f - 0.1f));
            }
        }

        // indicate grid index direction
        if (grid != null)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawCube(grid[0, 0].worldPosition, Vector3.one * (nodeRadius * 2.0f));
            Gizmos.color = Color.red;
            Gizmos.DrawCube(grid[2, 0].worldPosition, Vector3.one * (nodeRadius * 2.0f));
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(grid[0, 2].worldPosition, Vector3.one * (nodeRadius * 2.0f));
        }
    }
#endif
}
