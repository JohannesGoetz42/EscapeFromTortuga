using Unity.VisualScripting;
using UnityEngine;

enum EscapeAgentState
{
    Initial,
    WaitingForTaskCompletion,
    WaitingForTaskReturn,
    ReadyToDepart
}

public class EscapeAgent : RunBehaviorTree
{
    const string EscapeAgentStateKey = "EscapeAgentState";
    const string EscapeAreaKey = "EscapeArea";

    [SerializeField]
    EscapeArea escapeArea;
    [SerializeField]
    EscapeAgentTask[] tasks;
    [SerializeField]
    float interactionRange = 4.0f;

    EscapeAgentTask _currentTask;
    int _currentTaskIndex = -1;
    SphereCollider _trigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        _trigger = transform.AddComponent<SphereCollider>();
        _trigger.radius = interactionRange;
        _trigger.isTrigger = true;
    }

    public void OnTaskCompleted(EscapeAgentTask task)
    {
        if (task == _currentTask)
        {
            GameTaskText.Instance.SetText(_currentTask.returnText);
            blackboardInstance.SetValueAsEnum(EscapeAgentStateKey, EscapeAgentState.WaitingForTaskReturn);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        // if in initial state or the current task is complete, advance progree
        EscapeAgentState currentState = blackboardInstance.GetValueAsEnum<EscapeAgentState>(EscapeAgentStateKey);
        if (currentState == EscapeAgentState.Initial || currentState == EscapeAgentState.WaitingForTaskReturn)
        {
            AdvanceProgress();
        }
    }

    void AdvanceProgress()
    {
        _currentTaskIndex++;

        // start new task if there is another one that can be activated
        while (tasks.Length > _currentTaskIndex)
        {
            _currentTask = Instantiate(tasks[_currentTaskIndex]);
            if (_currentTask.SetUp(this))
            {
                GameTaskText.Instance.SetText(_currentTask.taskText);
                DialogueText.AddDialogueTextToObject(transform, _currentTask.dialogueText);

                blackboardInstance.SetValueAsEnum(EscapeAgentStateKey, EscapeAgentState.WaitingForTaskCompletion);
                return;
            }
            else
            {
                Debug.LogWarningFormat("Escape agent task {0} could not be set up and will be skipped!", _currentTask.name);
            }

            _currentTaskIndex++;
        }

        // go to escape area and wait for depart
        blackboardInstance.SetValueAsEnum(EscapeAgentStateKey, EscapeAgentState.ReadyToDepart);
        blackboardInstance.SetValueAsObject(EscapeAreaKey, escapeArea);
    }
}