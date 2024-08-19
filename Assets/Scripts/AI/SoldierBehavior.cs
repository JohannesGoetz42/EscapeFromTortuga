using System.Linq;
using UnityEngine;

public enum SoldierBehavorState
{
    Idle,
    Patrolling,
    Searching,
    Chasing
}

[System.Serializable]
public struct SoldierBehaviorTask
{
    public SoldierBehavorState state;
    public BehaviorTask task;
}

public class SoldierBehavior : MonoBehaviour
{
    [SerializeField]
    SoldierBehaviorTask[] behaviorTasks = new SoldierBehaviorTask[0];

    private Vision vision;
    private BehaviorTask currentTask;
    private SoldierBehavorState currentState = SoldierBehavorState.Idle;

    public bool TrySetBehaviorState(SoldierBehavorState newState)
    {
        // find the first task for the selected state that can be activated
        SoldierBehaviorTask task = behaviorTasks.FirstOrDefault(x => x.state == newState &&
            x.task != null && x.task != currentTask && x.task.CanActivate());
        if (task.task == null)
        {
            return false;
        }

        if (currentTask != null)
        {
            currentTask.Deactivate();
        }

        currentTask = task.task;
        currentState = newState;
        task.task.Activate();
        return true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vision = GetComponent<Vision>();
        if (vision == null)
        {
            Debug.LogWarning(string.Format("SoldierBehavior on {0] has no vision and will be inactive!", gameObject.name));
            enabled = false;
            return;
        }

        TrySetBehaviorState(SoldierBehavorState.Patrolling);
    }
}
