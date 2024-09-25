using UnityEngine;

public class SetPlayerChasedService : BehaviorTreeServiceBase
{

    public SetPlayerChasedService() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Set player chased";
        nodeName = nodeTypeName;
#endif
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.AddChasingCharacter(user.GetNPCController());
        }
    }

    protected override void OnCeaseRelevant(IBehaviorTreeUser user)
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.RemoveChasingCharacter(user.GetNPCController());
        }
    }
}
