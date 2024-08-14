using UnityEngine;
using UnityEngine.UIElements;

public class CycleModelButtons : MenuButtonBase
{
    [SerializeField] bool cycleBack;

    protected override void OnClick(ClickEvent clickEvent)
    {
        foreach (ModelSwitch modelSwitch in FindObjectsByType<ModelSwitch>(FindObjectsSortMode.None))
        {
            if (cycleBack)
            {

                modelSwitch.SelectPreviousModel();
            }
            else
            {
                modelSwitch.SelectNextModel();
            }
        }
    }
}
