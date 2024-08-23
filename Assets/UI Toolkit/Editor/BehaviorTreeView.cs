using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
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
        foreach (BehaviorTreeNode node in selectedTree.nodes)
        {
            CreateNodeView(node);
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        // remove removed elements from tree
        if (graphViewChange.elementsToRemove != null)
        {
            IEnumerable<BehaviorTreeNodeView> removedNodes = graphViewChange.elementsToRemove.Select(x => x as BehaviorTreeNodeView)
                .Where(x => x != null);

            foreach (BehaviorTreeNodeView nodeView in removedNodes)
            {
                currentTree.DeleteNode(nodeView.node);
            }
        }

        // connect nodes
        if(graphViewChange.edgesToCreate != null)
        {
            foreach(Edge edge in graphViewChange.edgesToCreate)
            {
                BehaviorTreeNodeView parent = edge.output.node as BehaviorTreeNodeView;
                BehaviorTreeNodeView child = edge.input.node as BehaviorTreeNodeView;
                if (parent == null || child == null)
                {
                    Debug.LogError("Tried to add an invalid behavior node edge!");
                    continue;
                }

                BehaviorTreeCompositeNode parentNode = parent.node as BehaviorTreeCompositeNode;
                child.node.SetParent(parent.node);

                // TODO: order children
                parentNode.AddChild(child.node);
            }
        }

        return graphViewChange;
    }

    private void CreateNode(Type nodeType)
    {
        BehaviorTreeNode node = currentTree.CreateNode(nodeType);
        CreateNodeView(node);
    }

    public void CreateNodeView(BehaviorTreeNode node)
    {
        BehaviorTreeNodeView nodeView = new BehaviorTreeNodeView(node);
        AddElement(nodeView);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        // add composite nodes
        TypeCache.TypeCollection availableComposites = TypeCache.GetTypesDerivedFrom<BehaviorTreeCompositeNode>();
        evt.menu.AppendSeparator();
        foreach (Type compositeType in availableComposites)
        {
            evt.menu.AppendAction(compositeType.Name, x => CreateNode(compositeType));
            //evt.menu.AppendAction(string.Format("[{0}] {1}", compositeType.BaseType.Name, compositeType.Name), x => CreateNode(compositeType));
        }

        // add leaf nodes
        TypeCache.TypeCollection availableNodes = TypeCache.GetTypesDerivedFrom<BehaviorTreeLeaf>();
        evt.menu.AppendSeparator();
        foreach (Type leafType in availableNodes)
        {
            evt.menu.AppendAction(leafType.Name, x => CreateNode(leafType));
            //evt.menu.AppendAction(string.Format("[{0}] {1}", leafType.BaseType.Name, leafType.Name), x => CreateNode(leafType));
        }
    }
}
#endif
