public abstract class BehaviorTreeCompositeNode : BehaviorTreeNode
{
    public BehaviorTreeNode[] children = new BehaviorTreeNode[0];
    public override bool CanEnterNode()
    {
        return children.Length > 0 && base.CanEnterNode();
    }
}
