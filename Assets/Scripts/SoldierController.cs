using UnityEngine;

public class SoldierController : CharacterControllerBase
{
    protected override void Update()
    {
        if (PlayerController.Instance.isGameOver)
        {
            currentSpeed = 0.0f;
        }
        else
        {
            currentSpeed = movementSpeed;

            Vector3 direction = (PlayerController.Instance.gameObject.transform.position - transform.position).normalized;

            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction * movementSpeed * Time.deltaTime;
        }

        base.Update();
    }
}
