using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoTool : MonoBehaviour
{
    private bool isSelecting = false;
    private Vector3 mousePosition1;
    private GameObject selectionBox;

    public GameObject selectionBoxPrefab; // Assign a prefab of a transparent grey box in the Unity inspector

    
    private SpriteRenderer spriteRenderer;

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
            // Reset selection state
            isSelecting = false;

            // Check if the mouse click hits a collider
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider == null)
            {
                // Mouse click didn't hit any collider, deselect all units
                DeselectAllUnits();
            }
            else
            {
                // Check if the clicked object is a selectable unit
                 SelectableUnit selectableUnit = hit.collider.GetComponent<SelectableUnit>();
                if (selectableUnit != null)
                {
                    DeselectAllUnits();
                    // Handle direct click selection
                    SelectUnit(selectableUnit.gameObject);
                }
            }

            // Destroy the selection box object
            Destroy(selectionBox);
        }

        // Check for right mouse button click
        if (Input.GetMouseButtonDown(1))
        {
            // Check if a troop is selected
            foreach (SelectableUnit selectableUnit in FindObjectsOfType<SelectableUnit>())
            {
                spriteRenderer = selectableUnit.GetComponent<SpriteRenderer>();

                if (spriteRenderer.color == Color.green)
                {

                    
                    // Start moving the selected troop
                    selectableUnit.StartMoving();

                    // Move the selected troop to the clicked position
                    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    targetPosition.z = 0f; // Ensure z position is correct

                    MoveSelectedTroopsTo(targetPosition, 1.5f);
                }
            }
        }
    }

    private void MoveSelectedTroopsTo(Vector3 targetPosition, float offsetRange)
    {
        List<SelectableUnit> selectedUnits = new List<SelectableUnit>();

        // Find and store selected units
        foreach (SelectableUnit selectableUnit in FindObjectsOfType<SelectableUnit>())
        {
            spriteRenderer = selectableUnit.GetComponent<SpriteRenderer>();
            if (spriteRenderer.color == Color.green)
            {
                selectedUnits.Add(selectableUnit);
            }
        }

        // Calculate the size of the square formation based on the number of selected troops
        int squareSize = Mathf.CeilToInt(Mathf.Sqrt(selectedUnits.Count));

        // Calculate the offset between troops in the square formation
        float xOffset = offsetRange * (squareSize - 1) / 2;
        float yOffset = offsetRange * (squareSize - 1) / 2;

        // Move the troops to the adjusted positions in a square formation
        int i = 0;
        foreach (SelectableUnit selectableUnit in selectedUnits)
        {
            // Calculate the position in the square formation
            int row = i / squareSize;
            int col = i % squareSize;

            // Calculate the adjusted position
            Vector3 adjustedPosition = targetPosition + new Vector3(col * offsetRange - xOffset, row * offsetRange - yOffset, 0f);

            // Move the troop to the adjusted position
            selectableUnit.MoveTo(adjustedPosition);

            Debug.Log("Moving " + selectableUnit.name + " to position: " + adjustedPosition);

            // Increment the index
            i++;
        }
    }

    private Vector3 AvoidTroopCollision(Vector3 position, List<SelectableUnit> selectedUnits, SelectableUnit currentUnit)
    {
        // Check for collisions with other troops and adjust position accordingly
        foreach (SelectableUnit unit in selectedUnits)
        {
            if (unit != null && unit != currentUnit && Vector3.Distance(unit.transform.position, position) < 1.0f)
            {
                // Calculate a new position that avoids the collision
                Vector3 direction = (position - unit.transform.position).normalized;
                position = unit.transform.position + direction * 1.0f;
            }
        }

        return position;
    }

    private bool AllTroopsReachedDestination(List<SelectableUnit> troops)
    {
        foreach (SelectableUnit troop in troops)
        {
            if (!troop.HasReachedDestination())
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator StandStillForTwoSeconds(SelectableUnit unit)
    {
        unit.StopMoving(); // Stop the unit's movement
        yield return new WaitForSeconds(2.0f); // Wait for 2 seconds
        unit.ResumeMoving(); // Resume the unit's movement
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SelectableUnit"))
        {
            // Unit entered the selection box
            SelectUnit(other.gameObject);
            
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SelectableUnit"))
        {
            
        }
    }

    private void SelectUnit(GameObject unitObject)
    {
        // Change color or apply selection effect to the unit
        // For example:
        SpriteRenderer unitRenderer = unitObject.GetComponent<SpriteRenderer>();
        if (unitRenderer != null)
        {
            unitRenderer.color = Color.green;
        }
    }

    private void DeselectAllUnits()
    {
        // Loop through all selectable units
        foreach (SelectableUnit selectableUnit in FindObjectsOfType<SelectableUnit>())
        {
            // Deselect the unit
            Deselect(selectableUnit.gameObject);
        }
    }

    private void Deselect(GameObject unitObject)
    {
        // Revert the color or selection effect of the unit
        // For example:
        SpriteRenderer unitRenderer = unitObject.GetComponent<SpriteRenderer>();
        if (unitRenderer != null)
        {
            unitRenderer.color = Color.blue; // Or revert to original color
        }
    }
}
