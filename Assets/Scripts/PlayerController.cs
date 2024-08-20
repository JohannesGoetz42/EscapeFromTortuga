using UnityEngine;

public class PlayerController : CharacterControllerBase
{
    public static PlayerController Instance { get; private set; }
    public float gameTime { get; private set; }
    public bool isGameOver { get; private set; }

    private bool canEscape = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        gameTime = 0.0f;
        Time.timeScale = 1.0f;
        base.Start();
    }

    public void TryEscape()
    {
        if (!canEscape)
        {
            return;
        }

        Time.timeScale = 0.0f;
        GUIManager.Instance.SetGuiMode(GUIMode.Escaped);
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
    protected void Update()
    {
        if (isGameOver)
        {
            return;
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        HandleMovement();
        gameTime += Time.deltaTime;
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movementDirection = Vector3.Normalize(new Vector3(horizontalInput, 0.0f, verticalInput));
        HandleMovement(movementDirection);
    }

    public void GameOver()
    {
        isGameOver = true;
        GUIManager.Instance.SetGuiMode(GUIMode.GameOver);
    }
}
