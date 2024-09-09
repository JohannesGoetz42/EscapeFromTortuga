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

        GameObject createdPickUp = Instantiate(item.Mesh, spawnLocation, Quaternion.identity);
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

        PlayerController.Instance.KeyRing.Add(item);
        Destroy(gameObject);
    }
}
