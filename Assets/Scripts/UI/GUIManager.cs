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
    public static GUIManager Instance { get; private set; }
    public GUIModeDocument[] modeDocuments;

    public void SetGuiMode(GUIMode newMode)
    {
        foreach (GUIModeDocument modeDocument in modeDocuments)
        {
            modeDocument.document.gameObject.SetActive(modeDocument.mode == newMode);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGuiMode(GUIMode.Default);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }
}
