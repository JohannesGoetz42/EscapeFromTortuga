using System.Collections.Generic;
using UnityEngine;

public class WaitTimeAction : BehaviorTreeAction
{
    [field: SerializeField]
    public BlackboardKeySelector blackboardTime {  get; set; }
    [SerializeField]
    bool useBlackboardTime = false;
    [SerializeField]
    float WaitForSeconds = 5.0f;

    Dictionary<IBehaviorTreeUser, float> _endTimesMemory = new Dictionary<IBehaviorTreeUser, float>();

    public WaitTimeAction() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Wait time";
        nodeName = nodeTypeName;
#endif

        blackboardTime = new BlackboardKeySelector(BlackboardValueType.Float, new BlackboardValueType[] { BlackboardValueType.Float });
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        float waitTime = useBlackboardTime ? user.GetBlackboard().GetValueAsFloat(blackboardTime.selectedKey) : WaitForSeconds;
        if (_endTimesMemory.ContainsKey(user))
        {
            _endTimesMemory[user] = Time.time + waitTime;
        }
        else
        {
            _endTimesMemory.Add(user, Time.time + waitTime);
        }
    }

    protected override void OnUpdate(IBehaviorTreeUser user)
    {
        if (Time.time > _endTimesMemory[user])
        {
            Exit(user, BehaviorNodeState.Success);
        }
    }
}
