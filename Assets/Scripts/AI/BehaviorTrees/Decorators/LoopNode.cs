using System.Collections.Generic;
using UnityEngine;

class LoopDecoratorMemory
{
    public int RemainingLoops = 0;
    public float LoopStartTime = 0.0f;
}

public class LoopNode : BehaviorTreeSequence
{
    [SerializeField]
    /** 
     * How often should be looped? 
     * Infinite if 0
     */
    int LoopCount = 0;

    [SerializeField]
    /** 
     * How long should be looped? 
     * Infinite if 0.0f
     */
    float LoopDuration = 0.0f;

    private Dictionary<IBehaviorTreeUser, LoopDecoratorMemory> _memory = new Dictionary<IBehaviorTreeUser, LoopDecoratorMemory>();
    public LoopNode() : base()
    {

#if UNITY_EDITOR
        nodeName = "Loop";
#endif

    }

    protected override void OnBecomeRelevant(IBehaviorTreeUser user)
    {
        LoopDecoratorMemory myMemory;
        if (_memory.ContainsKey(user))
        {
            myMemory = _memory[user];
        }
        else
        {
            myMemory = new LoopDecoratorMemory();
            _memory.Add(user, myMemory);
        }

        myMemory.RemainingLoops = LoopCount;
        myMemory.LoopStartTime = Time.time;

        base.OnBecomeRelevant(user);
    }

    internal override void OnChildExit(IBehaviorTreeUser user, BehaviorTreeNode child, BehaviorNodeState result)
    {
        if (result == BehaviorNodeState.Success)
        {
            int nextChild = children.IndexOf(child) + 1;
            if (children.Count > nextChild)
            {
                BehaviorTreeAction nextAction = children[nextChild].TryGetFirstActivateableAction(user);
                if (nextAction != null)
                {
                    nextAction.SetActive(user);
                    return;
                }
            }

            _memory[user].RemainingLoops--;
            // try start new loop
            if (_memory[user].RemainingLoops > 0)
            {
                BehaviorTreeAction nextAction = children[0].TryGetFirstActivateableAction(user);
                if (nextAction != null)
                {
                    nextAction.SetActive(user);
                    return;
                }
            }
        }

        CeaseRelevant(user);
        parent.OnChildExit(user, this, result);
    }

    public override bool CanStayActive(IBehaviorTreeUser user)
    {
        if (LoopDuration > 0.0f &&
            Time.time - _memory[user].LoopStartTime > LoopDuration)
        {
            return false;
        }

        return base.CanStayActive(user);
    }
}
