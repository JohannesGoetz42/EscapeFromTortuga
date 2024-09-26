using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float cameraDistance = 30.0f;
    public float cameraAngle = 70.0f;

    void LateUpdate()
    {
        if (PlayerController.Instance)
        {
            Quaternion cameraRotation = Quaternion.Euler(cameraAngle, 0.0f, 0.0f);
            Vector3 cameraOffset = Vector3.up * cameraDistance;
            cameraOffset = Quaternion.Euler(-90.0f, 0.0f, 0.0f) * (cameraRotation * cameraOffset);

            transform.position = PlayerController.Instance.transform.position + cameraOffset;
            transform.rotation = cameraRotation;
        }
    }
}
