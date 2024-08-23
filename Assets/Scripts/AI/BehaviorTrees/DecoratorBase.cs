
using UnityEngine;

public abstract class DecoratorBase : ScriptableObject
{
    public bool abortActive = false;

    public virtual bool Evaluate() => false;
}
