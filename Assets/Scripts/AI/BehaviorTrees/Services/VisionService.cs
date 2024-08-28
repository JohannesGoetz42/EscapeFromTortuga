using UnityEngine;

public class VisionService : BehaviorTreeServiceBase
{
    [SerializeField] BlackboardKeySelector canSeePlayer = new BlackboardKeySelector(typeof(bool));
    [SerializeField] BlackboardKeySelector target = new BlackboardKeySelector(typeof(Object));

    public VisionService() : base()
    {
#if UNITY_EDITOR
        nodeName = "Vision";
#endif
    }
}
