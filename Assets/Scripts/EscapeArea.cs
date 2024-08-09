using UnityEngine;

public class EscapeArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController.Instance.TryEscape();
        }
    }
}
