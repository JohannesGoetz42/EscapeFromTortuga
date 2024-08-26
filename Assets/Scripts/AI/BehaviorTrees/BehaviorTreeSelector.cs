
public class BehaviorTreeSelector : BehaviorTreeCompositeNode
{
    public BehaviorTreeSelector() : base()
    {
#if UNITY_EDITOR
        nodeName = "Selector";
#endif
    }

    public override BehaviorTreeAction TryGetFirstActivateableAction()
    {
        foreach (BehaviorTreeNode descendant in children)
        {
            if (descendant.CanEnterNode())
            {
                return descendant.TryGetFirstActivateableAction();
            }
        }

        return null;
    }
}
