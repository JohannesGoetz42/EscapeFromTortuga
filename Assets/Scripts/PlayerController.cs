using UnityEngine;

public class PlayerController : CharacterController
{
    public static PlayerController Instance { get; private set; }
    public float gameTime { get; private set; }

    private bool canEscape = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTime = 0.0f;
        Time.timeScale = 1.0f;
    }

    public void TryEscape()
    {
        if (!canEscape)
        {
            return;
        }

        Time.timeScale = 0.0f;
        GUIManager.instance.SetGuiMode(GUIMode.Escaped);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    // Update is called once per frame
    protected override void Update()
    {
        HandleMovement();
        gameTime += Time.deltaTime;

        base.Update();
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movementVector = Vector3.Normalize(new Vector3(horizontalInput, 0.0f, verticalInput));

        transform.rotation = Quaternion.LookRotation(movementVector);
        transform.position += movementVector * Time.deltaTime * movementSpeed;

        currentSpeed = movementVector == Vector3.zero ? 0.0f : movementSpeed;
    }
}
