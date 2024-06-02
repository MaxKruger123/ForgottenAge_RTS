using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTroop : MonoBehaviour
{
    public float attackRange = 2f; // Range for attacking
    public float minDistanceToAlly = 1.5f; // Minimum distance to ally to avoid touching

    private AllyTroopStats targetAlly; // Reference to the nearest ally
    private bool isAttacking = false; // Flag to indicate if the enemy is attacking
    private NavMeshAgent agent; // Reference to the NavMeshAgent

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // Ensure the NavMeshAgent operates on the X-Y plane for 2D
            agent.updateUpAxis = false;
            agent.updateRotation = false;

            // Check if the agent is on the NavMesh
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position); // Place the agent on the NavMesh
            }
            else
            {
                Debug.LogError("EnemyTroop " + gameObject.name + " is not on a NavMesh!");
            }
        }
        else
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (!isAttacking)
        {
            FindNearestAlly(); // Find the nearest ally troop

            if (targetAlly != null)
            {
                float distanceToAlly = Vector3.Distance(transform.position, targetAlly.transform.position);

                // Move towards the ally, but maintain minimum distance
                if (distanceToAlly > minDistanceToAlly)
                {
                    if (agent != null && agent.isOnNavMesh)
                    {
                        agent.SetDestination(targetAlly.transform.position);
                    }
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
            if (ally.gameObject.CompareTag("Player") || ally.gameObject.CompareTag("AllyRanged"))
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
