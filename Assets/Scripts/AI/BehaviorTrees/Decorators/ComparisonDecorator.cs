using UnityEngine;

public enum ComparisonType
{
    Equal,
    NotEqual,
    GreatherThan,
    GreatherEqual,
    LessThan,
    LessEqual

}

public class ComparisonDecorator : DecoratorBase
{
    [SerializeField]
    ComparisonType comparisonType = ComparisonType.Equal;
    [field: SerializeField]
    public BlackboardKeySelector leftValue { get; set; }
    [SerializeField]
    /** The value that is compared to (right side) as float */
    float comparedValue;

    public ComparisonDecorator() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = string.Format("Comparison");
        nodeName = nodeTypeName;
#endif
        leftValue = new BlackboardKeySelector(BlackboardValueType.Float, new BlackboardValueType[] { BlackboardValueType.Float, BlackboardValueType.Enum });
    }

    public override bool Evaluate(IBehaviorTreeUser user)
    {
        float value = 0.0f;
        if (leftValue.type == BlackboardValueType.Float)
        {
            value = user.GetBlackboard().GetValueAsFloat(leftValue.selectedKey);
        }
        else if (leftValue.type == BlackboardValueType.Enum)
        {
            value = (float)user.GetBlackboard().GetValueAsEnum(leftValue.selectedKey);
        }

        switch (comparisonType)
        {
            case ComparisonType.Equal:
                return value == comparedValue;
            case ComparisonType.NotEqual:
                return value != comparedValue;
            case ComparisonType.GreatherThan:
                return value > comparedValue;
            case ComparisonType.GreatherEqual:
                return value >= comparedValue;
            case ComparisonType.LessThan:
                return value < comparedValue;
            case ComparisonType.LessEqual:
                return value <= comparedValue;
            default:
                return true;
        }

    }
}
