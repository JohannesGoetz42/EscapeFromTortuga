using UnityEngine;

public class CharacterControllerBase : MonoBehaviour
{
    public bool isSprinting = false;

    protected Animator animator;

    [SerializeField]
    protected float movementSpeed = 5.0f;
    [SerializeField]
    protected float sprintSpeed = 10.0f;
    [SerializeField]
    private float maxStamina = 20.0f;
    [SerializeField]
    /** The amount of stamina recovered per second while not sprinting */
    private float staminaRecoveryRate = 2.0f;

    public float CurrentStamina { get; private set; }
    public float MaxStamina { get => maxStamina; }

    protected virtual void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        CurrentStamina = maxStamina;
    }

    protected void HandleMovement(Vector3 direction)
    {
        float speed = 0.0f;
        if (direction != Vector3.zero)
        {
            speed = movementSpeed;

            // if sprinting reduce current stamina and set sprint speed
            if (isSprinting)
            {
                CurrentStamina = Mathf.Clamp(CurrentStamina - Time.deltaTime, 0.0f, maxStamina);
                if (CurrentStamina > 0)
                {
                    speed = sprintSpeed;
                }
            }
            // if not sprinting, recover stamina
            else
            {
                CurrentStamina = Mathf.Clamp(CurrentStamina + Time.deltaTime * staminaRecoveryRate, 0.0f, maxStamina);
            }

            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction * Time.deltaTime * speed;
        }

        if (animator != null)
        {
            animator.SetFloat("MovementSpeed", speed);
        }
    }
}
