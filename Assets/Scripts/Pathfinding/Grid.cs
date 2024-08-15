using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        float diameterPlusRadius = nodeDiameter + nodeRadius;

        // create nodes for every row
        for (int x = 0; x < gridSizeX; x++)
        {
            // create nodes for every column
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePosition = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(nodePosition, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, nodePosition);
            }
        }

        Debug.Log(string.Format("CreatedGrid: {0} x {1}", gridSizeX, gridSizeY));
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    public Node GetNodeAtWorldPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - transform.localPosition;

        float coordX = (relativePosition.x + gridWorldSize.x * 0.5f) / gridWorldSize.x;
        float coordY = (relativePosition.z + gridWorldSize.y * 0.5f) / gridWorldSize.y;
        coordX = Mathf.Clamp(coordX, 0.0f, 1.0f);
        coordY = Mathf.Clamp(coordY, 0.0f, 1.0f);

        int x = Mathf.RoundToInt((gridSizeX - 1) * coordX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * coordY);

        return grid[x, y];
    }
}
