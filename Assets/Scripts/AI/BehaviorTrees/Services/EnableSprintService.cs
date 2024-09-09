using UnityEngine;

public class EnableSprintService : BehaviorTreeServiceBase
{
    [SerializeField]
    bool wantsToSprint = true;

    public EnableSprintService() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Enable sprint";
        nodeName = nodeTypeName;
#endif
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        NPCController controller = user.GetBehaviorUser().GetComponent<NPCController>();
        if (controller != null)
        {
            controller.wantsToSprint = wantsToSprint;
        }
    }

    protected override void OnCeaseRelevant(IBehaviorTreeUser user)
    {
        NPCController controller = user.GetBehaviorUser().GetComponent<NPCController>();
        if (controller != null)
        {
            controller.wantsToSprint = !wantsToSprint;
        }
    }
}
