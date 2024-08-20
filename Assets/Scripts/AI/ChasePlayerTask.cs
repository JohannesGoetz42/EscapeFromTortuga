using UnityEngine;

public class ChasePlayerTask : BehaviorTask
{
    public override bool CanActivate() => true;

    public override void Activate()
    {
        controller.movementTarget = PlayerController.Instance.transform;
        controller.isSprinting = true;

    }
    public override void Deactivate()
    {
        controller.movementTarget = null;
        controller.isSprinting = false;
        base.Deactivate();
    }
}
