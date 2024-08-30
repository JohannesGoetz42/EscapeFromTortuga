using UnityEngine;
using System.Linq;
using System;



#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BehaviorTree", menuName = "Scriptable Objects/Behavior/BehaviorTree")]
public class BehaviorTree : ScriptableObject
{
    public BehaviorTreeRoot root;
    public Blackboard blackboard;

#if UNITY_EDITOR
    public List<BehaviorTreeNode> nodes = new List<BehaviorTreeNode>();
    public List<EmbeddedBehaviorTreeNode> embeddedNodes = new List<EmbeddedBehaviorTreeNode>();

    public BehaviorTreeNodeBase CreateNode(System.Type type)
    {
        BehaviorTreeNodeBase node = CreateInstance(type) as BehaviorTreeNodeBase;
        node.name = type.Name;
        node.id = GUID.Generate();
        node.behaviorTree = this;

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
        // remove node as standalone node
        BehaviorTreeNode standaloneNode = node as BehaviorTreeNode;
        if (standaloneNode != null)
        {
            // remove services and decorators
            foreach (EmbeddedBehaviorTreeNode service in standaloneNode.services.ToArray())
            {
                DeleteNode(service);
            }
            foreach (EmbeddedBehaviorTreeNode decorator in standaloneNode.decorators.ToArray())
            {
                DeleteNode(decorator);
            }

            nodes.Remove(standaloneNode);
        }

        // remove decorator node
        EmbeddedBehaviorTreeNode embeddedNode = node as EmbeddedBehaviorTreeNode;
        if (embeddedNode != null)
        {
            // remove embedded node from parent
            BehaviorTreeServiceBase service = node as BehaviorTreeServiceBase;
            if (service != null)
            {
                embeddedNode.parent.services.Remove(service);
            }
            DecoratorBase decorator = node as DecoratorBase;
            if (decorator != null)
            {
                embeddedNode.parent.decorators.Remove(decorator);
            }

            embeddedNodes.Remove(embeddedNode);
        }

        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

#endif
}
