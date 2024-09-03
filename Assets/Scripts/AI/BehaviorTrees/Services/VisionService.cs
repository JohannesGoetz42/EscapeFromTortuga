using System.Collections.Generic;
using UnityEngine;

struct VisionMemory
{
    internal ViewCone viewCone;
}

public class VisionService : BehaviorTreeServiceBase
{
    [SerializeField] BlackboardKeySelector canSeePlayer = new BlackboardKeySelector(BlackboardValueType.Bool);
    [SerializeField] BlackboardKeySelector target = new BlackboardKeySelector(BlackboardValueType.Object);

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
        nodeName = "Vision";
#endif
    }

    internal override void BecomeRelevant(IBehaviorTreeUser user)
    {
        if (user == null)
        {
            return;
        }
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
            myMemory.viewCone.SetViewConeMode(ViewConeMode.Idle);
        }
    }

    protected override void UpdateService(IBehaviorTreeUser user)
    {
        if (PlayerController.Instance != null)
        {
            if (IsVisible(user, PlayerController.Instance.transform))
            {
                user.GetBlackboard().SetValueAsBool(canSeePlayer.selectedKey, true);
                user.GetBlackboard().SetValueAsObject(target.selectedKey, PlayerController.Instance.transform);
            }
            else
            {
                user.GetBlackboard().SetValueAsBool(canSeePlayer.selectedKey, false);
                user.GetBlackboard().SetValueAsObject(target.selectedKey, null);
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
        if (Physics.Raycast(userTransform.position, rayDirection, out hit, viewRange))
        {
            return hit.collider.gameObject.CompareTag("Player");
        }

        return false;
    }
}