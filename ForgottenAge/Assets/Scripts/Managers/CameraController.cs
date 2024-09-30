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
    private bool hasMoved = false;

    private TutorialManager.CameraMovementType requiredMovement = TutorialManager.CameraMovementType.None;
    private bool hasPerformedRightClickPan = false;
    private bool hasPerformedEdgePan = false;
    private bool hasPerformedZoom = false;

    private bool isPanningEnabled = true;  // Changed to true by default
    private bool isTutorialMode = false;  // New field to check if we're in tutorial mode
    private TutorialManager tutorialManager;  // New field

    public float fogBuffer = 5f; // Buffer zone for fog visibility

    void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
        isTutorialMode = tutorialManager != null;
        isPanningEnabled = true;
    }

    void Update()
    {
        if (!isTutorialMode || isPanningEnabled)
        {
            HandleRightClickPanning();
            HandleEdgePanning();
            HandleZooming();
        }
        else if (isTutorialMode)
        {
            HandleTutorialMovement();
        }

        ClampCameraPosition();
    }

    void HandleRightClickPanning()
    {
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
            if (mouseDelta.magnitude > 5f) // Threshold to detect significant movement
            {
                Vector3 panDirection = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f);
                transform.Translate(panDirection * panSpeed * Time.deltaTime, Space.World);
                lastMousePosition = Input.mousePosition;
                if (isTutorialMode && requiredMovement == TutorialManager.CameraMovementType.RightClick)
                {
                    hasPerformedRightClickPan = true;
                }
            }
        }
    }

    void HandleEdgePanning()
    {
        Vector3 movement = Vector3.zero;
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
            movement += Vector3.up;
        else if (Input.mousePosition.y <= panBorderThickness)
            movement += Vector3.down;
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
            movement += Vector3.right;
        else if (Input.mousePosition.x <= panBorderThickness)
            movement += Vector3.left;

        if (movement != Vector3.zero)
        {
            transform.Translate(movement * panSpeed * Time.deltaTime, Space.World);
            if (isTutorialMode && requiredMovement == TutorialManager.CameraMovementType.EdgePan)
            {
                hasPerformedEdgePan = true;
            }
        }
    }

    void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newSize = Camera.main.fieldOfView - scroll * scrollSpeed;
            newSize = Mathf.Clamp(newSize, minFOV, maxFOV);
            Camera.main.fieldOfView = newSize;
            if (isTutorialMode && requiredMovement == TutorialManager.CameraMovementType.Zoom)
            {
                hasPerformedZoom = true;
            }
        }
    }

    void HandleTutorialMovement()
    {
        // This method will handle movement specifically for tutorial mode
        // It's similar to the old Update method, but only for tutorial-specific actions
        if (requiredMovement == TutorialManager.CameraMovementType.RightClick)
        {
            HandleRightClickPanning();
        }
        else if (requiredMovement == TutorialManager.CameraMovementType.EdgePan)
        {
            HandleEdgePanning();
        }
        else if (requiredMovement == TutorialManager.CameraMovementType.Zoom)
        {
            HandleZooming();
        }
    }

    void ClampCameraPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX + fogBuffer, maxX - fogBuffer);
        pos.y = Mathf.Clamp(pos.y, minYBound + fogBuffer, maxYBound - fogBuffer);
        transform.position = pos;
    }

    public void SetAllowedMovement(TutorialManager.CameraMovementType movementType)
    {
        requiredMovement = movementType;
        hasPerformedRightClickPan = false;
        hasPerformedEdgePan = false;
        hasPerformedZoom = false;
        isPanningEnabled = (movementType == TutorialManager.CameraMovementType.RightClick ||
                            movementType == TutorialManager.CameraMovementType.EdgePan);
    }

    public bool HasCompletedRequiredMovement()
    {
        switch (requiredMovement)
        {
            case TutorialManager.CameraMovementType.None:
                return true;
            case TutorialManager.CameraMovementType.RightClick:
                return hasPerformedRightClickPan;
            case TutorialManager.CameraMovementType.EdgePan:
                return hasPerformedEdgePan;
            case TutorialManager.CameraMovementType.Zoom:
                return hasPerformedZoom;
            default:
                return false;
        }
    }

    public void ResetMovementFlag()
    {
        hasPerformedRightClickPan = false;
        hasPerformedEdgePan = false;
        hasPerformedZoom = false;
    }

    // Call this method when switching to non-tutorial scene
    public void DisableTutorialMode()
    {
        isTutorialMode = false;
        isPanningEnabled = true;
        requiredMovement = TutorialManager.CameraMovementType.None;
    }
}
