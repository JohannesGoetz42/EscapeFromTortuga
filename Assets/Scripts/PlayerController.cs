using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movementVector = Vector3.Normalize(new Vector3(horizontalInput, 0.0f, verticalInput));

        transform.position += movementVector * Time.deltaTime * movementSpeed;
    }
}
