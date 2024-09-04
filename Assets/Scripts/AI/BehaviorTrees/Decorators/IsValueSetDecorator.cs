using UnityEditor;
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
        nodeName = string.Format("Is value set?");
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
    bool bInvertResult;

    public override bool Evaluate(Blackboard blackboard)
    {
        bool result = false;
        if (comparedBlackboardValue.type == BlackboardValueType.Bool)
        {
            result = blackboard.GetValueAsBool(comparedBlackboardValue.selectedKey);
        }
        else if (comparedBlackboardValue.type == BlackboardValueType.Object)
        {
            result = blackboard.GetValueAsObject(comparedBlackboardValue.selectedKey) != null;
        }

        return bInvertResult ? !result : result;
    }
}
