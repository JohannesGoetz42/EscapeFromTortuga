using System.Collections.Generic;
using UnityEngine;

public class PatrolTask : BehaviorTask
{
    public bool shouldPatrolRandom = false;

    [SerializeField]
    List<Transform> patrolPoints = new List<Transform>();
    [SerializeField]
    /** the radius at which the current patrol point is considered reached */
    float AcceptanceRadius = 2.0f;

    private int selectedPatrolPoint;

    private bool HasReachedPatrolPoint() => (patrolPoints[selectedPatrolPoint].transform.position - transform.position).sqrMagnitude < Mathf.Pow(AcceptanceRadius, 2);
    public override bool CanActivate() => patrolPoints.Count > 1;

    public override void Activate()
    {
        PickNextPatrolPoint();
        base.Activate();
    }

    public override void Deactivate()
    {
        controller.movementTarget = null;
        base.Deactivate();
    }

    private void Update()
    {
        // if the current patrol point is reached, select a new one
        if (HasReachedPatrolPoint())
        {
            PickNextPatrolPoint();
        }
    }

    void PickNextPatrolPoint()
    {
        if (shouldPatrolRandom)
        {
            // reduce available points by one and increase index if higher current index to avoid selection of currently selected point
            int newSelection = Random.Range(0, patrolPoints.Count - 1);
            if (newSelection >= selectedPatrolPoint)
            {
                newSelection++;
            }

            selectedPatrolPoint = newSelection;
        }
        else
        {
            selectedPatrolPoint = (selectedPatrolPoint + 1) % patrolPoints.Count;
        }

        controller.movementTarget = patrolPoints[selectedPatrolPoint];
    }
}
