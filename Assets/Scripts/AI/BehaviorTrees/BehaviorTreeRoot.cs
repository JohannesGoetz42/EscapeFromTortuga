

public class BehaviorTreeRoot : BehaviorTreeNode
{
    public BehaviorTreeAction currentAction;
    public BehaviorTreeNode StartNode { get; private set; }

    public BehaviorTreeRoot() : base()
    {
#if UNITY_EDITOR
        nodeName = "Root";
#endif
    }

    public void UpdateBehavior(IBehaviorTreeUser user)
    {
        // update the current  if it can stay active
        if (currentAction != null)
        {
            if (currentAction.CanStayActive())
            {
                currentAction.UpdateNode(user);
                return;
            }

            currentAction.Exit(BehaviorNodeResult.Abort);
        }

        // ... otherwise find the next activateable 
        currentAction = StartNode.TryGetFirstActivateableAction();
        if (currentAction != null)
        {
            currentAction.UpdateNode(user);
        }
    }

    public override BehaviorTreeAction TryGetFirstActivateableAction()
    {
        if (StartNode == null || !StartNode.CanEnterNode())
        {
            return StartNode.TryGetFirstActivateableAction();
        }

        return null;
    }

    public override bool CanStayActive() => true;

#if UNITY_EDITOR
    public override void AddChild(BehaviorTreeNodeBase child)
    {
        StartNode = child as BehaviorTreeCompositeNode;
    }

    public override void RemoveChild(BehaviorTreeNodeBase node)
    {
        if (StartNode == node)
        {
            StartNode = null;
        }
    }
#endif
}
