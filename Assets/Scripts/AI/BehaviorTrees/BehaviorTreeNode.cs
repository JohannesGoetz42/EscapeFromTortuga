
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
    public List<DecoratorBase> decorators = new List<DecoratorBase>();
    public List<BehaviorTreeServiceBase> services = new List<BehaviorTreeServiceBase>();

    virtual public bool CanEnterNode(IBehaviorTreeUser user)
    {
        foreach (DecoratorBase decorator in decorators)
        {
            if (!decorator.Evaluate(user.GetBlackboard()))
            {
                return false;
            }
        }

        return true;
    }

    virtual public bool CanStayActive(IBehaviorTreeUser user)
    {
        foreach (DecoratorBase decorator in decorators)
        {
            if (decorator.abortActive && !decorator.Evaluate(user.GetBlackboard())) { return false; }
        }

        return parent.CanStayActive(user);
    }

    public virtual BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user) => null;

    /** Called by the child when it exits active state */
    virtual public void OnChildExit(IBehaviorTreeUser user, BehaviorTreeNode child, BehaviorNodeResult result)
    {
        if (parent != null)
        {
            parent.OnChildExit(user, this, result);
        }
    }

#if UNITY_EDITOR
    public Vector2 position;

    public virtual void AddChild(BehaviorTreeNodeBase child)
    {
        BehaviorTreeServiceBase service = child as BehaviorTreeServiceBase;
        if (service != null)
        {
            services.Add(service);
            service.SetParent(this);
            return;
        }

        DecoratorBase decorator = child as DecoratorBase;
        if (decorator != null)
        {
            decorators.Add(decorator);
            decorator.SetParent(this);
            return;
        }
    }

    public virtual void RemoveChild(BehaviorTreeNodeBase child) 
    {
        BehaviorTreeServiceBase service = child as BehaviorTreeServiceBase;
        if (service != null)
        {
            services.Remove(service);
            return;
        }

        DecoratorBase decorator = child as DecoratorBase;
        if (decorator != null)
        {
            decorators.Remove(decorator);
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
