using UnityEngine;

public abstract class BehaviorTreeServiceBase : EmbeddedBehaviorTreeNode
{
    public BehaviorTreeServiceBase() : base()
    {
#if UNITY_EDITOR
        nodeName = "SERVICE_BASE";
#endif

    }

    public abstract void UpdateService(IBehaviorTreeUser user);
}
