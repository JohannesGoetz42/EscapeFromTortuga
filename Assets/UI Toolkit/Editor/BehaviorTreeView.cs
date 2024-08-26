using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

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
    private BehaviorTree currentTree;
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

    public void SetBehaviorTree(BehaviorTree selectedTree)
    {
        if (selectedTree == null)
        {
            return;
        }

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        currentTree = selectedTree;
        if (currentTree.root == null)
        {
            currentTree.root = ScriptableObject.CreateInstance<BehaviorTreeRoot>();
            currentTree.nodes.Insert(0, currentTree.root);
        }

        List<EdgeData> edges = new List<EdgeData>();
        List<BehaviorTreeNodeView> nodeViews = new List<BehaviorTreeNodeView>();

        // create nodes
        foreach (BehaviorTreeNode node in selectedTree.nodes)
        {
            BehaviorTreeNodeView createdNode = CreateNodeView(node);
            nodeViews.Add(createdNode);
            if (node.parent != null)
            {
                edges.Add(new EdgeData(node.parent, createdNode));
            }
        }

        // create edges after every node is created
        foreach (EdgeData edge in edges)
        {
            BehaviorTreeNodeView parentView = nodeViews.First(x => x.node == edge.parentNode);
            Edge graphEdge = parentView.childrenPort.ConnectTo(edge.childNodeView.parentPort);
            AddElement(graphEdge);
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
        // remove removed elements from tree
        if (graphViewChange.elementsToRemove != null)
        {

            foreach (var item in graphViewChange.elementsToRemove)
            {
                // remove nodes
                BehaviorTreeNodeView nodeView = item as BehaviorTreeNodeView;
                if (nodeView != null)
                {
                    currentTree.DeleteNode(nodeView.node);
                    continue;
                }

                // remove edges
                Edge edge = item as Edge;
                if (edge != null)
                {
                    BehaviorTreeNodeView parent = edge.output.node as BehaviorTreeNodeView;
                    BehaviorTreeNodeView child = edge.input.node as BehaviorTreeNodeView;
                    parent.node.RemoveChild(child.node);
                    child.node.SetParent(null);
                }
            }
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

                // TODO: order children
                parent.node.AddChild(child.node);
            }
        }

        return graphViewChange;
    }

    private void CreateNode(Type nodeType, Vector2 position)
    {
        if (currentTree == null)
        {
            return;
        }

        BehaviorTreeNode node = currentTree.CreateNode(nodeType);
        BehaviorTreeNodeView nodeView = CreateNodeView(node);
        nodeView.SetPosition(new Rect(position, new Vector2(200, 50)));
    }

    public BehaviorTreeNodeView CreateNodeView(BehaviorTreeNode node)
    {
        BehaviorTreeNodeView nodeView = new BehaviorTreeNodeView(node);
        AddElement(nodeView);
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
        TypeCache.TypeCollection availableNodes = TypeCache.GetTypesDerivedFrom<BehaviorTreeLeaf>();
        evt.menu.AppendSeparator();
        foreach (Type leafType in availableNodes)
        {
            evt.menu.AppendAction(leafType.Name, x => CreateNode(leafType, position));
            //evt.menu.AppendAction(string.Format("[{0}] {1}", leafType.BaseType.Name, leafType.Name), x => CreateNode(leafType));
        }
    }
}
#endif
