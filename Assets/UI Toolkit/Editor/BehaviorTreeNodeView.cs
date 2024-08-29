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

    public BehaviorTreeView BehaviorTreeView { get; private set; }

    public BehaviorTreeNodeView(BehaviorTreeNode node, BehaviorTreeView behaviorTreeView) : base("Assets/UI Toolkit/Editor/BehaviorNodeView.uxml")
    {
        this.node = node;
        title = node.nodeName;
        BehaviorTreeView = behaviorTreeView;

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
        evt.StopPropagation();

        EmbeddedBehaviorTreeNodeView embeddedNode = TryGetClickedEmbeddedNode(evt);
        if (embeddedNode != null)
        {
            evt.menu.AppendAction("Remove", x => RemoveEmbeddedNode(embeddedNode));
            return;
        }

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

    EmbeddedBehaviorTreeNodeView TryGetClickedEmbeddedNode(IMouseEvent mouseEvent)
    {
        // try find service that is hit
        if (serviceContainer.worldBound.Contains(mouseEvent.mousePosition))
        {
            foreach (VisualElement service in serviceContainer.Children())
            {
                if (service.worldBound.Contains(mouseEvent.mousePosition))
                {
                    return service as EmbeddedBehaviorTreeNodeView;
                }
            }
        }
        // try find decorator that is hit
        else if (decoratorContainer.worldBound.Contains(mouseEvent.mousePosition))
        {
            foreach (VisualElement decorator in decoratorContainer.Children())
            {
                if (decorator.worldBound.Contains(mouseEvent.mousePosition))
                {
                    return decorator as EmbeddedBehaviorTreeNodeView;
                }
            }
        }

        return null;
    }

    void CreateEmbeddedNode(System.Type nodeType)
    {
        if (nodeType == null)
        {
            return;
        }

        EmbeddedBehaviorTreeNode embeddedNode = BehaviorTreeView.CurrentTree.CreateNode(nodeType) as EmbeddedBehaviorTreeNode;
        node.AddChild(embeddedNode);

        if (embeddedNode is DecoratorBase)
        {
            decoratorContainer.Add(CreateEmbeddedNodeView(embeddedNode));
        }

        if (embeddedNode is BehaviorTreeServiceBase)
        {
            serviceContainer.Add(CreateEmbeddedNodeView(embeddedNode));
        }
    }

    EmbeddedBehaviorTreeNodeView CreateEmbeddedNodeView(EmbeddedBehaviorTreeNode node)
    {
        EmbeddedBehaviorTreeNodeView nodeView = new EmbeddedBehaviorTreeNodeView(node, this);
        return nodeView;
    }

    void RemoveEmbeddedNode(EmbeddedBehaviorTreeNodeView embeddedView)
    {
        node.behaviorTree.DeleteNode(embeddedView.embeddedNode);
        if (embeddedView.embeddedNode is DecoratorBase)
        {
            decoratorContainer.Remove(embeddedView);
        }
        else if (embeddedView.embeddedNode is BehaviorTreeServiceBase)
        {
            serviceContainer.Remove(embeddedView);
        }
    }
}
