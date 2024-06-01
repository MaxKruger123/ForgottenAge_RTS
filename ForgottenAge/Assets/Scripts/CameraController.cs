using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Speed of camera movement
    public float panBorderThickness = 10f; // Thickness of border for edge scrolling

    private bool isPanning = false;
    private Vector3 lastMousePosition;

    void Update()
    {
        // Check if the middle mouse button is pressed
        if (Input.GetMouseButtonDown(2))
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }

        // Check if the middle mouse button is released
        if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }

        // If the middle mouse button is held down, pan the camera
        if (isPanning)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            Vector3 panDirection = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f).normalized; // Inverted controls

            // Move the camera based on mouse movement
            transform.Translate(panDirection * panSpeed * Time.deltaTime, Space.World);

            // Update the last mouse position
            lastMousePosition = Input.mousePosition;
        }

        // Edge scrolling (optional)
        if (isPanning)
        {
            if (Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                transform.Translate(Vector3.down * panSpeed * Time.deltaTime, Space.World); // Inverted controls
            }
            else if (Input.mousePosition.y <= panBorderThickness)
            {
                transform.Translate(Vector3.up * panSpeed * Time.deltaTime, Space.World); // Inverted controls
            }

            if (Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World); // Inverted controls
            }
            else if (Input.mousePosition.x <= panBorderThickness)
            {
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World); // Inverted controls
            }
        }
    }
}
