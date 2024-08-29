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
        nodeName = string.Format("Is value set: {0}", comparedBlackboardValue.selectedKey);
#endif
    }

#if UNITY_EDITOR
    [SerializeField]
    ValueType valueType = ValueType.Bool;
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

#if UNITY_EDITOR
    public override string[] GetRefreshProperties()
    {
        return new string[] { nameof(valueType) };
    }

    public override void OnPropertyChanged(SerializedProperty property)
    {
        if (property == null)
        {
            return;
        }

        if (property.name == nameof(valueType))
        {
            switch (valueType)
            {
                case ValueType.Bool:
                    comparedBlackboardValue.type = BlackboardValueType.Bool;
                    break;
                case ValueType.Object:
                    comparedBlackboardValue.type = BlackboardValueType.Object;
                    break;
                default:
                    Debug.LogError("Selected unsupported blackboard type for IsValueSetDecorator!");
                    return;

            }
        }
    }

#endif
}
