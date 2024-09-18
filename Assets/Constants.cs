using UnityEngine;

public static class Constants
{
    public const string mainMenu = "MainMenu";
    public const string gameScene = "Tortuga";
    // layer 3: unwalkable or layer 8: player
    public const int npcViewLayer = 1 << 3 | 1 << 8;
}