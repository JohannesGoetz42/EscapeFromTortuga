using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartGameButton : MonoBehaviour
{
    private Button _startGameButton;

    private void Awake()
    {
        UIDocument document = GetComponent<UIDocument>();
        if (document == null)
        {
            return;
        }

        _startGameButton = document.rootVisualElement.Q("StartGameButton") as Button;

        if (_startGameButton == null)
        {
            Debug.Log("script requires a Button named 'StartGameButton'!");
            return;
        }
        _startGameButton.RegisterCallback<ClickEvent>(OnStartGameClick);
    }

    private void OnDisable()
    {
        if (_startGameButton == null)
        {
            return;
        }

        _startGameButton.UnregisterCallback<ClickEvent>(OnStartGameClick);
    }

    void OnStartGameClick(ClickEvent clickEvent)
    {
        Debug.Log("Starting Game");
        SceneManager.LoadScene(Constants.gameScene);
    }
}
