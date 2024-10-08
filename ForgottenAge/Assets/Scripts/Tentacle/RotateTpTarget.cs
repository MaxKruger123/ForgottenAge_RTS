using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    public float rotationSpeed = 5f; // Adjust this value to control rotation speed
    public float moveSpeed = 5f; // Adjust this value to control movement speed

    void Update()
    {
        // Get the mouse position in world coordinates
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the current position to the mouse position
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        // Calculate the angle needed to look at the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Create a rotation based on that angle
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Smoothly rotate toward the target
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // Move toward the mouse position
        transform.position = Vector2.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);
    }
}
