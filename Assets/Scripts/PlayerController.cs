using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterControllerBase
{
    public delegate void PlayerSearchedChanged();

    [SerializeField]
    private float cameraDistance = 10.0f;
    [SerializeField]
    private float cameraAngle = 60.0f;

    public static PlayerController Instance { get; private set; }
    public float gameTime { get; private set; }
    public bool isGameOver { get; private set; }
    public Camera MainCamera { get; private set; }
    public Canvas OverlayCanvas { get; private set; }

    public PlayerSearchedChanged playerChasedChanged;

    private HashSet<INPCController> _chasingCharacters = new HashSet<INPCController>();
    public bool IsChased() => _chasingCharacters.Count > 0;

    public void AddChasingCharacter(INPCController controller)
    {
        if (controller == null)
        {
            return;
        }

        _chasingCharacters.Add(controller);
        playerChasedChanged.Invoke();
    }

    public void RemoveChasingCharacter(INPCController controller)
    {
        if (controller == null)
        {
            return;
        }

        _chasingCharacters.Remove(controller);
        playerChasedChanged.Invoke();
    }

    public void TryEscape()
    {
        if (IsChased())
        {
            return;
        }

        Time.timeScale = 0.0f;
        GUIManager.Instance.SetGuiMode(GUIMode.Escaped);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        GameObject spawnLocation = SpawnLocation.GetRandomSpawnLocation(SpawnLocationType.Player);
        if (spawnLocation != null)
        {
            transform.position = spawnLocation.transform.position;
            transform.rotation = spawnLocation.transform.rotation;
        }

        gameTime = 0.0f;
        Time.timeScale = 1.0f;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // position the camera based on camera settings
        if (MainCamera != null)
        {
            Quaternion cameraRotation = Quaternion.Euler(cameraAngle, 0.0f, 0.0f);
            Vector3 cameraOffset = Vector3.up * cameraDistance;
            cameraOffset = Quaternion.Inverse(cameraRotation) * cameraOffset;
            MainCamera.transform.position = transform.position + cameraOffset;
            MainCamera.transform.rotation = cameraRotation;
        }

        OverlayCanvas = GameObject.FindGameObjectWithTag("OverlayCanvas").GetComponent<Canvas>();

        base.Start();
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

        wantsToSprint = Input.GetKey(KeyCode.LeftShift);

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
