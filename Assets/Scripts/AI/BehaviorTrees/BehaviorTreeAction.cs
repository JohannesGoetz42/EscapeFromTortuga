using UnityEngine;
using UnityEngine.Animations;

public class BehaviorTreeAction : BehaviorTreeNode
{
    internal override BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user) => this;

    internal void SetActive(IBehaviorTreeUser user)
    {
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
        parent.OnChildExit(user, this, result);
    }
}
