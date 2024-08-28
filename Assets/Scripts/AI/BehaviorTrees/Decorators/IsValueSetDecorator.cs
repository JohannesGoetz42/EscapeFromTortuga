using UnityEngine;

#if UNITY_EDITOR
public enum BlackboardValueType
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
        nodeName = string.Format("Is value set: {0}", comparedBlackboardValue.selectedKey);
#endif
    }

#if UNITY_EDITOR
    [SerializeField]
    BlackboardValueType _valueType = BlackboardValueType.Bool;

    public BlackboardValueType ValueType
    {
        get => _valueType;
        set
        {
            switch (value)
            {
                case BlackboardValueType.Bool:
                    comparedBlackboardValue.type = typeof(bool);
                    break;
                case BlackboardValueType.Object:
                    comparedBlackboardValue.type = typeof(Object);
                    break;
                default:
                    Debug.LogError("Selected unsupported blackboard type for IsValueSetDecorator!");
                    return;

            }
            _valueType = value;
            updateDetails.Invoke();
        }
    }
#endif

    [SerializeField]
    BlackboardKeySelector comparedBlackboardValue;
    [SerializeField]
    /** 
     * if set the evaluated result is inverted
     * It will be evaluated to true if the values are not equal
     */
    bool bInvertResult;


    public override bool Evaluate(Blackboard blackboard)
    {
        bool result = false;
        if (comparedBlackboardValue.type == typeof(bool))
        {
            result = blackboard.GetValueAsBool(comparedBlackboardValue.selectedKey);
        }
        else if (comparedBlackboardValue.type == typeof(Object))
        {
            result = blackboard.GetValueAsObject(comparedBlackboardValue.selectedKey);
        }

        return bInvertResult ? !result : result;
    }
}
