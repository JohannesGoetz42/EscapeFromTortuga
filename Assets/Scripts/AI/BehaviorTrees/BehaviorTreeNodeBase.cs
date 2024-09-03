using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BehaviorNodeState
{
    Inactive,
    Active,
    Success,
    Aborted,
    Failed
}

public abstract class BehaviorTreeNodeBase : ScriptableObject
{
    public BehaviorTreeNode parent;
    public BehaviorTree behaviorTree;
    private Dictionary<IBehaviorTreeUser, BehaviorNodeState> _states = new Dictionary<IBehaviorTreeUser, BehaviorNodeState>();
    public Dictionary<IBehaviorTreeUser, BehaviorNodeState> States { get => _states; }

#if UNITY_EDITOR
    public string nodeName;
    public GUID id;
#endif

    public void SetParent(BehaviorTreeNode newParent)
    {
        parent = newParent;
    }

    internal virtual void BecomeRelevant(IBehaviorTreeUser user)
    {
        if (!States.ContainsKey(user))
        {
            States.Add(user, BehaviorNodeState.Active);
        }
        else if (States[user] == BehaviorNodeState.Active)
        {
            return;
        }

        States[user] = BehaviorNodeState.Active;
        OnBecomeRelevant(user);
    }

    internal virtual void CeaseRelevant(IBehaviorTreeUser user)
    {
        States[user] = BehaviorNodeState.Inactive;
        OnCeaseRelevant(user);
    }

    /**
     * Called when the node becomes relevant 
     * (this node / the parent of an embedded node is entered)
     * Initialize user specific memory here
     */
    protected virtual void OnBecomeRelevant(IBehaviorTreeUser user) { }

    ///**  
    // * Called when the node ceases to be relevant 
    // * (this node / the parent of an embedded node is exited)
    // */
    protected virtual void OnCeaseRelevant(IBehaviorTreeUser user) { }

    /**  
     * Called when the user stops running the owning behavior
     * (this node / the parent of an embedded node is exited)
     * remove user specific memory here
     */
    protected virtual void OnBehaviorStopped(IBehaviorTreeUser user)
    {
        States.Remove(user);
    }
}
