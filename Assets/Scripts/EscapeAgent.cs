using Unity.VisualScripting;
using UnityEngine;

enum EscapeAgentState
{
    Initial,
    RequirementMissing,
    WaitingForRequirementDelivery,
    ReadyToDepart
}

public class EscapeAgent : RunBehaviorTree
{
    const string EscapeAgentStateKey = "EscapeAgentState";

    [SerializeField]
    EscapeArea escapeArea;
    [SerializeField]
    KeyItem requiredItem;
    [SerializeField]
    float interactionRange = 4.0f;

    SphereCollider _trigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        _trigger = transform.AddComponent<SphereCollider>();
        _trigger.radius = interactionRange;
        _trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        EscapeAgentState currentState = blackboardInstance.GetValueAsEnum<EscapeAgentState>(EscapeAgentStateKey);
        switch (currentState)
        {
            case EscapeAgentState.Initial:
                WaitForRequirement();
                break;
            case EscapeAgentState.WaitingForRequirementDelivery:
                blackboardInstance.SetValueAsEnum(EscapeAgentStateKey, EscapeAgentState.ReadyToDepart);
                break;
        }

    }

    private void WaitForRequirement()
    {
        KeyItemPickUp.CreateKeyItemPickup(requiredItem, new Vector3(0, 1, 0));

        PlayerInventory inventory = PlayerController.Instance.GetComponent<PlayerInventory>();
        inventory.keyItemStored += OnPlayerItemStored;

        blackboardInstance.SetValueAsEnum(EscapeAgentStateKey, EscapeAgentState.WaitingForRequirementDelivery);
    }

    private void OnPlayerItemStored(KeyItem item)
    {
        if (item == requiredItem && blackboardInstance.GetValueAsEnum<EscapeAgentState>(EscapeAgentStateKey) == EscapeAgentState.RequirementMissing)
        {
            PlayerInventory inventory = PlayerController.Instance.GetComponent<PlayerInventory>();
            inventory.keyItemStored -= OnPlayerItemStored;
            blackboardInstance.SetValueAsEnum(EscapeAgentStateKey, EscapeAgentState.WaitingForRequirementDelivery);
        }
    }
}
