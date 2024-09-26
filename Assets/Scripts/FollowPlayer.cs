using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    private float cameraDistance = 30.0f;
    [SerializeField]
    private float cameraAngle = 70.0f;

    public Vector3 cameraOffset { get; private set; }

    void LateUpdate()
    {
        if (PlayerController.Instance)
        {
            Quaternion cameraRotation = Quaternion.Euler(cameraAngle, 0.0f, 0.0f);
            cameraOffset = Vector3.up * cameraDistance;
            cameraOffset = Quaternion.Euler(-90.0f, 0.0f, 0.0f) * (cameraRotation * cameraOffset);

            transform.position = PlayerController.Instance.transform.position + cameraOffset;
            transform.rotation = cameraRotation;
        }
    }
}
