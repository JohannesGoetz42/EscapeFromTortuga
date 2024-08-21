using UnityEngine;

public abstract class BehaviorTask : MonoBehaviour
{
    protected NPCController controller;

    private void Start()
    {
        // behavior tasks start disabled since only the active state should be enabled
        enabled = false;
        controller = GetComponent<NPCController>();
    }

    virtual public bool CanActivate() => false;
    virtual public void Activate() => enabled = true;
    virtual public void Deactivate() => enabled = false;
}
