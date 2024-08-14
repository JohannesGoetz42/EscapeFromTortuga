using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BackToMenuButton : MenuButtonBase
{
    BackToMenuButton() : base()
    {
        buttonName = "BackToMenuButton";
    }

    protected override void OnClick(ClickEvent clickEvent)
    {
        SceneManager.LoadScene(Constants.mainMenu);
    }
}
