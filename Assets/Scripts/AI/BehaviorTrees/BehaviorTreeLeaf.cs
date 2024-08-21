using UnityEngine;
using UnityEngine.Animations;

public class BehaviorTreeLeaf : BehaviorTreeNode
{
    public override BehaviorTreeLeaf TryGetFirstActivateableLeaf() => this;
    virtual public void Update() { }
    virtual public void Exit(BehaviorNodeResult result) { parent.OnChildExit(this, result); }
}
