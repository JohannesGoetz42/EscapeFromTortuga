
using System;

public class BehaviorTreeSequence : BehaviorTreeCompositeNode
{
    public override void OnChildExit(BehaviorTreeNode child, BehaviorNodeResult result)
    {
        if (result == BehaviorNodeResult.Success)
        {
            int nextChild = Array.IndexOf(children, child) + 1;
            if (children.Length > nextChild)
            {
                GetRoot().currentLeaf = children[nextChild].TryGetFirstActivateableLeaf();
                return;
            }
        }

        base.OnChildExit(child, result);
    }
}
