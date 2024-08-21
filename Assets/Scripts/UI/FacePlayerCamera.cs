using UnityEngine;

public class FacePlayerCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.MainCamera != null)
        {
            transform.rotation = PlayerController.Instance.MainCamera.transform.rotation;
        }
    }
}
