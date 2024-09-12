using Unity.VisualScripting;
using UnityEngine;

public class KeyItemPickUp : PickUpBase
{
    KeyItem item;

    public static void CreateKeyItemPickup(KeyItem item, in Vector3 spawnLocation)
    {
        if (item == null)
        {
            return;
        }

        GameObject createdPickUp = Instantiate(item.pickUpPrefab, spawnLocation, Quaternion.identity);
        WorldMarker worldMarker = createdPickUp.GetComponentInChildren<WorldMarker>();
        if (worldMarker != null)
        {
            worldMarker.AddMarkerSource(createdPickUp, WorldMarkerVisibility.OverheadAndScreenBorder);
        }

        KeyItemPickUp pickUp = createdPickUp.AddComponent<KeyItemPickUp>();
        pickUp.item = item;
    }

    protected override void OnPickedUp(GameObject player)
    {
        if (item == null)
        {
            Debug.LogWarningFormat("KeyItemPickup {0} has no item assigned!", item);
            return;
        }

        PlayerInventory inventory = PlayerController.Instance.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("Player has no inventory!");
        }
        else
        {
            inventory.StoreItem(item);
        }

        Destroy(gameObject);
    }
}
