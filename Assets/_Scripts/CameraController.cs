using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Speed of camera movement
    public float zoomSpeed = 5f; // Speed of zooming in/out
    public float minZoom = 5f;   // Minimum zoom level (orthographic size)
    public float maxZoom = 20f;  // Maximum zoom level (orthographic size)

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        // Get input axes for horizontal and vertical movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Calculate the new position based on input and pan speed
        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0) * panSpeed * Time.deltaTime;

        // Update the camera's position
        transform.position = newPosition;
    }

    void HandleZoom()
    {
        // Get the scroll wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Calculate the new orthographic size based on scroll input and zoom speed
        float newSize = Camera.main.orthographicSize - scroll * zoomSpeed;

        // Clamp the new size to be within the min and max zoom levels
        newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

        // Update the camera's orthographic size
        Camera.main.orthographicSize = newSize;
    }
}
