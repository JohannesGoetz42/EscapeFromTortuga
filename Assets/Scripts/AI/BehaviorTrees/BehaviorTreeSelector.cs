
public class BehaviorTreeSelector : BehaviorTreeCompositeNode
{
    public override BehaviorTreeLeaf TryGetFirstActivateableLeaf()
    {
        foreach (BehaviorTreeNode descendant in children)
        {
            if (descendant.CanEnterNode())
            {
                return descendant.TryGetFirstActivateableLeaf();
            }
        }

        return null;
    }
}
