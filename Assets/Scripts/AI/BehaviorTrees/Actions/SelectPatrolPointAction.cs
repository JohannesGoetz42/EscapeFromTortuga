using System.Collections.Generic;
using UnityEngine;

enum PatrolPointSelectionMode
{
    InOrder,
    Random
}

class SelectPatrolPointMemory
{
    public int SelectedIndex;
    public NPCController controller;
}

public class SelectPatrolPointAction : BehaviorTreeAction
{
    [field: SerializeField]
    public BlackboardKeySelector patrolPoint { get; set; }
    [SerializeField]
    PatrolPointSelectionMode selectionMode;

    Dictionary<IBehaviorTreeUser, SelectPatrolPointMemory> _memory = new Dictionary<IBehaviorTreeUser, SelectPatrolPointMemory>();

    public SelectPatrolPointAction() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Select patrol point";
        nodeName = nodeTypeName;
#endif

        patrolPoint = new BlackboardKeySelector(BlackboardValueType.Object, new BlackboardValueType[] { BlackboardValueType.Object, BlackboardValueType.Vector3 });
    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        // initialize memory
        SelectPatrolPointMemory myMemory;
        if (_memory.ContainsKey(user))
        {
            myMemory = _memory[user];
        }
        else
        {
            myMemory = new SelectPatrolPointMemory();
            myMemory.controller = user.GetNPCController() as NPCController;
            if (!myMemory.controller || myMemory.controller.patrolPoints.Length == 0)
            {
                Exit(user, BehaviorNodeState.Failed);
                return;
            }

            _memory.Add(user, myMemory);
        }

        int selectedPoint = myMemory.SelectedIndex;
        switch (selectionMode)
        {
            case PatrolPointSelectionMode.Random:
                selectedPoint = Random.Range(0, myMemory.controller.patrolPoints.Length);
                // to evenly distribute random chances, increment every value that is above the current selected index,
                // not only if the new selected index equals the current index
                if (selectedPoint >= myMemory.SelectedIndex)
                {
                    selectedPoint++;
                }
                break;
            case PatrolPointSelectionMode.InOrder:
                selectedPoint++;
                break;
            default:
                Debug.LogErrorFormat("Patrol point selection mode '{0}' is not implemented!", selectionMode.ToString());
                Exit(user, BehaviorNodeState.Failed);
                return;
        }

        selectedPoint = selectedPoint % myMemory.controller.patrolPoints.Length;
        myMemory.SelectedIndex = selectedPoint;

        if (patrolPoint.type == BlackboardValueType.Object)
        {
            user.GetBlackboard().SetValueAsObject(patrolPoint.selectedKey, myMemory.controller.patrolPoints[selectedPoint]);
        }
        else if (patrolPoint.type != BlackboardValueType.Vector3)
        {
            user.GetBlackboard().SetValueAsVector3(patrolPoint.selectedKey, myMemory.controller.patrolPoints[selectedPoint].position);
        }
        else
        {
            Exit(user, BehaviorNodeState.Failed);
            return;
        }

        Exit(user, BehaviorNodeState.Success);
    }

}
