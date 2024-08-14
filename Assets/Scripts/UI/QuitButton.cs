using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitButton : MenuButtonBase
{
    QuitButton() : base()
    {
        buttonName = "QuitButton";
    }

    protected override void OnClick(ClickEvent clickEvent)
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
