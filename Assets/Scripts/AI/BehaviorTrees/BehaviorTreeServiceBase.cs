using UnityEngine;

public class BehaviorTreeServiceBase : EmbeddedBehaviorTreeNode
{
    public BehaviorTreeServiceBase() : base()
    {
#if UNITY_EDITOR
        nodeName = "SERVICE_BASE";
#endif
    }
}
