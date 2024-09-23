using UnityEngine;


public class EscapeAgent : EscapeAgentBase
{
    [field: SerializeField]
    public EscapeArea escapeArea { get; private set; }
    [SerializeField]
    EscapeAgentTask[] tasks;
    [SerializeField, TextArea(10, 100)]
    string searchedDialogueText;

    EscapeAgentTask _currentTask;
    int _currentTaskIndex = -1;

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
        if (currentState == EscapeAgentState.ReadyToDepart)
        {
            return;
        }

        if (PlayerController.Instance.IsSearched())
        {
            dialogueText.SetText(FormatText(searchedDialogueText));
            return;
        }

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
        escapeArea.SetReadyToDepart();
    }

    string FormatText(string format)
    {
        format = format.Replace("[" + nameof(escapeArea) + "]", escapeArea.DisplayName);
        return format;
    }
}