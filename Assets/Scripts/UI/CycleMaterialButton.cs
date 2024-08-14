using UnityEngine;
using UnityEngine.UIElements;

public class CycleMaterialButton : MenuButtonBase
{
    [SerializeField] bool cycleBack;

    protected override void OnClick(ClickEvent clickEvent)
    {
        foreach (MaterialSwitch materialSwitch in FindObjectsByType<MaterialSwitch>(FindObjectsSortMode.None))
        {
            if (cycleBack)
            {

                materialSwitch.SelectPreviousMaterial();
            }
            else
            {
                materialSwitch.SelectNextMaterial();
            }
        }
    }
}
