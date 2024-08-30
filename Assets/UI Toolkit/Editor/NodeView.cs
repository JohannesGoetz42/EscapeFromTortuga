using UnityEngine;

public interface INodeView
{
    public void Update();
    public BehaviorTreeNodeBase GetNode();
}
