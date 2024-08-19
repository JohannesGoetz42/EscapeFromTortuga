using UnityEngine;

public class ChasePlayerTask : BehaviorTask
{
    public override bool CanActivate() => true;

    public override void Activate()
    {
        SetMovementTarget(PlayerController.Instance.transform);
    }
    public override void Deactivate()
    {
        SetMovementTarget(null);
        base.Deactivate();
    }
}
