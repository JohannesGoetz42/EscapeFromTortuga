using UnityEngine;

public abstract class BehaviorTask : MonoBehaviour
{
    private void Start()
    {
        // behavior tasks start disabled since only the active state should be enabled
        enabled = false;
    }

    virtual public bool CanActivate() => false;
    virtual public void Activate() => enabled = true;
    virtual public void Deactivate() => enabled = false;

    protected void SetMovementTarget(Transform newTarget)
    {
        NPCController controller = GetComponent<NPCController>();
        if (controller != null)
        {
            controller.movementTarget = newTarget;
        }
    }
}
