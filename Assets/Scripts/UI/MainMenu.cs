using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private UIDocument _document;
    private Button _startGameButton;
    private Button _quitButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        _startGameButton = _document.rootVisualElement.Q("StartGameButton") as Button;
        _startGameButton.RegisterCallback<ClickEvent>(OnStartGameClick);

        _quitButton = _document.rootVisualElement.Q("QuitButton") as Button;
        _quitButton.RegisterCallback<ClickEvent>(OnQuitClick);
    }

    private void OnDisable()
    {
        _startGameButton.UnregisterCallback<ClickEvent>(OnStartGameClick);
        _quitButton.UnregisterCallback<ClickEvent>(OnQuitClick);
    }

    void OnStartGameClick(ClickEvent clickEvent)
    {
        Debug.Log("Starting Game");
        SceneManager.LoadScene(Constants.gameScene);
    }

    void OnQuitClick(ClickEvent clickEvent)
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
