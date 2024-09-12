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
    [SerializeField]
    DialogueText dialogueText;
    [SerializeField, TextArea(10, 100)]
    string readyToDepartText;
    [SerializeField, TextArea(10, 100)]
    string searchedDialogueText;

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
            GameTaskText.Instance.SetText(FormatText(_currentTask.returnText));
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

        if (PlayerController.Instance.IsSearched())
        {
            dialogueText.SetText(FormatText(searchedDialogueText));
            return;
        }

        if (currentState == EscapeAgentState.Initial || currentState == EscapeAgentState.WaitingForTaskReturn)
        {
            AdvanceProgress();
        }

        if (currentState == EscapeAgentState.ReadyToDepart)
        {
            PlayerController.Instance.TryEscape();
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
                GameTaskText.Instance.SetText(FormatText(_currentTask.taskText));
                dialogueText.SetText(FormatText(_currentTask.dialogueText));

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
        dialogueText.SetText(FormatText(readyToDepartText));
        blackboardInstance.SetValueAsEnum(EscapeAgentStateKey, EscapeAgentState.ReadyToDepart);
        blackboardInstance.SetValueAsObject(EscapeAreaKey, escapeArea);
    }

    string FormatText(string format)
    {
        format = format.Replace("[" + nameof(escapeArea) + "]", escapeArea.displayName);
        return format;
    }
}