
using UnityEngine;

public abstract class DecoratorBase : EmbeddedBehaviorTreeNode
{
    public DecoratorBase() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "DECORATOR_BASE";
        nodeName = nodeTypeName;
#endif
    }

    public bool abortActive = false;

    public virtual bool Evaluate(IBehaviorTreeUser user) => false;
}
