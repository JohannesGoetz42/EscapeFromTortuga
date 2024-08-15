using UnityEngine;

public class Node
{
    public Node(bool _isWalkable, Vector3 _worldPosition)
    {
        isWalkable = _isWalkable;
        worldPosition = _worldPosition;
    }

    public bool isWalkable;
    public Vector3 worldPosition;

}
