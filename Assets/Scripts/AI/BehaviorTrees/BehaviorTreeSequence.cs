
using System;

public class BehaviorTreeSequence : BehaviorTreeCompositeNode
{
    public BehaviorTreeSequence() : base()
    {
#if UNITY_EDITOR
        nodeName = "Sequence";
#endif
    }

    internal override void OnChildExit(IBehaviorTreeUser user, BehaviorTreeNode child, BehaviorNodeState result)
    {
        if (result == BehaviorNodeState.Success)
        {
            int nextChild = children.IndexOf(child) + 1;
            if (children.Count > nextChild)
            {
                behaviorTree.root.currentActions[user] = children[nextChild].TryGetFirstActivateableAction(user);
                return;
            }
        }

        base.OnChildExit(user, child, result);
    }

    internal override BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user)
    {
        foreach (BehaviorTreeNode child in children)
        {
            if (child.CanEnterNode(user))
            {
                return child.TryGetFirstActivateableAction(user);
            }
        }

        return null;
    }

}
