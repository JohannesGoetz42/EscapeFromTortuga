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
        base.OnClick(clickEvent);

        SceneManager.LoadScene(Constants.mainMenu);
    }
}
