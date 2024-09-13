using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterControllerBase
{
    public delegate void PlayerSearchedChanged();

    public static PlayerController Instance { get; private set; }
    public float gameTime { get; private set; }
    public bool isGameOver { get; private set; }
    public Camera MainCamera { get; private set; }
    public Canvas OverlayCanvas { get; private set; }

    public PlayerSearchedChanged playerSearchedChanged;

    private HashSet<NPCController> _searchingCharacters;
    public bool IsSearched() => _searchingCharacters.Count > 0;

    public void AddSearchingCharacter(NPCController controller)
    {
        _searchingCharacters.Add(controller);
        playerSearchedChanged.Invoke();
    }

    public void RemoveSearchingCharacter(NPCController controller)
    {
        _searchingCharacters.Remove(controller);
        playerSearchedChanged.Invoke();
    }

    public void TryEscape()
    {
        if (IsSearched())
        {
            return;
        }

        Time.timeScale = 0.0f;
        GUIManager.Instance.SetGuiMode(GUIMode.Escaped);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        gameTime = 0.0f;
        Time.timeScale = 1.0f;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        OverlayCanvas = GameObject.FindGameObjectWithTag("OverlayCanvas").GetComponent<Canvas>();

        _searchingCharacters = new HashSet<NPCController>();

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
