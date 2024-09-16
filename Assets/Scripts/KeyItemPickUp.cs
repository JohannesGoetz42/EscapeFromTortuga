using Unity.VisualScripting;
using UnityEngine;

public class KeyItemPickUp : PickUpBase, IHasThumbnail
{
    KeyItem item;

    public Sprite Thumbnail => item == null ? null : item.Thumbnail;

    public static KeyItemPickUp CreateKeyItemPickup(KeyItem item, in Vector3 spawnLocation)
    {
        if (item == null || item.pickUpPrefab == null || item.pickUpPrefab.GetComponent<KeyItemPickUp>() == null)
        {
            return null;
        }

        GameObject createdPickUp = Instantiate(item.pickUpPrefab, spawnLocation, Quaternion.identity);
        WorldMarker worldMarker = createdPickUp.GetComponentInChildren<WorldMarker>();
        KeyItemPickUp pickUp = createdPickUp.GetComponent<KeyItemPickUp>();
        pickUp.item = item;

        if (worldMarker != null)
        {
            worldMarker.AddMarkerSource(createdPickUp.GetComponent<KeyItemPickUp>(), WorldMarkerVisibility.OverheadAndScreenBorder);
        }

        return pickUp;
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
