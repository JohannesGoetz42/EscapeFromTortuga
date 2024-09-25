using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameTaskText : MonoBehaviour
{
    public static GameTaskText Instance { get; private set; }
    TextElement _taskText;
    string _currentText = "You are wanted!\r\nFind a way to escape the town";
    [SerializeField, TextArea(10, 100)]
    string isChasedText = "You are currently being chased!\r\nLoose your pursuers";

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
            if (_taskText == null)
            {
                return;
            }
        }

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.playerChasedChanged += UpdatePlayerChased;
        }
    }

    private void UpdatePlayerChased()
    {
        if (PlayerController.Instance.IsChased())
        {
            _taskText.text = isChasedText;
            _taskText.AddToClassList("warning");
        }
        else
        {
            _taskText.text = _currentText;
            _taskText.RemoveFromClassList("warning");
        }
    }

    public void SetText(string text)
    {
        _currentText = text;
        if (!PlayerController.Instance.IsChased() && _taskText != null)
        {
            _taskText.text = text;
        }
    }
}
