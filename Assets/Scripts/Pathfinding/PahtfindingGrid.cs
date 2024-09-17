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
    [SerializeField]
    float nodeRadius;
#if UNITY_EDITOR
    [SerializeField]
    bool drawDebug;
#endif
    PathfindingNode[,] grid;
    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    public int MaxGridSize { get { return gridSizeX * gridSizeY; } }

    public PathfindingNode GetNodeAtWorldPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - transform.localPosition;

        float coordX = (relativePosition.x + gridWorldSize.x * 0.5f) / gridWorldSize.x;
        float coordY = (relativePosition.z + gridWorldSize.z * 0.5f) / gridWorldSize.z;
        coordX = Mathf.Clamp(coordX, 0.0f, 1.0f);
        coordY = Mathf.Clamp(coordY, 0.0f, 1.0f);

        int x = Mathf.RoundToInt((gridSizeX - 1) * coordX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * coordY);

        return grid[x, y];
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
        nodeDiameter = nodeRadius * 2;
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
                nodePosition = floorHit.point;

                // check if obstructed by unwalkable object
                if (walkable)
                {
                    walkable = !Physics.CheckSphere(nodePosition, nodeRadius, unwalkableMask);
                }

                grid[x, y] = new PathfindingNode(walkable, nodePosition, x, y);
            }
        }
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
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
#endif
}
