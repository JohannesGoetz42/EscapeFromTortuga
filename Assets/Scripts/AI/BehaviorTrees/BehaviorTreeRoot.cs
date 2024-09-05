

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
        nodeName = "Root";
#endif
    }

    public void UpdateBehavior()
    {
        foreach (IBehaviorTreeUser user in behaviorTree.ActiveUsers)
        {
            // update the current action if it can stay active
            if (currentActions[user] != null)
            {
                if (currentActions[user].CanStayActive(user))
                {
                    currentActions[user].UpdateNode(user);
                    return;
                }

                currentActions[user].Exit(user, BehaviorNodeState.Aborted);
            }

            // ... otherwise find the next activateable 
            currentActions[user] = StartNode.TryGetFirstActivateableAction(user);
            if (currentActions[user] != null)
            {
                currentActions[user].SetActive(user);
                currentActions[user].UpdateNode(user);
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
        if (result == BehaviorNodeState.Failed)
        {
            TryGetFirstActivateableAction(user);
            return;
        }
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
