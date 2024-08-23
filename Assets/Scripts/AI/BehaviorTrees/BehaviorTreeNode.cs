
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
    public DecoratorBase[] Decorators = new DecoratorBase[0];
    protected BehaviorTreeNode parent;
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
#endif
}
