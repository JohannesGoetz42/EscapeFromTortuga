using UnityEngine;

public abstract class BehaviorTreeServiceBase : EmbeddedBehaviorTreeNode
{
    public BehaviorTreeServiceBase() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "SERVICE_BASE";
        nodeName = nodeTypeName;
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
