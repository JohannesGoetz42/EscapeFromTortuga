

using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeRoot : BehaviorTreeNode
{
    public Dictionary<IBehaviorTreeUser, BehaviorTreeAction> currentActions = new Dictionary<IBehaviorTreeUser, BehaviorTreeAction>();

    [SerializeField]
    private BehaviorTreeNode startNode;
    public BehaviorTreeNode StartNode { get => startNode; }

    public BehaviorTreeRoot() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Root";
        nodeName = nodeTypeName;
#endif
    }

    public void UpdateBehavior()
    {
        foreach (IBehaviorTreeUser user in behaviorTree.ActiveUsers)
        {
            BehaviorTreeAction currentAction = currentActions[user];
            // update the current action if it can stay active
            if (currentAction != null)
            {
                if (currentAction.CanStayActive(user))
                {
                    currentAction.UpdateNode(user);
                    continue;
                }

                BehaviorTreeNode abortedNode = currentActions[user];
                currentAction.Exit(user, BehaviorNodeState.Aborted);

                // if the aborted node is not the same as the current node (node switch was handled by exit), update the new current node
                if (currentAction != null && abortedNode != currentAction)
                {
                    currentActions[user].UpdateNode(user);
                    continue;
                }
            }

            // ... otherwise find the next activateable 
            currentActions[user] = StartNode.TryGetFirstActivateableAction(user);
            if (currentActions[user] != null)
            {
                currentActions[user].SetActive(user);
            }
        }
    }

    internal override BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user)
    {
        if (StartNode == null || !StartNode.CanEnterNode(user))
        {
            return null;
        }

        return StartNode.TryGetFirstActivateableAction(user);
    }

    internal override void OnChildExit(IBehaviorTreeUser user, BehaviorTreeNode child, BehaviorNodeState result)
    {
        // to avoid stack overflow, just clear the child and wait for the next update to handle selection of next action
        currentActions[user] = null;
    }

    public override bool CanStayActive(IBehaviorTreeUser user) => true;

#if UNITY_EDITOR
    public override void AddChild(BehaviorTreeNodeBase child)
    {
        startNode = child as BehaviorTreeCompositeNode;
    }

    public override void RemoveChild(BehaviorTreeNodeBase node)
    {
        if (StartNode == node)
        {
            startNode = null;
        }
    }
#endif
}
