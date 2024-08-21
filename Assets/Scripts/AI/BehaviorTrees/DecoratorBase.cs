
public abstract class DecoratorBase
{
    public bool abortActive = false;

    public virtual bool Evaluate() => false;
}
