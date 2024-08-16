using UnityEngine;

public class CharacterControllerBase : MonoBehaviour
{
    protected Animator animator;

    [SerializeField]
    protected float movementSpeed = 10.0f;

    protected virtual void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    protected void HandleMovement(Vector3 direction)
    {
        float speed = 0.0f;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction * Time.deltaTime * movementSpeed;
            speed = movementSpeed;
        }

        if (animator != null)
        {
            animator.SetFloat("MovementSpeed", speed);
        }
    }
}
