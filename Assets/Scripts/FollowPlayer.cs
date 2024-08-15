using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Vector3 offset;

    private void Start()
    {
        if (PlayerController.Instance)
        {
            offset = transform.position - PlayerController.Instance.transform.position;
        }
    }

    void LateUpdate()
    {
        if (PlayerController.Instance)
        {
            transform.position = offset + PlayerController.Instance.transform.position;
        }
    }
}
