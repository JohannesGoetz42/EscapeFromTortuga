using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum GUIMode
{
    Default,
    GameOver,
    Escaped
}

[Serializable]
public struct GUIModeDocument
{
    public GUIMode mode;
    public UIDocument document;
}

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance { get; private set; }
    public GUIModeDocument[] modeDocuments;

    public void SetGuiMode(GUIMode newMode)
    {
        foreach (GUIModeDocument modeDocument in modeDocuments)
        {
            modeDocument.document.rootVisualElement.visible = modeDocument.mode == newMode;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGuiMode(GUIMode.Default);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }

        instance = this;
    }
}
