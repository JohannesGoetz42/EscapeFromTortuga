using UnityEngine;

public class VisionService : BehaviorTreeServiceBase
{
    [SerializeField] BlackboardKeySelector canSeePlayer = new BlackboardKeySelector(BlackboardValueType.Bool);
    [SerializeField] BlackboardKeySelector target = new BlackboardKeySelector(BlackboardValueType.Object);

    public VisionService() : base()
    {
#if UNITY_EDITOR
        nodeName = "Vision";
#endif
    }
}
