using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class NPCController : CharacterControllerBase, INPCController
{
    [field: SerializeField]
    public PartolPoint[] patrolPoints { get; private set; }

#if UNITY_EDITOR
    [SerializeField]
    Color patrolPathColor = Color.blue;
#endif

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
    public Vector3 GetMovementTarget() => _movementTarget;

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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Count() < 2)
        {
            return;
        }

        Gizmos.color = patrolPathColor;
        for (int i = 1; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] != null && patrolPoints[i - 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[i - 1].transform.position + Vector3.up, patrolPoints[i].transform.position + Vector3.up);
            }
        }

        if (patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
        {
            Gizmos.DrawLine(patrolPoints[0].transform.position + Vector3.up, patrolPoints[patrolPoints.Count() - 1].transform.position + Vector3.up);
            Gizmos.DrawLine(patrolPoints[0].transform.position + Vector3.up, transform.position + Vector3.up);
        }
    }
#endif
}
