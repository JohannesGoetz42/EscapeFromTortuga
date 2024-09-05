using UnityEngine;

// TODO: remove this
public class ChasePlayerTask : BehaviorTask
{
    public override bool CanActivate() => true;

    public override void Activate()
    {
        //controller._movementTarget = PlayerController.Instance.transform.position;
        controller.wantsToSprint = true;

    }
    public override void Deactivate()
    {
        controller.wantsToSprint = false;
        base.Deactivate();
    }
}
