using UnityEngine;
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
        Debug.Log("Starting Game");
        SceneManager.LoadScene(Constants.gameScene);
    }
}
