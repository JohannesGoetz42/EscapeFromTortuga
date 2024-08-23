
public class BehaviorTreeRoot : BehaviorTreeNode
{
    public BehaviorTreeLeaf currentLeaf;
    private BehaviorTreeNode _startNode;

    protected override BehaviorTreeRoot GetRoot() => this;

    public BehaviorTreeRoot() : base()
    {
#if UNITY_EDITOR
        nodeName = "Root";
#endif
    }

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
        currentLeaf = _startNode.TryGetFirstActivateableLeaf();
        if (currentLeaf != null)
        {
            currentLeaf.Update();
        }
    }

    public override BehaviorTreeLeaf TryGetFirstActivateableLeaf()
    {
        if (_startNode == null || !_startNode.CanEnterNode())
        {
            return _startNode.TryGetFirstActivateableLeaf();
        }

        return null;
    }

#if UNITY_EDITOR
    public override void AddChild(BehaviorTreeNode child)
    {
        _startNode = child;
    }
#endif
}
