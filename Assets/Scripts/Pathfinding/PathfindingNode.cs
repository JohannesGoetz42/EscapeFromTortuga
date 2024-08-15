using UnityEngine;

public class PathfindingNode
{
    public int gridX {  get; private set; }
    public int gridY { get; private set; }

    public PathfindingNode pathPredecessor;
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public PathfindingNode(bool _isWalkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        isWalkable = _isWalkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    public bool IsCheaperThan(PathfindingNode otherNode)
    {
        return fCost < otherNode.fCost || (fCost == otherNode.fCost && hCost < otherNode.hCost);
    }

    public bool isWalkable;
    public Vector3 worldPosition;

}
