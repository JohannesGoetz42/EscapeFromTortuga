
public class BehaviorTreeSelector : BehaviorTreeCompositeNode
{
    public BehaviorTreeSelector() : base()
    {
#if UNITY_EDITOR
        nodeTypeName = "Selector";
        nodeName = nodeTypeName;
#endif
    }

    internal override BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user)
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

    internal override void OnChildExit(IBehaviorTreeUser user, BehaviorTreeNode child, BehaviorNodeState result)
    {
        CeaseRelevant(user);
        parent.OnChildExit(user, this, result);
    }
}
