using System.Collections.Generic;
using UnityEngine;

class MoveToActionMemory
{
    public Vector3 targetPosition;
    public Transform targetTransform;
    public Queue<Vector3> path;
    public bool hasFoundPath;
    public float timeSincePathUpdate;
    /** randomize the update interval to avoid updating every instance at the same time */
    public float nextUpdateInterval;
}

public class MoveToAction : BehaviorTreeAction
{
    [field: SerializeField]
    public BlackboardKeySelector movementTarget { get; set; }

    [SerializeField]
    /** the maximum distance to the target at which the action is considered complete */
    float acceptedDistance = 0.5f;
    [SerializeField]
    float updatePathInterval = 0.5f;
    [SerializeField]
    float updateIntervalRange = 0.3f;

    [SerializeField]
    /** 
     * whether the target should be updated when the target moves 
     * Only effective if the target is an object
     */
    bool trackTargetMovement = false;

    Dictionary<IBehaviorTreeUser, MoveToActionMemory> _memory = new Dictionary<IBehaviorTreeUser, MoveToActionMemory>();

    public MoveToAction() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Move to";
        nodeName = nodeTypeName;
#endif

        movementTarget = new BlackboardKeySelector(BlackboardValueType.Object, new BlackboardValueType[] { BlackboardValueType.Object, BlackboardValueType.Vector3 });
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        // initialize memory
        MoveToActionMemory myMemory;
        if (_memory.ContainsKey(user))
        {
            myMemory = _memory[user];
        }
        else
        {
            myMemory = new MoveToActionMemory();
            _memory.Add(user, myMemory);
        }

        // reset path
        myMemory.hasFoundPath = false;

        if (movementTarget.type == BlackboardValueType.Object)
        {
            Object blackboardObject = user.GetBlackboard().GetValueAsObject(movementTarget.selectedKey);
            if (blackboardObject == null)
            {
                Exit(user, BehaviorNodeState.Failed);
                return;
            }

            Transform transform = blackboardObject as Transform;
            if (transform == null)
            {
                GameObject gameObect = blackboardObject as GameObject;
                transform = gameObect.transform;
            }

            // if no avlid object is assigned, exit as failed
            if (transform == null)
            {
                Exit(user, BehaviorNodeState.Failed);
                return;
            }

            myMemory.targetTransform = transform;
            myMemory.targetPosition = transform.position;
        }
        else
        {
            myMemory.targetPosition = user.GetBlackboard().GetValueAsVector3(movementTarget.selectedKey);
        }

        UpdatePath(user, myMemory);
    }

    protected override void OnUpdate(IBehaviorTreeUser user)
    {
        MoveToActionMemory myMemory = _memory[user];

        // if target is an object, update the path
        if (myMemory.targetTransform != null)
        {
            myMemory.timeSincePathUpdate += Time.deltaTime;
            if (trackTargetMovement && myMemory.timeSincePathUpdate > myMemory.nextUpdateInterval)
            {
                myMemory.targetPosition = myMemory.targetTransform.position;
                UpdatePath(user, myMemory);
            }
        }


        if (myMemory.hasFoundPath)
        {
            // target is reached
            if (myMemory.path.Count == 0)
            {
                if ((user.GetNPCController().GetMovementTarget() - user.GetBehaviorUser().transform.position).sqrMagnitude < acceptedDistance * acceptedDistance)
                {
                    myMemory.hasFoundPath = false;
                    user.GetNPCController().ClearMovementTarget();
                    Exit(user, BehaviorNodeState.Success);
                }

                return;
            }

            Vector3 movementTarget = myMemory.path.Peek();

            // handle path segment target reached
            if ((movementTarget - user.GetBehaviorUser().transform.position).sqrMagnitude < acceptedDistance * acceptedDistance)
            {
                // start next path segment
                myMemory.path.Dequeue();
            }

            user.GetNPCController().SetMovementTarget(movementTarget);
        }
    }

    void UpdatePath(IBehaviorTreeUser user, in MoveToActionMemory myMemory)
    {
        PathRequest request = new PathRequest(user, user.GetBehaviorUser().position, myMemory.targetPosition, OnPathFound);
        PathfindingManager.RequestPath(request);
    }

    void OnPathFound(Vector3[] pathResult, bool success, IBehaviorTreeUser user)
    {
        MoveToActionMemory myMemory = _memory[user];
        myMemory.hasFoundPath = success;
        if (success)
        {
            myMemory.path = new Queue<Vector3>(pathResult);
        }
        // if no path could be found, fail action
        else
        {
            Exit(user, BehaviorNodeState.Failed);
            return;
        }

        // schedule next path request
        if (trackTargetMovement && myMemory.targetTransform == null)
        {
            myMemory.nextUpdateInterval = Random.Range(updatePathInterval - updateIntervalRange, updatePathInterval + updateIntervalRange);
            myMemory.timeSincePathUpdate = 0.0f;
        }
    }
}
