
using System;

public class BehaviorTreeSequence : BehaviorTreeCompositeNode
{
    public BehaviorTreeSequence() : base()
    {
#if UNITY_EDITOR
        nodeName = "Sequence";
#endif
    }

    public override void OnChildExit(IBehaviorTreeUser user, BehaviorTreeNode child, BehaviorNodeResult result)
    {
        if (result == BehaviorNodeResult.Success)
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
}
