using UnityEngine;
using UnityEngine.Animations;

public class BehaviorTreeAction : BehaviorTreeNode
{
    internal override BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user) => this;

    internal void SetActive(IBehaviorTreeUser user)
    {
        behaviorTree.root.currentActions[user] = this;
        BecomeRelevant(user);
    }

    internal override void UpdateNode(IBehaviorTreeUser user)
    {
        base.UpdateNode(user);
        OnUpdate(user);
    }

    protected virtual void OnUpdate(IBehaviorTreeUser user) { }

    virtual public void Exit(IBehaviorTreeUser user, BehaviorNodeState result)
    {
        States[user] = result;
        OnCeaseRelevant(user);
        parent.OnChildExit(user, this, result);
    }

    internal override void OnChildExit(IBehaviorTreeUser user, BehaviorTreeNode child, BehaviorNodeState result)
    {
        Debug.LogErrorFormat("OnChildExit called on action node '{0}'. This should never happen!", name);
    }
}
