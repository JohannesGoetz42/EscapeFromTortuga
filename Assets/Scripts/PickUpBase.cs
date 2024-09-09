using Unity.VisualScripting;
using UnityEngine;

public abstract class PickUpBase : MonoBehaviour
{
    [SerializeField]
    float PickUpRange = 1.0f;

    SphereCollider _trigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _trigger = transform.AddComponent<SphereCollider>();
        _trigger.isTrigger = true;
    }

    protected abstract void OnPickedUp(GameObject player);

    private void OnTriggerEnter(Collider other)
    {
        // if player walks in, apply effect
        if (other.gameObject.CompareTag("Player"))
        {
            OnPickedUp(other.gameObject);
        }
    }
}
