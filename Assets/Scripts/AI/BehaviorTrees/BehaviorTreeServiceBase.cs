using UnityEngine;

public abstract class BehaviorTreeServiceBase : EmbeddedBehaviorTreeNode
{
    public BehaviorTreeServiceBase() : base()
    {
#if UNITY_EDITOR
        nodeName = "SERVICE_BASE";
#endif

    }

    internal bool shouldUpdate = true;

    internal void TryUpdateService(IBehaviorTreeUser user)
    {
        if (shouldUpdate)
        {
            UpdateService(user);
        }
    }

    protected abstract void UpdateService(IBehaviorTreeUser user);
}
