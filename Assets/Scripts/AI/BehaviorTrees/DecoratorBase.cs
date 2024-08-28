
using UnityEngine;

public abstract class DecoratorBase : EmbeddedBehaviorTreeNode
{
#if UNITY_EDITOR
    public delegate void UpdateDecoratorDetailsDelegate();
    public UpdateDecoratorDetailsDelegate updateDetails;
#endif

    public DecoratorBase() : base()
    {
#if UNITY_EDITOR
        nodeName = "DECORATOR_BASE";
#endif
    }

    public bool abortActive = false;

    public virtual bool Evaluate(Blackboard blackboard) => false;
}
