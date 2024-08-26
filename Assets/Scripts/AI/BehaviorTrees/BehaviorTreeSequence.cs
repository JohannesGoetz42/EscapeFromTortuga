
using System;

public class BehaviorTreeSequence : BehaviorTreeCompositeNode
{
    public BehaviorTreeSequence() : base()
    {
#if UNITY_EDITOR
        nodeName = "Sequence";
#endif
    }

    public override void OnChildExit(BehaviorTreeNode child, BehaviorNodeResult result)
    {
        if (result == BehaviorNodeResult.Success)
        {
            int nextChild = children.IndexOf(child) + 1;
            if (children.Count > nextChild)
            {
                GetRoot().currentAction = children[nextChild].TryGetFirstActivateableAction();
                return;
            }
        }

        base.OnChildExit(child, result);
    }
}
