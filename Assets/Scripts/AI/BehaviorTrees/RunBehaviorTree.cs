using UnityEngine;

public class RunBehaviorTree : MonoBehaviour, IBehaviorTreeUser
{
    [SerializeField] BehaviorTree behaviorTree;
    public Blackboard blackboardInstance { get; private set; }

    public Blackboard GetBlackboard() => blackboardInstance;
    public Transform GetBehaviorUser() => transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (behaviorTree != null && behaviorTree.blackboard != null && behaviorTree.root != null)
        {
            blackboardInstance = behaviorTree.RunBehavior(this);
        }
    }

    private void OnDestroy()
    {
        if (behaviorTree != null)
        {
            behaviorTree.StopBehavior(this);
        }
    }
}
