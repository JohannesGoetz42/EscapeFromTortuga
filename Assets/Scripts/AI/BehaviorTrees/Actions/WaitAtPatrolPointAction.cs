using System.Collections.Generic;
using UnityEngine;

public class WaitAtPatrolPointAction : BehaviorTreeAction
{
    [field: SerializeField]
    public BlackboardKeySelector patrolPoint { get; set; }

    Dictionary<IBehaviorTreeUser, float> _stopWaitingTimes = new Dictionary<IBehaviorTreeUser, float>();

    public WaitAtPatrolPointAction() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Wait at patrol point";
        nodeName = nodeTypeName;
#endif

        patrolPoint = new BlackboardKeySelector(BlackboardValueType.Object, new BlackboardValueType[] { BlackboardValueType.Object });
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        IHasWaitTime waitTimeSource = user.GetBlackboard().GetValueAsObject(patrolPoint.selectedKey) as IHasWaitTime;
        if (waitTimeSource == null)
        {
            Exit(user, BehaviorNodeState.Failed);
            return;
        }

        float finishWaitTime = Random.Range(waitTimeSource.MinWaitDuration, waitTimeSource.MaxWaitDuration) + Time.time;
        if (_stopWaitingTimes.ContainsKey(user))
        {
            _stopWaitingTimes[user] = finishWaitTime;
        }
        else
        {
            _stopWaitingTimes.Add(user, finishWaitTime);
        }
    }

    protected override void OnUpdate(IBehaviorTreeUser user)
    {
        if (_stopWaitingTimes[user] < Time.time)
        {
            Exit(user, BehaviorNodeState.Success);
        }
    }
}
