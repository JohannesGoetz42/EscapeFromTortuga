using UnityEngine;

public class CatchPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.Instance.gameObject)
        {
            PlayerController.Instance.GameOver();
        }
    }
}
