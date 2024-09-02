
public class BehaviorTreeSelector : BehaviorTreeCompositeNode
{
    public BehaviorTreeSelector() : base()
    {
#if UNITY_EDITOR
        nodeName = "Selector";
#endif
    }

    public override BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user)
    {
        foreach (BehaviorTreeNode descendant in children)
        {
            if (descendant.CanEnterNode(user))
            {
                return descendant.TryGetFirstActivateableAction(user);
            }
        }

        return null;
    }
}
