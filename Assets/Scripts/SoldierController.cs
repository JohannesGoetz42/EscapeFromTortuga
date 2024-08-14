using UnityEngine;

public class SoldierController : CharacterController
{    
    protected override void Update()
    {
        currentSpeed = movementSpeed;

        Vector3 direction = (PlayerController.Instance.gameObject.transform.position - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(direction);
        transform.position += direction * movementSpeed * Time.deltaTime;

        base.Update();
    }
}
