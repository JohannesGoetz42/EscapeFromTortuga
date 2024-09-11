using UnityEngine;
using UnityEngine.UI;

public class DialogueText : MonoBehaviour
{
    [SerializeField]
    float visibleDuration = 10.0f;
    [SerializeField]
    Text dialogueText;
    float _currentDuration = 0.0f;

    void Start()
    {
        enabled = false;
        if (dialogueText != null)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetText(string text)
    {
        if (dialogueText != null)
        {
            dialogueText.text = text;
            gameObject.SetActive(true);
            _currentDuration = 0.0f;
            enabled = true;
        }
    }

    void Update()
    {
        // hide after visible duration
        _currentDuration += Time.deltaTime;
        if (_currentDuration > visibleDuration)
        {
            gameObject.SetActive(false);
            enabled = false;
        }
    }
}
