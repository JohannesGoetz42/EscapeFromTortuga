using UnityEngine;
using UnityEngine.UIElements;

public class GameTaskText : MonoBehaviour
{
    public static GameTaskText Instance { get; private set; }
    TextElement _taskText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        if (document != null)
        {
            _taskText = document.rootVisualElement.Q("taskText") as TextElement;
        }
    }

    public void SetText(string text)
    {
        if (_taskText != null)
        {
            _taskText.text = text;
        }
    }
}
