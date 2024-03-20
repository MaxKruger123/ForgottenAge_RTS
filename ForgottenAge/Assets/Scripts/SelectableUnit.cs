using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{

    private bool isMoving = false; // Flag to track if the unit is currently moving


    // Speed of troop movement
    public float moveSpeed = 1f;

    // Target position for movement
    private Vector3 targetPosition;



    private Collider2D collider;
  

    private void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    public void StartMoving()
    {
        
        isMoving = true;
        collider.enabled = false; // Disable collider when unit starts moving
    }

    public void MoveTo(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        isMoving = true;
        // Disable collider while moving
        collider.enabled = false;
    }

    private void Update()
    {
        if (isMoving)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (transform.position == targetPosition)
            {
                isMoving = false;
                // Re-enable collider when movement is complete
                collider.enabled = true;
            }
        }
    }

    public void StopMoving()
    {
        isMoving = false;
        collider.enabled = true; // Re-enable collider when unit stops moving
    }

    public void ResumeMoving()
    {
        StartMoving(); // Resuming moving is just starting to move again
    }

    public bool HasReachedDestination()
    {
        // Implement logic to check if the unit has reached its destination
        // Return true if the unit has reached its destination, false otherwise
        return !isMoving; // Example: assuming the unit stops moving when it reaches the destination
    }


}
