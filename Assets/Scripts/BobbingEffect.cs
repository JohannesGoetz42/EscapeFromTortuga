using UnityEngine;

public class BobbingEffect : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 20.0f;
    [SerializeField]
    float bobbingSpeed = 1.0f;
    [SerializeField]
    float bobbingDistance = 0.2f;

    private void Awake()
    {
        transform.position += Vector3.up * bobbingDistance * 2;
    }

    private void Update()
    {
        // rotate
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // move up and down
        float bobbingOffset = bobbingDistance * Mathf.Sin(Time.time * bobbingSpeed) * Time.deltaTime;
        transform.position += Vector3.up * bobbingOffset;
    }
}
