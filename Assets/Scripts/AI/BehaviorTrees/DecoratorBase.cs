
using UnityEngine;

public abstract class DecoratorBase : EmbeddedBehaviorTreeNode
{
    public DecoratorBase() : base()
    {
#if UNITY_EDITOR
        nodeName = "DECORATOR_BASE";
#endif
    }

    public bool abortActive = false;

    public virtual bool Evaluate(IBehaviorTreeUser user) => false;
}
