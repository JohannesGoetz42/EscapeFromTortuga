using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartGameButton : MenuButtonBase
{
    StartGameButton() : base()
    {
        buttonName = "StartGameButton";
    }

    protected override void OnClick(ClickEvent clickEvent)
    {
        base.OnClick(clickEvent);

        SceneManager.LoadScene(Constants.gameScene);
    }
}
