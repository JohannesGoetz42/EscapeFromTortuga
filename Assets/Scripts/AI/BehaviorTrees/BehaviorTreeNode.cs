
using System;
using UnityEditor;
using UnityEngine;

public enum BehaviorNodeResult
{
    Success,
    Abort,
    Error
}

public abstract class BehaviorTreeNode : ScriptableObject
{

    public DecoratorBase[] Decorators;
    //public DecoratorBase[] Decorators = new DecoratorBase[0];
    public BehaviorTreeNode parent;
    virtual protected BehaviorTreeRoot GetRoot() => parent.GetRoot();

    virtual public bool CanEnterNode()
    {
        foreach (DecoratorBase decorator in Decorators)
        {
            if (!decorator.Evaluate())
            {
                return false;
            }
        }

        return true;
    }

    virtual public bool CanStayActive()
    {
        foreach (DecoratorBase decorator in Decorators)
        {
            if (decorator.abortActive && !decorator.Evaluate()) { return false; }
        }

        return parent.CanStayActive();
    }

    public virtual BehaviorTreeLeaf TryGetFirstActivateableLeaf() => null;

    /** Called by the child when it exits active state */
    virtual public void OnChildExit(BehaviorTreeNode child, BehaviorNodeResult result)
    {
        if (parent != null)
        {
            parent.OnChildExit(this, result);
        }
    }

#if UNITY_EDITOR
    public string nodeName;
    public GUID id;
    public Vector2 position;

    public void SetParent(BehaviorTreeNode newParent)
    {
        parent = newParent;
    }

    public virtual void AddChild(BehaviorTreeNode child)
    {
        Debug.LogError("Tried to add child to invalid behavior node parent!");
    }

    public virtual void RemoveChild(BehaviorTreeNode node) { }

    public bool IsAncestorOf(BehaviorTreeNode node)
    {
        BehaviorTreeNode ancestor = node.parent;
        while (ancestor != null)
        {
            if (ancestor == this)
            {
                return true;
            }

            ancestor = ancestor.parent;
        }

        return false;
    }

    public bool IsDescendantOf(BehaviorTreeNode node)
    {
        BehaviorTreeNode ancestor = parent;
        while (ancestor != null)
        {
            if (ancestor == node)
            {
                return true;
            }

            ancestor = ancestor.parent;
        }

        return false;
    }
#endif
}
