using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    private bool isMoving = false; // Flag to track if the unit is currently moving
    public float moveSpeed = 1f; // Speed of troop movement
    private Vector3 targetPosition;
    private Collider2D unitCollider;

    private void Start()
    {
        unitCollider = GetComponent<Collider2D>();
    }

    public void StartMoving()
    {
        isMoving = true;
       
    }

    public void MoveTo(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        isMoving = true;
        
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
                unitCollider.enabled = true; // Re-enable collider when movement is complete
            }
        }
    }

    public void StopMoving()
    {
        isMoving = false;
        unitCollider.enabled = true; // Re-enable collider when unit stops moving
    }

    public void ResumeMoving()
    {
        StartMoving(); // Resuming moving is just starting to move again
    }

    public bool HasReachedDestination()
    {
        return !isMoving; // Assuming the unit stops moving when it reaches the destination
    }
}
