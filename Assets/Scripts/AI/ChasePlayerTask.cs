using UnityEngine;

public class ChasePlayerTask : BehaviorTask
{
    public override bool CanActivate() => true;

    public override void Activate()
    {
        controller.movementTarget = PlayerController.Instance.transform;
        controller.wantsToSprint = true;

    }
    public override void Deactivate()
    {
        controller.movementTarget = null;
        controller.wantsToSprint = false;
        base.Deactivate();
    }
}
