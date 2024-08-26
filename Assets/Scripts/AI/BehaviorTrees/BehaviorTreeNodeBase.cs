using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BehaviorTreeNodeBase : ScriptableObject
{
    virtual protected BehaviorTreeRoot GetRoot() => parent.GetRoot();
    public BehaviorTreeNode parent;

#if UNITY_EDITOR
    public string nodeName;
    public GUID id;

    public void SetParent(BehaviorTreeNode newParent)
    {
        parent = newParent;
    }
#endif
}
