using UnityEngine;

public class RunBehaviorTree : MonoBehaviour
{
    [SerializeField] BehaviorTree behaviorTree;
    private Blackboard _blackboardInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (behaviorTree != null && behaviorTree.blackboard != null && behaviorTree.root != null)
        {
            _blackboardInstance = ScriptableObject.CreateInstance(behaviorTree.blackboard.GetType()) as Blackboard;
            _blackboardInstance.InitializeValues();
            enabled = true;
        }
        else
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        behaviorTree.root.Update();
    }
}
