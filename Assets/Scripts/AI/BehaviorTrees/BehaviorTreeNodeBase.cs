using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BehaviorTreeNodeBase : ScriptableObject
{
    public BehaviorTreeNode parent;
    public BehaviorTree behaviorTree;

#if UNITY_EDITOR
    public string nodeName;
    public GUID id;

    public void SetParent(BehaviorTreeNode newParent)
    {
        parent = newParent;
    }
#endif
}
