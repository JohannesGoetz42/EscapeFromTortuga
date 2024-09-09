using Unity.VisualScripting;
using UnityEngine;

public class EscapeHelper : MonoBehaviour
{
    [SerializeField]
    EscapeArea escapeArea;
    [SerializeField]
    KeyItem requiredItem;
    [SerializeField]
    float interactionRange = 4.0f;

    SphereCollider _trigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _trigger = transform.AddComponent<SphereCollider>();
        _trigger.radius = interactionRange;
        _trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            KeyItemPickUp.CreateKeyItemPickup(requiredItem, new Vector3(0, 1, 0));
        }
    }
}
