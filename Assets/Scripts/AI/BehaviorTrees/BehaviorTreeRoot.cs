

using System.Collections.Generic;

public class BehaviorTreeRoot : BehaviorTreeNode
{
    public Dictionary<IBehaviorTreeUser, BehaviorTreeAction> currentActions;
    public BehaviorTreeNode StartNode { get; private set; }

    public BehaviorTreeRoot() : base()
    {
#if UNITY_EDITOR
        nodeName = "Root";
#endif
    }

    public void UpdateBehavior()
    {
        foreach (IBehaviorTreeUser user in currentActions.Keys)
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
        StartNode = child as BehaviorTreeCompositeNode;
    }

    public override void RemoveChild(BehaviorTreeNodeBase node)
    {
        if (StartNode == node)
        {
            StartNode = null;
        }
    }
#endif
}
