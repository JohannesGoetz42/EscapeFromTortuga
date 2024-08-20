using UnityEngine;

public class FacePlayerCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = PlayerController.Instance.MainCamera.transform.rotation;
    }
}
