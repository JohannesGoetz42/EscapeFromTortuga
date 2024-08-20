using UnityEngine;

public class CharacterControllerBase : MonoBehaviour
{
    public bool isSprinting = false;

    protected Animator animator;

    [SerializeField]
    protected float movementSpeed = 5.0f;
    [SerializeField]
    protected float sprintSpeed = 10.0f;

    protected virtual void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    protected void HandleMovement(Vector3 direction)
    {
        float speed = 0.0f;
        if (direction != Vector3.zero)
        {
            speed = isSprinting ? sprintSpeed : movementSpeed;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction * Time.deltaTime * speed;
        }

        if (animator != null)
        {
            animator.SetFloat("MovementSpeed", speed);
        }
    }
}
