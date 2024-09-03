using System.Collections.Generic;
using UnityEngine;

struct SetViewConeModeMemory
{
    internal ViewCone viewCone;
}

public class SetViewConeModeAction : BehaviorTreeAction
{
    [SerializeField]
    ViewConeMode viewConeMode;

    private Dictionary<IBehaviorTreeUser, SetViewConeModeMemory> _memory;

    public SetViewConeModeAction() : base()
    {
#if UNITY_EDITOR
        nodeName = "Set view cone mode";
#endif
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        SetViewConeModeMemory myMemory;
        if (_memory.ContainsKey(user))
        {
            ViewCone viewCone = user.GetBehaviorUser().GetComponentInChildren<ViewCone>();
            if (viewCone == null)
            {
                Debug.LogWarning(string.Format("{0} could not find ViewCone on {1}", 
                    nameof(SetViewConeModeAction), user.GetBehaviorUser().name));
                return;
            }

            myMemory = new SetViewConeModeMemory();
            myMemory.viewCone = viewCone;
        }
        else
        {
            myMemory = _memory[user];
        }

        myMemory.viewCone.SetViewConeMode(viewConeMode);
    }
}
