using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - PlayerController.Instance.transform.position;
    }

    void LateUpdate()
    {
        transform.position = offset + PlayerController.Instance.transform.position;
    }
}
