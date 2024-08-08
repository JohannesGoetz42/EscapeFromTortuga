using UnityEngine;
using UnityEngine.UIElements;

public class GameTime : MonoBehaviour
{
    private float startTime = 0;
    private TextElement gameTimeText;

    private void Awake()
    {
        startTime = Time.realtimeSinceStartup;
        gameTimeText = GetComponent<UIDocument>().rootVisualElement.Q<TextElement>("GameTimeText");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameTimeText != null)
        {
            float currentTime = Time.realtimeSinceStartup - startTime;
            int minutes = Mathf.FloorToInt(currentTime / 60.0f);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            int miliseconds = Mathf.FloorToInt((currentTime % 1) * 100);
            gameTimeText.text = string.Format("CurrentTime: {0}:{1}:{2}", minutes, seconds.ToString("00"), miliseconds.ToString("00"));
        }
    }
}
