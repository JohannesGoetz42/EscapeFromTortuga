using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;

public class BehaviorTreeNodeView : Node, INodeView
{
    public BehaviorTreeNode node;
    public Port parentPort;
    public Port childrenPort;
    public VisualElement serviceContainer;
    public VisualElement decoratorContainer;
    public Label orderIndex;
    public Label nodeTypeLabel;

    public BehaviorTreeView BehaviorTreeView { get; private set; }

    private BehaviorNodeState nodeState = BehaviorNodeState.Inactive;

    public BehaviorTreeNodeView(BehaviorTreeNode node, BehaviorTreeView behaviorTreeView) : base("Assets/UI Toolkit/Editor/BehaviorNodeView.uxml")
    {
        this.node = node;
        title = node.nodeName == "" ? node.nodeTypeName : node.nodeName;
        BehaviorTreeView = behaviorTreeView;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateParentPort();
        CreateChildrenPort();

        serviceContainer = this.Q("services");
        decoratorContainer = this.Q("decorators");
        orderIndex = this.Q("orderIndex") as Label;

        nodeTypeLabel = this.Q("nodeTypeLabel") as Label;
        if (nodeTypeLabel != null)
        {
            nodeTypeLabel.text = node.nodeTypeName;
        }

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

    public override void OnSelected()
    {
        base.OnSelected();
        BehaviorTreeView.editor.SelectNode(this);
    }

    public BehaviorTreeNodeView GetParentNode()
    {
        if (parentPort == null || parentPort.connections == null)
        {
            return null;
        }

        return parentPort.connections.Select(x => x.output.node as BehaviorTreeNodeView).FirstOrDefault();
    }

    public IEnumerable<BehaviorTreeNodeView> GetChildNodes()
    {
        if (childrenPort == null)
        {
            return new BehaviorTreeNodeView[0];
        }

        return childrenPort.connections.Select(x => x.input.node as BehaviorTreeNodeView).Where(x => x != null);
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
        BehaviorTreeView.embeddedNodeViews.Add(nodeView);
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

    BehaviorTreeNodeBase INodeView.GetNode() => node;
    void INodeView.Update()
    {
        if (title != null && node != null)
        {
            title = node.nodeName;
        }
    }

    internal void UpdateNodeState()
    {
        if (!node.States.ContainsKey(BehaviorTreeView.editor.debugUser))
        {
            return;
        }

        BehaviorNodeState newState = node.States[BehaviorTreeView.editor.debugUser];
        if (nodeState == newState)
        {
            return;
        }

        nodeState = newState;

        RemoveFromClassList("active");
        RemoveFromClassList("success");
        RemoveFromClassList("aborted");
        RemoveFromClassList("failed");

        switch (newState)
        {
            case BehaviorNodeState.Inactive:
                return;
            case BehaviorNodeState.Active:
                AddToClassList("active");
                return;
            case BehaviorNodeState.Success:
                AddToClassList("success");
                return;
            case BehaviorNodeState.Aborted:
                AddToClassList("aborted");
                return;
            case BehaviorNodeState.Failed:
                AddToClassList("failed");
                return;
        }
    }

    /** 
     * Order the child nodes based on their position in x direction
     * since the added or removed nodes are still there when grap is updated (I could not find post update event for that),
     * add newChild and skip removedElements when sorting
     */
    internal void OrderChildren(BehaviorTreeNodeView newChild = null, List<GraphElement> removedElements = null)
    {
        // order is only relevant for composite nodes
        BehaviorTreeCompositeNode compositeNode = node as BehaviorTreeCompositeNode;
        if (compositeNode == null)
        {
            return;
        }

        // get all child nodes that are not removed
        List<BehaviorTreeNodeView> childNodes = GetChildNodes().Where(x => removedElements == null || !removedElements.Contains(x)).ToList();

        // if a new child is added, add it before ordering
        if (newChild != null)
        {
            childNodes.Add(newChild);
        }

        // get all child nodes ordered by x position
        childNodes = childNodes.OrderBy(x => x.node.position.x).ToList();

        for (int i = 0; i < childNodes.Count; i++)
        {
            childNodes[i].SetOrderIndex(i);
        }

        compositeNode.children = childNodes.Select(x => x.node).ToList();
    }

    internal void SetOrderIndex(int index)
    {
        if (orderIndex != null)
        {
            orderIndex.text = index.ToString();
        }
    }
}
