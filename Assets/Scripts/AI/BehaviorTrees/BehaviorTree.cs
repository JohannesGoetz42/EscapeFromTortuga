using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviorTree", menuName = "Scriptable Objects/BehaviorTree")]
public class BehaviorTree : ScriptableObject
{
    public BehaviorTreeRoot root = new BehaviorTreeRoot();

#if UNITY_EDITOR
    public List<BehaviorTreeNode> nodes = new List<BehaviorTreeNode>();

    public BehaviorTreeNode CreateNode(System.Type type)
    {
        BehaviorTreeNode node = CreateInstance(type) as BehaviorTreeNode;
        node.name = type.Name;
        node.id = GUID.Generate();
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(BehaviorTreeNode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
#endif
}
