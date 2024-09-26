using UnityEngine;

public class FadeWhenPlayerIsClose : MonoBehaviour
{
    [SerializeField]
    MeshRenderer defaultMesh;
    [SerializeField]
    MeshRenderer fadedMesh;
    [SerializeField]
    float triggerRange = 10;

    SphereCollider _trigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fadedMesh == null || defaultMesh == null)
        {
            Debug.LogWarningFormat("{0} on '{1}'has invalid fields and will be deactivated", nameof(FadeWhenPlayerIsClose), name);
            return;
        }

        if (triggerRange > 0)
        {
            _trigger = gameObject.AddComponent<SphereCollider>();
            _trigger.radius = triggerRange;
            _trigger.isTrigger = true;
        }

        Fade(true);
    }

    public void Fade(bool fadeIn)
    {
        if (defaultMesh != null && fadedMesh != null)
        {
            defaultMesh.enabled = fadeIn;
            fadedMesh.enabled = !fadeIn;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Fade(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Fade(true);
        }
    }
}
