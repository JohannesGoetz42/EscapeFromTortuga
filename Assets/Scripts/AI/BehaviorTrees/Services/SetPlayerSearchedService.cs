using UnityEngine;

public class SetPlayerSearchedService : BehaviorTreeServiceBase
{

    public SetPlayerSearchedService() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Set player searched";
        nodeName = nodeTypeName;
#endif
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        NPCController controller = user.GetBehaviorUser().GetComponent<NPCController>();
        if (controller != null && PlayerController.Instance != null)
        {
            PlayerController.Instance.AddChasingCharacter(controller);
        }
    }

    protected override void OnCeaseRelevant(IBehaviorTreeUser user)
    {
        NPCController controller = user.GetBehaviorUser().GetComponent<NPCController>();
        if (controller != null && PlayerController.Instance != null)
        {
            PlayerController.Instance.RemoveChasingCharacter(controller);
        }
    }
}
