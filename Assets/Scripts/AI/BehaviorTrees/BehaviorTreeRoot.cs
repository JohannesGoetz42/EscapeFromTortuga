
public class BehaviorTreeRoot : BehaviorTreeNode
{
    public BehaviorTreeLeaf currentLeaf;
    private BehaviorTreeCompositeNode _rootComposite;

    protected override BehaviorTreeRoot GetRoot() => this;

    private void Update()
    {
        // update the current leaf if it can stay active
        if (currentLeaf != null)
        {
            if (currentLeaf.CanStayActive())
            {
                currentLeaf.Update();
                return;
            }

            currentLeaf.Exit(BehaviorNodeResult.Abort);
        }

        // ... otherwise find the next activateable leaf
        currentLeaf = _rootComposite.TryGetFirstActivateableLeaf();
        if (currentLeaf != null)
        {
            currentLeaf.Update();
        }
    }

    public override BehaviorTreeLeaf TryGetFirstActivateableLeaf()
    {
        if (_rootComposite == null || !_rootComposite.CanEnterNode())
        {
            return _rootComposite.TryGetFirstActivateableLeaf();
        }

        return null;
    }

}
