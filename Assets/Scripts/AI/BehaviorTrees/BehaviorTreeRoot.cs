
using static Unity.VisualScripting.Metadata;

public class BehaviorTreeRoot : BehaviorTreeNode
{
    public BehaviorTreeAction currentAction;
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
        // update the current  if it can stay active
        if (currentAction != null)
        {
            if (currentAction.CanStayActive())
            {
                currentAction.Update();
                return;
            }

            currentAction.Exit(BehaviorNodeResult.Abort);
        }

        // ... otherwise find the next activateable 
        currentAction = _startNode.TryGetFirstActivateableAction();
        if (currentAction != null)
        {
            currentAction.Update();
        }
    }

    public override BehaviorTreeAction TryGetFirstActivateableAction()
    {
        if (_startNode == null || !_startNode.CanEnterNode())
        {
            return _startNode.TryGetFirstActivateableAction();
        }

        return null;
    }

#if UNITY_EDITOR
    public override void AddChild(BehaviorTreeNodeBase child)
    {
        _startNode = child as BehaviorTreeCompositeNode;
    }

    public override void RemoveChild(BehaviorTreeNodeBase node) 
    {
        if(_startNode == node)
        {
            _startNode = null;
        }
    }
#endif
}
