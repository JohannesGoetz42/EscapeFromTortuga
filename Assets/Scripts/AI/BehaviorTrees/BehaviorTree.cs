using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BehaviorTree", menuName = "Scriptable Objects/Behavior/BehaviorTree")]
public class BehaviorTree : ScriptableObject
{
    public BehaviorTreeRoot root;
    [SerializeField] Blackboard blackboard;
    public Blackboard Blackboard { get => blackboard; }

#if UNITY_EDITOR
    public List<BehaviorTreeNode> nodes = new List<BehaviorTreeNode>();
    public List<EmbeddedBehaviorTreeNode> embeddedNodes = new List<EmbeddedBehaviorTreeNode>();

    public BehaviorTreeNodeBase CreateNode(System.Type type)
    {
        BehaviorTreeNodeBase node = CreateInstance(type) as BehaviorTreeNodeBase;
        node.name = type.Name;
        node.id = GUID.Generate();

        BehaviorTreeNode standaloneNode = node as BehaviorTreeNode;
        if (standaloneNode != null)
        {
            nodes.Add(standaloneNode);
        }

        EmbeddedBehaviorTreeNode embeddedNode = node as EmbeddedBehaviorTreeNode;
        if (embeddedNode != null)
        {
            embeddedNodes.Add(embeddedNode);
        }

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(BehaviorTreeNodeBase node)
    {

        BehaviorTreeNode standaloneNode = node as BehaviorTreeNode;
        if (standaloneNode != null)
        {
            nodes.Remove(standaloneNode);
        }

        EmbeddedBehaviorTreeNode embeddedNode = node as EmbeddedBehaviorTreeNode;
        if (embeddedNode != null)
        {
            embeddedNodes.Remove(embeddedNode);
        }

        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

#endif
}
