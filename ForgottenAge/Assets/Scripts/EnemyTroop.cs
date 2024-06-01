using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTroop : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of enemy movement
    public float attackRange = 2f; // Range for attacking
    public float minDistanceToAlly = 1.5f; // Minimum distance to ally to avoid touching

    private AllyTroopStats targetAlly; // Reference to the nearest ally
    private bool isAttacking = false; // Flag to indicate if the enemy is attacking

    void Update()
    {
        if (!isAttacking)
        {
            FindNearestAlly(); // Find the nearest ally troop

            if (targetAlly != null)
            {
                Vector3 direction = (targetAlly.transform.position - transform.position).normalized;
                float distanceToAlly = Vector3.Distance(transform.position, targetAlly.transform.position);

                // Move towards the ally, but maintain minimum distance
                if (distanceToAlly > minDistanceToAlly)
                {
                    transform.Translate(direction * moveSpeed * Time.deltaTime);
                }
                else
                {
                    // Attack the ally when in range
                    AttackAlly();
                }
            }
        }
    }

    void FindNearestAlly()
    {
        AllyTroopStats[] allyTroops = FindObjectsOfType<AllyTroopStats>();

        float minDistance = Mathf.Infinity;
        AllyTroopStats nearestAlly = null;

        foreach (AllyTroopStats ally in allyTroops)
        {
            if (ally.gameObject.CompareTag("SelectableUnit"))
            {
                float distance = Vector3.Distance(transform.position, ally.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAlly = ally;
                }
            }
        }
        
        targetAlly = nearestAlly;
    }

    void AttackAlly()
    {
        // Perform attack on the ally
        
        targetAlly.TakeDamage(1.0f);

        // Set flag to indicate attacking
        isAttacking = true;

        // Wait for a short duration before resetting the attack flag
        StartCoroutine(ResetAttackFlag());
    }

    IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(2.0f); // Adjust the duration as needed
        isAttacking = false;
    }
}
