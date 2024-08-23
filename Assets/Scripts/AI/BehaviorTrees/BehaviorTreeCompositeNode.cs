using System.Collections.Generic;

public abstract class BehaviorTreeCompositeNode : BehaviorTreeNode
{
    public List<BehaviorTreeNode> children = new List<BehaviorTreeNode>();
    public override bool CanEnterNode()
    {
        return children.Count > 0 && base.CanEnterNode();
    }

#if UNITY_EDITOR
    public override void AddChild(BehaviorTreeNode child)
    {
        children.Add(child);
    }
#endif
}
