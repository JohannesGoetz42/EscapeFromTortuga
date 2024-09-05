
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
                BehaviorTreeAction nextAction = children[nextChild].TryGetFirstActivateableAction(user);
                if (nextAction != null)
                {
                    nextAction.SetActive(user);
                }
                else
                {
                    base.OnChildExit(user, this, BehaviorNodeState.Failed);
                }

                return;
            }
        }

        base.OnChildExit(user, this, result);
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
