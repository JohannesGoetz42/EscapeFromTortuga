using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviorTreeNodeView : Node
{
    public BehaviorTreeNode node;
    public Port parentPort;
    public Port childrenPort;

    public BehaviorTreeNodeView(BehaviorTreeNode node)
    {
        this.node = node;
        title = node.nodeName;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateParentPort();
        CreateChildrenPort();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.position = newPos.min;
    }

    private void CreateParentPort()
    {
        parentPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(BehaviorTreeNode));
        parentPort.portName = "";
        inputContainer.Add(parentPort);
    }
    private void CreateChildrenPort()
    {
        // leafs cant't have children
        if (node is BehaviorTreeLeaf)
        {
            return;
        }

        childrenPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(BehaviorTreeNode));
        childrenPort.portName = "";
        outputContainer.Add(childrenPort);
    }

}
