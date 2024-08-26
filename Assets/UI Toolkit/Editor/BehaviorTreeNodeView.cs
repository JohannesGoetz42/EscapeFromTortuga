using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviorTreeNodeView : Node
{
    public BehaviorTreeNode node;
    public Port parentPort;
    public Port childrenPort;
    public VisualElement serviceContainer;
    public VisualElement decoratorContainer;

    private BehaviorTreeView _behaviorTreeView;

    public BehaviorTreeNodeView(BehaviorTreeNode node, BehaviorTreeView behaviorTreeView) : base("Assets/UI Toolkit/Editor/BehaviorNodeView.uxml")
    {
        this.node = node;
        title = node.nodeName;
        _behaviorTreeView = behaviorTreeView;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateParentPort();
        CreateChildrenPort();

        serviceContainer = this.Q("services");
        decoratorContainer = this.Q("decorators");

        if (node is BehaviorTreeRoot)
        {
            titleContainer.AddToClassList("root");
        }
        else if (node is BehaviorTreeSelector)
        {
            titleContainer.AddToClassList("selector");
        }
        else if (node is BehaviorTreeAction)
        {
            titleContainer.AddToClassList("action");
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
        // actions cant't have children
        if (node is BehaviorTreeAction)
        {
            return;
        }

        childrenPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(BehaviorTreeNode));
        childrenPort.portName = "";
        childrenPort.style.flexDirection = FlexDirection.ColumnReverse;
        outputContainer.Add(childrenPort);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        // add composite nodes
        TypeCache.TypeCollection availableComposites = TypeCache.GetTypesDerivedFrom<BehaviorTreeServiceBase>();
        evt.menu.AppendSeparator();
        foreach (System.Type compositeType in availableComposites)
        {
            evt.menu.AppendAction(compositeType.Name, x => CreateEmbeddedNode(compositeType));
        }

        // add leaf nodes
        TypeCache.TypeCollection availableDecorators = TypeCache.GetTypesDerivedFrom<DecoratorBase>();
        evt.menu.AppendSeparator();
        foreach (System.Type decoratorType in availableDecorators)
        {
            evt.menu.AppendAction(decoratorType.Name, x => CreateEmbeddedNode(decoratorType));
        }
    }

    void CreateEmbeddedNode(System.Type nodeType)
    {
        if(nodeType == null)
        {
            return;
        }

        EmbeddedBehaviorTreeNode embeddedNode = _behaviorTreeView.currentTree.CreateNode(nodeType) as EmbeddedBehaviorTreeNode;
        node.AddChild(embeddedNode);

        if(embeddedNode is DecoratorBase)
        {
            decoratorContainer.Add(CreateEmbeddedNodeView(embeddedNode));
        }

        if(embeddedNode is BehaviorTreeServiceBase)
        {
            serviceContainer.Add(CreateEmbeddedNodeView(embeddedNode));
        }
    }

    EmbeddedBehaviorTreeNodeView CreateEmbeddedNodeView(EmbeddedBehaviorTreeNode node)
    {
        EmbeddedBehaviorTreeNodeView nodeView = new EmbeddedBehaviorTreeNodeView(node);
        return nodeView;
    }
}
