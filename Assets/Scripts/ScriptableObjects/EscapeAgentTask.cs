using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EscapeAgentTask", menuName = "Scriptable Objects/EscapeAgentTask")]
public class EscapeAgentTask : ScriptableObject
{
    [field: SerializeField]
    public string taskText { get; private set; }
    [field: SerializeField]
    public string returnText { get; private set; }
    [field: SerializeField, TextArea(10, 100)]
    public string dialogueText { get; private set; }
    [SerializeField]
    KeyItem itemToCollect;
    [SerializeField]
    EscapeCrew[] escapeCrew;

    List<Object> _objectives;
    EscapeAgent _escapeAgent;
    bool HasObjectives() => itemToCollect != null || (escapeCrew != null && escapeCrew.Length > 0);
    public bool SetUp(EscapeAgent escapeAgent)
    {
        _objectives = new List<Object>();
        _escapeAgent = escapeAgent;

        if (!HasObjectives())
        {
            Debug.LogWarningFormat("{0} '{1}' has no objectives!", nameof(EscapeAgentTask), name);
            return false;
        }

        // set up item to collect
        if (itemToCollect != null)
        {
            GameObject spawnLocation = SpawnLocation.GetRandomSpawnLocation(SpawnLocationType.Item);
            if (spawnLocation == null)
            {
                return false;
            }
            KeyItemPickUp initializedPickUp = KeyItemPickUp.CreateKeyItemPickup(itemToCollect, spawnLocation.transform.position, escapeAgent.markerColor);
            PlayerInventory Inventory = PlayerController.Instance.GetComponentInChildren<PlayerInventory>();
            if (Inventory == null)
            {
                return false;
            }

            Inventory.keyItemStored += OnItemStored;
            _objectives.Add(itemToCollect);
        }

        // set up escape crew
        if (escapeCrew != null && escapeCrew.Length > 0)
        {
            List<GameObject> spawnLocations = SpawnLocation.GetRandomSpawnLocations(SpawnLocationType.CrewMate, escapeCrew.Length);
            for (int i = 0; i < spawnLocations.Count; i++)
            {
                EscapeCrew initializedCrewMate = EscapeCrew.InitializeEscapeCrew(escapeCrew[i], _escapeAgent, spawnLocations[i].transform.position);
                if (initializedCrewMate == null)
                {
                    return false;
                }

                _objectives.Add(initializedCrewMate);
                initializedCrewMate.crewMateReadyToDepart += OnCrewMateReadyToDepart;
            }
        }

        return true;
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

    void OnCrewMateReadyToDepart(EscapeCrew crewMate)
    {
        if (_objectives.Remove(crewMate))
        {
            crewMate.crewMateReadyToDepart -= OnCrewMateReadyToDepart;
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