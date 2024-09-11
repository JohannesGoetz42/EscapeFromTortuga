using UnityEngine;

public class EscapeArea : MonoBehaviour
{
    /** The display name i.E. used in dialog texts */
    public string displayName;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController.Instance.TryEscape();
        }
    }
}
