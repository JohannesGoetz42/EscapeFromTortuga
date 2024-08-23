
public class BehaviorTreeSelector : BehaviorTreeCompositeNode
{
    public BehaviorTreeSelector() : base()
    {
#if UNITY_EDITOR
        nodeName = "Selector";
#endif
    }

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
