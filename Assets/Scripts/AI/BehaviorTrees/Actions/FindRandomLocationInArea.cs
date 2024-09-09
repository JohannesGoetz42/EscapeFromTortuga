using System.Collections.Generic;
using UnityEngine;


class FindRandomLocationInAreaMemory
{
    public PathRequest request;
    public Vector3 searchPosition;
    public int currentTry = 0;
}

public class FindRandomLocationInArea : BehaviorTreeAction
{
    [field: SerializeField]
    public BlackboardKeySelector searchLocationInput { get; set; }
    [field: SerializeField]
    public BlackboardKeySelector pickedLocation { get; set; }
    [SerializeField]
    float minimumDistance = 1.0f;
    [SerializeField]
    float maximumDistance = 5.0f;
    [SerializeField]
    bool findNavigableOnly = true;
    [SerializeField]
    int maximumNavigationTries = 5;

    Dictionary<IBehaviorTreeUser, FindRandomLocationInAreaMemory> _memory = new Dictionary<IBehaviorTreeUser, FindRandomLocationInAreaMemory>();
    public FindRandomLocationInArea() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Find random location in area";
        nodeName = nodeTypeName;
#endif

        searchLocationInput = new BlackboardKeySelector(BlackboardValueType.Vector3, new BlackboardValueType[] { BlackboardValueType.Object, BlackboardValueType.Vector3 });
        pickedLocation = new BlackboardKeySelector(BlackboardValueType.Vector3, new BlackboardValueType[] { BlackboardValueType.Vector3 });
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        Vector3 searchPosition = Vector3.zero;
        if (!TryGetSearchLocation(user, ref searchPosition))
        {
            Exit(user, BehaviorNodeState.Failed);
            return;
        }

        if (findNavigableOnly)
        {
            FindRandomLocationInAreaMemory myMemory;
            if (_memory.ContainsKey(user))
            {
                myMemory = _memory[user];
            }
            else
            {
                myMemory = new FindRandomLocationInAreaMemory();
                _memory.Add(user, myMemory);
            }

            myMemory.currentTry = 0;
            myMemory.searchPosition = searchPosition;
            myMemory.request = new PathRequest();
            myMemory.request.startPosition = user.GetBehaviorUser().position;
            myMemory.request.targetPosition = FindRandomLocation(searchPosition);
            myMemory.request.user = user;
            myMemory.request.callback = OnPathRequestFinished;
            PathfindingManager.RequestPath(myMemory.request);
        }
        else
        {
            Vector3 location = FindRandomLocation(searchPosition);
            user.GetBlackboard().SetValueAsVector3(pickedLocation.selectedKey, location);
            Exit(user, BehaviorNodeState.Success);
        }
    }

    bool TryGetSearchLocation(IBehaviorTreeUser user, ref Vector3 searchLocation)
    {
        // get the location by blackboard key type
        switch (searchLocationInput.type)
        {
            case BlackboardValueType.Vector3:
                searchLocation = user.GetBlackboard().GetValueAsVector3(searchLocationInput.selectedKey);
                return true;
            case BlackboardValueType.Object:
                Transform objectTransform = user.GetBlackboard().TryGetTransformFromObject(searchLocationInput.selectedKey);
                if (objectTransform == null)
                {
                    return false;
                }

                searchLocation = objectTransform.position;
                return true;
            default:
                return false;
        }
    }

    private Vector3 FindRandomLocation(Vector3 searchLocation)
    {
        float selectedDistance = Random.Range(minimumDistance, maximumDistance);
        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
        return randomRotation * (Vector3.one * selectedDistance) + searchLocation;
    }

    void OnPathRequestFinished(Vector3[] path, bool success, IBehaviorTreeUser user)
    {
        FindRandomLocationInAreaMemory myMemory = _memory[user];
        if (success)
        {
            user.GetBlackboard().SetValueAsVector3(pickedLocation.selectedKey, myMemory.request.targetPosition);
            Exit(user, BehaviorNodeState.Success);
            return;
        }

        if (myMemory.currentTry < maximumNavigationTries)
        {
            myMemory.currentTry++;
            myMemory.request.startPosition = user.GetBehaviorUser().position;
            myMemory.request.targetPosition = FindRandomLocation(myMemory.searchPosition);
            PathfindingManager.RequestPath(myMemory.request);
            return;
        }

        Exit(user, BehaviorNodeState.Failed);
    }
}
