using System.Collections.Generic;

public abstract class BehaviorTreeCompositeNode : BehaviorTreeNode
{
    public List<BehaviorTreeNode> children = new List<BehaviorTreeNode>();
    public override bool CanEnterNode(IBehaviorTreeUser user)
    {
        return children.Count > 0 && base.CanEnterNode(user);
    }

#if UNITY_EDITOR
    public override void AddChild(BehaviorTreeNodeBase child)
    {
        BehaviorTreeNode childNode = child as BehaviorTreeNode;
        if (childNode != null)
        {
            children.Add(childNode);
        }

        base.AddChild(child);
    }

    public override void RemoveChild(BehaviorTreeNodeBase child)
    {
        BehaviorTreeNode childNode = child as BehaviorTreeNode;
        if (childNode != null)
        {
            children.Remove(childNode);
        }

        base.RemoveChild(child);
    }
#endif
}
