using System.Collections.Generic;
using UnityEngine;

class SetViewConeModeMemory
{
    internal ViewCone viewCone;
}

public class SetViewConeModeAction : BehaviorTreeAction
{
    [SerializeField]
    ViewConeMode viewConeMode;

    private Dictionary<IBehaviorTreeUser, SetViewConeModeMemory> _memory = new Dictionary<IBehaviorTreeUser, SetViewConeModeMemory>();

    public SetViewConeModeAction() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Set view cone mode";
        nodeName = nodeTypeName;
#endif
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        SetViewConeModeMemory myMemory;
        if (!_memory.ContainsKey(user))
        {
            ViewCone viewCone = user.GetBehaviorUser().GetComponentInChildren<ViewCone>();
            if (viewCone == null)
            {
                Debug.LogWarning(string.Format("{0} could not find ViewCone on {1}",
                    nameof(SetViewConeModeAction), user.GetBehaviorUser().name));
                Exit(user, BehaviorNodeState.Failed);
                return;
            }

            myMemory = new SetViewConeModeMemory();
            myMemory.viewCone = viewCone;
            _memory.Add(user, myMemory);
        }
        else
        {
            myMemory = _memory[user];
        }

        myMemory.viewCone.SetViewConeMode(viewConeMode);
        Exit(user, BehaviorNodeState.Success);
    }
}
