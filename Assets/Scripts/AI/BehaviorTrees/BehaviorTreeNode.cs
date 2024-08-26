
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BehaviorNodeResult
{
    Success,
    Abort,
    Error
}

public abstract class BehaviorTreeNode : BehaviorTreeNodeBase
{
    public List<DecoratorBase> Decorators = new List<DecoratorBase>();
    public List<BehaviorTreeServiceBase> Services = new List<BehaviorTreeServiceBase>();

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

    public virtual BehaviorTreeAction TryGetFirstActivateableAction() => null;

    /** Called by the child when it exits active state */
    virtual public void OnChildExit(BehaviorTreeNode child, BehaviorNodeResult result)
    {
        if (parent != null)
        {
            parent.OnChildExit(this, result);
        }
    }

#if UNITY_EDITOR
    public Vector2 position;

    public virtual void AddChild(BehaviorTreeNodeBase child)
    {
        BehaviorTreeServiceBase service = child as BehaviorTreeServiceBase;
        if (service != null)
        {
            Services.Add(service);
            return;
        }

        DecoratorBase decorator = child as DecoratorBase;
        if (decorator != null)
        {
            Decorators.Add(decorator);
            return;
        }
    }

    public virtual void RemoveChild(BehaviorTreeNodeBase child) 
    {
        BehaviorTreeServiceBase service = child as BehaviorTreeServiceBase;
        if (service != null)
        {
            Services.Remove(service);
            return;
        }

        DecoratorBase decorator = child as DecoratorBase;
        if (decorator != null)
        {
            Decorators.Remove(decorator);
            return;
        }
    }

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
