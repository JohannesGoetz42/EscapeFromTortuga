using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitButton : MonoBehaviour
{
    private Button _quitButton;

    private void Awake()
    {
        UIDocument document = GetComponent<UIDocument>();
        if (document == null)
        {
            return;
        }

        _quitButton = document.rootVisualElement.Q("QuitButton") as Button;
        if (_quitButton == null)
        {
            Debug.Log("script requires a Button named 'QuitButton'!");
            return;
        }

        _quitButton.RegisterCallback<ClickEvent>(OnQuitClick);
    }

    private void OnDisable()
    {
        if (_quitButton == null)
        {
            return;
        }

        _quitButton.UnregisterCallback<ClickEvent>(OnQuitClick);
    }

    void OnQuitClick(ClickEvent clickEvent)
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
