using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviorTreeNodeView : Node
{
    public BehaviorTreeNode node;
    public Port parentPort;
    public Port childrenPort;

    public BehaviorTreeNodeView(BehaviorTreeNode node) : base("Assets/UI Toolkit/Editor/BehaviorNodeView.uxml")
    {
        this.node = node;
        title = node.nodeName;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateParentPort();
        CreateChildrenPort();

        if (node is BehaviorTreeRoot)
        {
            titleContainer.AddToClassList("root");
        }
        else if (node is BehaviorTreeSelector)
        {
            titleContainer.AddToClassList("selector");
        }
        else if (node is BehaviorTreeLeaf)
        {
            titleContainer.AddToClassList("leaf");
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.position = newPos.min;
    }

    private void CreateParentPort()
    {
        // root node should not have a parent
        if (node is BehaviorTreeRoot)
        {
            return;
        }

        parentPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(BehaviorTreeNode));
        parentPort.portName = "";
        parentPort.style.flexDirection = FlexDirection.Column;
        inputContainer.Add(parentPort);
    }
    private void CreateChildrenPort()
    {
        // leafs cant't have children
        if (node is BehaviorTreeLeaf)
        {
            return;
        }

        childrenPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(BehaviorTreeNode));
        childrenPort.portName = "";
        childrenPort.style.flexDirection = FlexDirection.ColumnReverse;
        outputContainer.Add(childrenPort);
    }

}
