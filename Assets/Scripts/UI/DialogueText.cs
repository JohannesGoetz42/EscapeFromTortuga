using UnityEngine;
using UnityEngine.UI;

public class DialogueText : MonoBehaviour
{
    [SerializeField]
    float visibleDuration = 10.0f;
    [SerializeField]
    Text dialogueText;
    float _currentDuration = 0.0f;

    public void SetText(string text)
    {
        if (dialogueText != null)
        {
            gameObject.SetActive(true);
            enabled = true;
            dialogueText.text = text;
            _currentDuration = 0.0f;
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
