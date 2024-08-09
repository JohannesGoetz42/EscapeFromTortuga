using UnityEngine;
using UnityEngine.UIElements;

public class GameTime : MonoBehaviour
{
    private TextElement gameTimeText;
    private string textFormat = string.Empty;

    private void Awake()
    {
        gameTimeText = GetComponent<UIDocument>().rootVisualElement.Q<TextElement>("GameTimeText");
        if (gameTimeText != null)
        {
            textFormat = gameTimeText.text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameTimeText != null)
        {
            float gameTime = PlayerController.Instance.gameTime;
            int minutes = Mathf.FloorToInt(gameTime / 60.0f);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            int miliseconds = Mathf.FloorToInt((gameTime % 1) * 100);
            gameTimeText.text = string.Format(textFormat, minutes, seconds.ToString("00"), miliseconds.ToString("00"));
        }
    }
}
