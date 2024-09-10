using UnityEngine;

public class SetWorldMarkerVisibilityService : BehaviorTreeServiceBase
{
    [SerializeField]
    WorldMarkerVisibility visibility;

    SetWorldMarkerVisibilityService() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Marker visibility";
        nodeName = nodeTypeName;
#endif
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        WorldMarker worldMarker = user.GetBehaviorUser().GetComponentInChildren<WorldMarker>();
        if (worldMarker == null)
        {
            Debug.LogWarningFormat("Could not find world marker for {0}", user.GetBehaviorUser().name);
            return;
        }

        worldMarker.AddMarkerSource(this, visibility);
    }

    protected override void OnCeaseRelevant(IBehaviorTreeUser user)
    {
        WorldMarker worldMarker = user.GetBehaviorUser().GetComponentInChildren<WorldMarker>();
        if (worldMarker != null)
        {
            worldMarker.RemoveMarkerSource(this);
        }
    }
}
