using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Speed of camera movement
    public float panBorderThickness = 10f; // Thickness of border for edge scrolling
    public float scrollSpeed = 20f; // Speed of scroll for zooming
    public float minFOV = 20f; // Minimum allowed FOV (zoom in)
    public float maxFOV = 80f; // Maximum allowed FOV (zoom out)

    // Boundary variables
    public float minX = -50f;
    public float maxX = 50f;
    public float minYBound = -50f;
    public float maxYBound = 50f;

    private bool isPanning = false;
    private Vector3 lastMousePosition;

    void Update()
    {
        // Right mouse button panning
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

            // Clamp the camera position
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minYBound, maxYBound);
            transform.position = pos;
        }

        // Edge scrolling (optional)
        if (!isPanning)
        {
            if (Input.mousePosition.y >= Screen.height - panBorderThickness)
                transform.Translate(Vector3.up * panSpeed * Time.deltaTime, Space.World);
            else if (Input.mousePosition.y <= panBorderThickness)
                transform.Translate(Vector3.down * panSpeed * Time.deltaTime, Space.World);

            if (Input.mousePosition.x >= Screen.width - panBorderThickness)
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
            else if (Input.mousePosition.x <= panBorderThickness)
                transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);

            // Clamp the camera position
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minYBound, maxYBound);
            transform.position = pos;
        }

        // Scrollwheel zooming
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newSize = Camera.main.fieldOfView - scroll * scrollSpeed;
            newSize = Mathf.Clamp(newSize, minFOV, maxFOV);
            Camera.main.fieldOfView = newSize;
        }
    }
}
