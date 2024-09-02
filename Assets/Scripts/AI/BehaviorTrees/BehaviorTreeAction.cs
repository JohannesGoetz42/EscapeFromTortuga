using UnityEngine;
using UnityEngine.Animations;

public class BehaviorTreeAction : BehaviorTreeNode
{
    public override BehaviorTreeAction TryGetFirstActivateableAction(IBehaviorTreeUser user) => this;
    virtual public void UpdateNode(IBehaviorTreeUser user) { }
    virtual public void Exit(IBehaviorTreeUser user, BehaviorNodeResult result) { parent.OnChildExit(user, this, result); }
}
