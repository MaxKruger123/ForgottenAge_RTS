using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Speed of camera movement
    public float panBorderThickness = 10f; // Thickness of border for edge scrolling
    public float scrollSpeed = 20f; // Speed of scroll for zooming
    public float minY = 10f; // Minimum allowed FOV (zoom out)
    public float maxY = 80f; // Maximum allowed FOV (zoom in)

    private bool isPanning = false;
    private Vector3 lastMousePosition;

    void Update()
    {
        // Middle mouse button panning
        if (Input.GetMouseButtonDown(1))
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isPanning = false;
        }
        if (isPanning)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            Vector3 panDirection = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f).normalized;
            transform.Translate(panDirection * panSpeed * Time.deltaTime, Space.World);
            lastMousePosition = Input.mousePosition;
        }

        // Edge scrolling (optional)
        if (isPanning)
        {
            if (Input.mousePosition.y >= Screen.height - panBorderThickness)
                transform.Translate(Vector3.down * panSpeed * Time.deltaTime, Space.World);
            else if (Input.mousePosition.y <= panBorderThickness)
                transform.Translate(Vector3.up * panSpeed * Time.deltaTime, Space.World);

            if (Input.mousePosition.x >= Screen.width - panBorderThickness)
                transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
            else if (Input.mousePosition.x <= panBorderThickness)
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }

        // Scrollwheel zooming
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newSize = Camera.main.fieldOfView - scroll * scrollSpeed;
            newSize = Mathf.Clamp(newSize, minY, maxY);
            Camera.main.fieldOfView = newSize;
        }
    }
}
