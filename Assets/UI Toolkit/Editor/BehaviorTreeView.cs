using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR

public struct EdgeData
{
    public EdgeData(BehaviorTreeNode parent, BehaviorTreeNodeView childView)
    {
        parentNode = parent;
        childNodeView = childView;
    }

    public BehaviorTreeNode parentNode;
    public BehaviorTreeNodeView childNodeView;
}

[UxmlElement]
public partial class BehaviorTreeView : GraphView
{
    public BehaviorTree CurrentTree { get => editor != null ? editor.CurrentTree : null; }
    public BehaviorTreeEditor editor;

    private List<BehaviorTreeNodeView> nodeViews = new List<BehaviorTreeNodeView>();

    public BehaviorTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/Editor/BehaviorTreeEditor.uss");
        styleSheets.Add(style);
    }

    public void OnBehaviorTreeChanged()
    {
        if (CurrentTree == null)
        {
            return;
        }

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        List<BehaviorTreeNodeView> nodeViews = new List<BehaviorTreeNodeView>();
        if (CurrentTree.root == null)
        {
            CurrentTree.root = ScriptableObject.CreateInstance<BehaviorTreeRoot>();
            AssetDatabase.AddObjectToAsset(CurrentTree.root, CurrentTree);
            AssetDatabase.SaveAssets();
        }

        nodeViews.Add(CreateNodeView(CurrentTree.root));

        List<EdgeData> edges = new List<EdgeData>();

        // create nodes
        foreach (BehaviorTreeNode node in CurrentTree.nodes)
        {
            if (node == null)
            {
                Debug.LogWarning("Behavior tree has null node!");
                continue;
            }

            BehaviorTreeNodeView createdNode = CreateNodeView(node);
            nodeViews.Add(createdNode);
            if (node.parent != null)
            {
                edges.Add(new EdgeData(node.parent, createdNode));
            }

            // create decorators
            foreach (DecoratorBase decorator in node.decorators)
            {
                EmbeddedBehaviorTreeNodeView decoratorView = new EmbeddedBehaviorTreeNodeView(decorator, createdNode);
                createdNode.decoratorContainer.Add(decoratorView);
            }

            // create services
            foreach (BehaviorTreeServiceBase service in node.services)
            {
                EmbeddedBehaviorTreeNodeView serviceView = new EmbeddedBehaviorTreeNodeView(service, createdNode);
                createdNode.serviceContainer.Add(serviceView);
            }
        }

        HashSet<BehaviorTreeNodeView> nodesToOrder = new HashSet<BehaviorTreeNodeView>();
        // create edges after every node is created
        foreach (EdgeData edge in edges)
        {
            BehaviorTreeNodeView parentView = nodeViews.First(x => x.node == edge.parentNode);
            Edge graphEdge = parentView.childrenPort.ConnectTo(edge.childNodeView.parentPort);
            AddElement(graphEdge);
            nodesToOrder.Add(parentView);
        }

        // order all nodes that are parent so another node
        foreach (BehaviorTreeNodeView parent in nodesToOrder)
        {
            parent.OrderChildren();
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        BehaviorTreeNodeView startPortView = startPort.node as BehaviorTreeNodeView;
        List<Port> result = new List<Port>();
        foreach (Port endPort in ports)
        {
            if (endPort.direction != startPort.direction && endPort.node != startPort.node)
            {
                BehaviorTreeNodeView endPortView = endPort.node as BehaviorTreeNodeView;
                if (endPortView == null || endPortView.node.IsAncestorOf(startPortView.node) || endPortView.node.IsDescendantOf(startPortView.node))
                {
                    continue;
                }

                result.Add(endPort);
            }
        }

        return result;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        List<BehaviorTreeNodeView> nodesToOrder = new List<BehaviorTreeNodeView>();

        // remove removed elements from tree
        if (graphViewChange.elementsToRemove != null)
        {
            foreach (var item in graphViewChange.elementsToRemove)
            {
                // remove nodes
                BehaviorTreeNodeView nodeView = item as BehaviorTreeNodeView;
                if (nodeView != null)
                {
                    CurrentTree.DeleteNode(nodeView.node);

                    BehaviorTreeNodeView parentNode = nodeView.GetParentNode();
                    if (parentNode != null)
                    {
                        nodesToOrder.Add(parentNode);
                    }
                    continue;
                }

                // remove edges
                Edge edge = item as Edge;
                if (edge != null)
                {
                    BehaviorTreeNodeView parent = edge.output.node as BehaviorTreeNodeView;
                    BehaviorTreeNodeView child = edge.input.node as BehaviorTreeNodeView;
                    parent.node.RemoveChild(child.node);
                    nodesToOrder.Add(parent);
                    child.node.SetParent(null);
                }
            }

        }

        // add parents of moved nodes to nodesToOrder
        if (graphViewChange.movedElements != null)
        {
            foreach (BehaviorTreeNodeView movedNode in graphViewChange.movedElements.Select(x => x as BehaviorTreeNodeView).Where(x => x != null))
            {
                BehaviorTreeNodeView parentNode = movedNode.GetParentNode();
                if (parentNode != null)
                {
                    nodesToOrder.Add(parentNode);
                }
            }
        }

        // order parents of removed or moved nodes, if they aren't removed themselves
        foreach (BehaviorTreeNodeView node in nodesToOrder.Where(x => graphViewChange.elementsToRemove == null || !graphViewChange.elementsToRemove.Contains(x)))
        {
            node.OrderChildren(null, graphViewChange.elementsToRemove);
        }

        // connect nodes
        if (graphViewChange.edgesToCreate != null)
        {
            foreach (Edge edge in graphViewChange.edgesToCreate)
            {
                BehaviorTreeNodeView parent = edge.output.node as BehaviorTreeNodeView;
                BehaviorTreeNodeView child = edge.input.node as BehaviorTreeNodeView;
                if (parent == null || child == null)
                {
                    Debug.LogError("Tried to add an invalid behavior node edge!");
                    continue;
                }

                child.node.SetParent(parent.node);

                parent.node.AddChild(child.node);
                parent.OrderChildren(child);
            }
        }

        return graphViewChange;
    }

    private void CreateNode(Type nodeType, Vector2 position)
    {
        if (CurrentTree == null)
        {
            return;
        }

        BehaviorTreeNode node = CurrentTree.CreateNode(nodeType) as BehaviorTreeNode;
        if (node != null)
        {
            BehaviorTreeNodeView nodeView = CreateNodeView(node);
            nodeView.SetPosition(new Rect(position, new Vector2(200, 50)));
        }
    }

    public BehaviorTreeNodeView CreateNodeView(BehaviorTreeNode node)
    {
        BehaviorTreeNodeView nodeView = new BehaviorTreeNodeView(node, this);
        AddElement(nodeView);
        nodeViews.Add(nodeView);
        return nodeView;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        // add composite nodes
        TypeCache.TypeCollection availableComposites = TypeCache.GetTypesDerivedFrom<BehaviorTreeCompositeNode>();
        evt.menu.AppendSeparator();
        // capture this as a local variable so it will not be reset in the lambda function
        Vector2 position = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
        foreach (Type compositeType in availableComposites)
        {
            evt.menu.AppendAction(compositeType.Name, x => CreateNode(compositeType, position));
            //evt.menu.AppendAction(string.Format("[{0}] {1}", compositeType.BaseType.Name, compositeType.Name), x => CreateNode(compositeType));
        }

        // add leaf nodes
        TypeCache.TypeCollection availableNodes = TypeCache.GetTypesDerivedFrom<BehaviorTreeAction>();
        evt.menu.AppendSeparator();
        foreach (Type leafType in availableNodes)
        {
            evt.menu.AppendAction(leafType.Name, x => CreateNode(leafType, position));
            //evt.menu.AppendAction(string.Format("[{0}] {1}", leafType.BaseType.Name, leafType.Name), x => CreateNode(leafType));
        }
    }

    internal void UpdateActiveNodes()
    {
        if (editor == null || editor.debugUser == null)
        {
            return;
        }

        foreach (BehaviorTreeNodeView nodeView in nodeViews)
        {
            nodeView.UpdateNodeState();
        }
    }
}
#endif
