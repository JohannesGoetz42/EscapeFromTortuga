using System;
using System.IO;
using UnityEngine;

public class NPCController : CharacterControllerBase, INPCController
{
    [field: SerializeField]
    public Transform[] patrolPoints { get; private set; }

    private Vector3 _movementTarget;
    private bool shouldMoveToTarget = false;

    public void SetMovementTarget(Vector3 newWovementTarget)
    {
        _movementTarget = newWovementTarget;
        shouldMoveToTarget = true;
    }

    public void ClearMovementTarget()
    {
        shouldMoveToTarget = false;
    }

    protected void LateUpdate()
    {
        if (PlayerController.Instance == null || PlayerController.Instance.isGameOver)
        {
            return;
        }

        Vector3 movementDirection = Vector3.zero;
        if (shouldMoveToTarget)
        {
            if ((_movementTarget - transform.position).sqrMagnitude < 0.2)
            {
                ClearMovementTarget();
            }
            else
            {
                movementDirection = (_movementTarget - transform.position).normalized;
            }
        }

        HandleMovement(movementDirection);
    }
}
