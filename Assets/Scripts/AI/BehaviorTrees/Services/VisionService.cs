using UnityEngine;

public class VisionService : BehaviorTreeServiceBase
{
    public VisionService() : base()
    {
#if UNITY_EDITOR
        nodeName = "Vision";
#endif
    }
}
