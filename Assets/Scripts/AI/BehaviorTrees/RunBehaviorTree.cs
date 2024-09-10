using UnityEngine;

public class RunBehaviorTree : MonoBehaviour, IBehaviorTreeUser
{
    [SerializeField] BehaviorTree behaviorTree;
    public Blackboard blackboardInstance { get; private set; }

    public Blackboard GetBlackboard() => blackboardInstance;
    public Transform GetBehaviorUser() => transform;

    public INPCController GetNPCController() => _controller;
    private NPCController _controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        _controller = GetComponent<NPCController>();
        if (_controller != null && behaviorTree != null && behaviorTree.root != null)
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
