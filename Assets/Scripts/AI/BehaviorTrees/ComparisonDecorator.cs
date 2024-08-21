using System;

public enum ComparisonType
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterEqual,
    LessThan,
    LessEqual
}

public class ComparisonDecorator<T> : DecoratorBase where T : IComparable
{
    public ComparisonType comparisonType;
    public T Value1;
    public T Value2;

    public override bool Evaluate()
    {
        switch (comparisonType)
        {
            case ComparisonType.Equal:
                return Value1.CompareTo(Value2) == 0;
            case ComparisonType.NotEqual:
                return Value1.CompareTo(Value2) != 0;
            case ComparisonType.GreaterThan:
                return Value1.CompareTo(Value2) > 0;
            case ComparisonType.GreaterEqual:
                return Value1.CompareTo(Value2) >= 0;
            case ComparisonType.LessThan:
                return Value1.CompareTo(Value2) < 0;
            case ComparisonType.LessEqual:
                return Value1.CompareTo(Value2) <= 0;
        }

        return false;
    }
}
