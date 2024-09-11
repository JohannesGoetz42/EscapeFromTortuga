using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EscapeAgentTask", menuName = "Scriptable Objects/EscapeAgentTask")]
public class EscapeAgentTask : ScriptableObject
{
    [field: SerializeField]
    public string taskText { get; private set; }
    [field: SerializeField]
    public string returnText { get; private set; }
    [field: SerializeField]
    public string dialogueText { get; private set; }
    [SerializeField]
    KeyItem itemToCollect;

    List<Object> _objectives;
    EscapeAgent _escapeAgent;

    public bool SetUp(EscapeAgent escapeAgent)
    {
        bool success = false;
        _objectives = new List<Object>();
        _escapeAgent = escapeAgent;

        // set up item to collect
        if (itemToCollect != null)
        {
            KeyItemPickUp.CreateKeyItemPickup(itemToCollect, new Vector3(0, 1, 0));
            PlayerInventory Inventory = PlayerController.Instance.GetComponentInChildren<PlayerInventory>();
            if (Inventory != null)
            {
                Inventory.keyItemStored += OnItemStored;
                success = true;
            }
        }

        return success;
    }

    void OnItemStored(KeyItem item)
    {
        if (itemToCollect == item)
        {
            PlayerInventory Inventory = PlayerController.Instance.GetComponentInChildren<PlayerInventory>();
            if (Inventory != null)
            {
                Inventory.keyItemStored -= OnItemStored;
            }

            _objectives.Remove(item);
            TryCompleteTask();
        }
    }

    void TryCompleteTask()
    {
        if (_objectives.Count == 0)
        {
            _escapeAgent.OnTaskCompleted(this);
        }
    }
}