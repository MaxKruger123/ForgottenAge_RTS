using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isSelecting = false;
    private Vector3 mousePosition1;
    private GameObject selectionBox;

    public GameObject selectionBoxPrefab; // Assign a prefab of a transparent grey box in the Unity inspector

    void Start()
    {
        // Get the SpriteRenderer component of the object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store the original color of the object
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Store the first mouse position for selection box
            isSelecting = true;
            mousePosition1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Instantiate selection box prefab
            selectionBox = Instantiate(selectionBoxPrefab, mousePosition1, Quaternion.identity);
        }

        // Update the selection box
        if (isSelecting)
        {
            Vector3 mousePosition2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 center = (mousePosition1 + mousePosition2) / 2f;

            // Calculate size based on mouse position
            float width = Mathf.Abs(mousePosition1.x - mousePosition2.x);
            float height = Mathf.Abs(mousePosition1.y - mousePosition2.y);

            // Set position and size of selection box
            selectionBox.transform.position = center;
            selectionBox.transform.localScale = new Vector3(width, height, 1f);
        }

        // Check for mouse release
        if (Input.GetMouseButtonUp(0))
        {
            // Reset selection box
            isSelecting = false;

            // Get the bounds of the selection box
            Bounds selectionBounds = selectionBox.GetComponent<SpriteRenderer>().bounds;
            Debug.Log("Selection Box Bounds: " + selectionBounds);

            // Loop through all selectable units
            foreach (SelectableUnit selectableUnit in FindObjectsOfType<SelectableUnit>())
            {
                // Get the bounds of the current unit
                Bounds unitBounds = selectableUnit.GetComponent<SpriteRenderer>().bounds;
                Debug.Log("Unit Bounds: " + unitBounds);

                // Check if the bounds of the unit overlap with the bounds of the selection box
                if (unitBounds.Intersects(selectionBounds))
                {
                    // Unit's bounds intersect with the selection box
                    Debug.Log("Unit selected: " + selectableUnit.gameObject.name);
                    SelectUnit(selectableUnit);
                }
            }

            // Destroy the selection box object
            Destroy(selectionBox);
        }
    }

    void SelectUnit(SelectableUnit selectableUnit)
    {
        // Change color to green
        selectableUnit.spriteRenderer.color = Color.green;
    }
}
