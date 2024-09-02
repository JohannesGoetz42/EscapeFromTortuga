

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
            // update the current  if it can stay active
            if (currentActions[user] != null)
            {
                if (currentActions[user].CanStayActive(user))
                {
                    currentActions[user].UpdateNode(user);
                    return;
                }

                currentActions[user].Exit(user, BehaviorNodeResult.Abort);
            }

            // ... otherwise find the next activateable 
            currentActions[user] = StartNode.TryGetFirstActivateableAction(user);
            if (currentActions[user] != null)
            {
                currentActions[user].UpdateNode(user);
            }
        }
    }

    public override BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user)
    {
        if (StartNode == null || !StartNode.CanEnterNode(user))
        {
            return StartNode.TryGetFirstActivateableAction(user);
        }

        return null;
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
