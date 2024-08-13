using UnityEngine;

public class CharacterController : MonoBehaviour
{
    protected Animator animator;

    [SerializeField]
    protected float movementSpeed = 10.0f;

    protected float currentSpeed;

    protected virtual void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("MovementSpeed", currentSpeed);
        }
    }
}
