using UnityEngine;

public class ClearBlackboardValue : BehaviorTreeAction
{
    [field: SerializeField]
    public BlackboardKeySelector valueToClear { get; set; }
    [SerializeField]
    /** if value is a bool, set it to true instead of false */
    bool setBoolTrue;

    public ClearBlackboardValue() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Clear blackboard value";
        nodeName = nodeTypeName;
#endif
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        switch (valueToClear.type)
        {
            case BlackboardValueType.Bool:
                user.GetBlackboard().SetValueAsBool(valueToClear.selectedKey, setBoolTrue);
                break;
            case BlackboardValueType.Float:
                user.GetBlackboard().SetValueAsFloat(valueToClear.selectedKey, 0.0f);
                break;
            case BlackboardValueType.Vector3:
                user.GetBlackboard().SetValueAsVector3(valueToClear.selectedKey, Vector3.zero);
                break;
            case BlackboardValueType.Object:
                user.GetBlackboard().SetValueAsObject(valueToClear.selectedKey, null);
                break;
            default:
                Exit(user, BehaviorNodeState.Failed);
                return;
        }

        Exit(user, BehaviorNodeState.Success);
    }
}