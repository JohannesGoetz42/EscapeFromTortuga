using System.Collections.Generic;
using UnityEngine;

class VisionMemory
{
    internal ViewCone viewCone;
}

public class VisionService : BehaviorTreeServiceBase
{
    [field: SerializeField]
    public BlackboardKeySelector canSeePlayer { get; set; }
    [field: SerializeField]
    public BlackboardKeySelector target { get; set; }
    [field: SerializeField]
    public BlackboardKeySelector lastSeenLocation { get; set; }

    [SerializeField] bool rememberLastSeenLocation = true;

    /** 
    * The view angle to each side of the character.
    * viewAngle 90deg results in a 180deg field of view
    */
    [SerializeField]
    private float viewAngle = 75.0f;
    [SerializeField]
    private float viewRange = 20.0f;

    private Dictionary<IBehaviorTreeUser, VisionMemory> _memories = new Dictionary<IBehaviorTreeUser, VisionMemory>();

    public VisionService() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Vision";
        nodeName = nodeTypeName;
#endif

        canSeePlayer = new BlackboardKeySelector(BlackboardValueType.Bool, new BlackboardValueType[] { BlackboardValueType.Bool });
        target = new BlackboardKeySelector(BlackboardValueType.Object, new BlackboardValueType[] { BlackboardValueType.Object });
        lastSeenLocation = new BlackboardKeySelector(BlackboardValueType.Vector3, new BlackboardValueType[] { BlackboardValueType.Vector3 });
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        VisionMemory myMemory;
        if (!_memories.ContainsKey(user))
        {
            ViewCone viewCone = user.GetBehaviorUser().GetComponentInChildren<ViewCone>(true);
            if (viewCone == null)
            {
                shouldUpdate = false;
                return;
            }

            myMemory = new VisionMemory();
            myMemory.viewCone = viewCone;
            _memories.Add(user, myMemory);
        }
        else
        {
            myMemory = _memories[user];
        }

        if (myMemory.viewCone != null)
        {
            myMemory.viewCone.viewAngle = viewAngle;
            myMemory.viewCone.viewRange = viewRange;
            myMemory.viewCone.enabled = true;
        }

        UpdateService(user);
    }

    protected override void UpdateService(IBehaviorTreeUser user)
    {
        if (PlayerController.Instance != null)
        {
            if (IsVisible(user, PlayerController.Instance.transform))
            {
                user.GetBlackboard().SetValueAsBool(canSeePlayer.selectedKey, true);
                user.GetBlackboard().SetValueAsObject(target.selectedKey, PlayerController.Instance.transform);
                if (rememberLastSeenLocation)
                {
                    user.GetBlackboard().SetValueAsVector3(lastSeenLocation.selectedKey, PlayerController.Instance.transform.position);
                }
            }
            else
            {
                user.GetBlackboard().SetValueAsBool(canSeePlayer.selectedKey, false);
            }
        }
    }

    bool IsVisible(in IBehaviorTreeUser user, Transform target)
    {
        Transform userTransform = user.GetBehaviorUser();
        Vector3 direction = target.position - userTransform.position;
        // target is out of view range
        if (direction.sqrMagnitude > System.Math.Pow(viewRange, 2))
        {
            return false;
        }

        float dotProduct = Vector3.Dot(userTransform.forward, direction.normalized);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
        if (angle > viewAngle)
        {
            return false;
        }

        RaycastHit hit;
        Vector3 rayDirection = target.position - userTransform.position;
        if (Physics.Raycast(userTransform.position + new Vector3(0.0f, 1.0f, 0.0f), rayDirection, out hit, viewRange, Constants.npcViewLayer))
        {
            return hit.collider.gameObject.CompareTag("Player");
        }

        return false;
    }
}