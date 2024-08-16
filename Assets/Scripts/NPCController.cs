using UnityEngine;

public class NPCController : CharacterControllerBase
{
    public Vector3 movementDirection;

    protected void LateUpdate()
    {
        if (!PlayerController.Instance.isGameOver)
        {
            HandleMovement(movementDirection);
            movementDirection = Vector3.zero;
        }
    }
}
