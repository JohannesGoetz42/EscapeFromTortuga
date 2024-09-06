
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BehaviorTreeNode : BehaviorTreeNodeBase
{
    public List<BehaviorTreeServiceBase> services = new List<BehaviorTreeServiceBase>();
    public List<DecoratorBase> decorators = new List<DecoratorBase>();

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        foreach (BehaviorTreeServiceBase service in services)
        {
            service.BecomeRelevant(user);
        }
        foreach (DecoratorBase decorator in decorators)
        {
            decorator.BecomeRelevant(user);
        }
    }

    protected override void OnCeaseRelevant(IBehaviorTreeUser user)
    {
        foreach (BehaviorTreeServiceBase service in services)
        {
            service.CeaseRelevant(user);
        }
        foreach (DecoratorBase decorator in decorators)
        {
            decorator.CeaseRelevant(user);
        }
    }

    virtual public bool CanEnterNode(IBehaviorTreeUser user)
    {
        foreach (DecoratorBase decorator in decorators)
        {
            if (!decorator.Evaluate(user))
            {
#if UNITY_EDITOR
                // set failed state in editor to so the editor can set the correct highlight
                if (decorator.evaluated != null)
                    decorator.evaluated(BehaviorNodeState.Failed);
#endif
                return false;
            }
        }

        return true;
    }

    virtual public bool CanStayActive(IBehaviorTreeUser user)
    {
        foreach (DecoratorBase decorator in decorators)
        {
            if (decorator.abortActive && !decorator.Evaluate(user))
            {
#if UNITY_EDITOR
                // set failed state in editor to so the editor can set the correct highlight
                if (decorator.evaluated != null)
                    decorator.evaluated(BehaviorNodeState.Failed);
#endif
                return false;
            }
        }

        return parent.CanStayActive(user);
    }

    internal virtual BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user) => null;

    /** Called by the child when it exits active state */
    internal abstract void OnChildExit(IBehaviorTreeUser user, BehaviorTreeNode child, BehaviorNodeState result);

    internal virtual void UpdateNode(IBehaviorTreeUser user)
    {
        // to keep the correct order, update ancestors first
        if (parent != null)
        {
            parent.UpdateNode(user);
        }

        foreach (BehaviorTreeServiceBase service in services)
        {
            service.TryUpdateService(user);
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
