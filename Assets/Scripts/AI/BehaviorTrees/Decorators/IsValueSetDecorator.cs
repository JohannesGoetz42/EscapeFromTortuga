using UnityEngine;

#if UNITY_EDITOR
public enum ValueType
{
    Bool,
    Object
}
#endif

public class IsValueSetDecorator : DecoratorBase
{
    public IsValueSetDecorator() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = string.Format("Is value set?");
        nodeName = nodeTypeName;
#endif

        comparedBlackboardValue = new BlackboardKeySelector(BlackboardValueType.Bool, new BlackboardValueType[] { BlackboardValueType.Bool, BlackboardValueType.Object });
    }

    [field: SerializeField]
    public BlackboardKeySelector comparedBlackboardValue { get; set; }

    [SerializeField]
    /** 
     * if set the evaluated result is inverted
     * It will be evaluated to true if the values are not equal
     */
    bool invertResult;

    public override bool Evaluate(IBehaviorTreeUser user)
    {
        bool result = false;
        if (comparedBlackboardValue.type == BlackboardValueType.Bool)
        {
            result = user.GetBlackboard().GetValueAsBool(comparedBlackboardValue.selectedKey);
        }
        else if (comparedBlackboardValue.type == BlackboardValueType.Object)
        {
            result = user.GetBlackboard().GetValueAsObject(comparedBlackboardValue.selectedKey) != null;
        }

        return invertResult ? !result : result;
    }
}
