using UnityEngine;
using UnityEngine.Animations;

public class BehaviorTreeAction : BehaviorTreeNode
{
    public override BehaviorTreeAction TryGetFirstActivateableAction() => this;
    virtual public void Update(IBehaviorTreeUser user) { }
    virtual public void Exit(BehaviorNodeResult result) { parent.OnChildExit(this, result); }
}
