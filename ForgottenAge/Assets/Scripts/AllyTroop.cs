using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyTroop : MonoBehaviour
{
    public float attackRange = 2f; // Range for attacking
    public float minDistanceToEnemy = 1.5f; // Minimum distance to enemy to avoid touching
    public float rangedAttackRange = 5f; // Range for ranged attacking
    public GameObject projectilePrefab; // Projectile prefab to be shot by the ranged ally
    public float projectileSpeed = 10f; // Speed of the projectile
    public float shootInterval = 1f; // Interval between shots

    private EnemyTroop targetEnemy; // Reference to the nearest enemy
    private GameObject targetMemoryTile; // Reference to the nearest MemoryTile
    private bool isAttacking = false; // Flag to indicate if the ally is attacking
    private NavMeshAgent agent; // Reference to the NavMeshAgent
    private Coroutine shootingCoroutine; // Coroutine for shooting

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
                Debug.LogError("AllyTroop " + gameObject.name + " is not on a NavMesh!");
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
            FindNearestEnemy(); // Find the nearest enemy troop

            if (targetEnemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

                // Move towards the enemy, but maintain minimum distance
                if (distanceToEnemy > minDistanceToEnemy)
                {
                    if (agent != null && agent.isOnNavMesh)
                    {
                        agent.SetDestination(targetEnemy.transform.position);
                    }
                }
                else if (gameObject.CompareTag("Player") || gameObject.CompareTag("AllyRanged"))
                {
                    // Attack the enemy when in range
                    AttackEnemy();
                }
                else if (gameObject.CompareTag("AllyRanged"))
                {
                    // Stop moving and shoot the enemy
                    agent.ResetPath();
                    if (shootingCoroutine == null)
                    {
                        shootingCoroutine = StartCoroutine(ShootEnemy());
                    }
                }
            }
            else
            {
                // Look for the nearest uncaptured tile
                FindNearestUncapturedTile();

                if (targetMemoryTile != null)
                {
                    if (agent != null && agent.isOnNavMesh)
                    {
                        agent.SetDestination(targetMemoryTile.transform.position);
                    }
                }
                else
                {
                    FindNearestMemoryTile(); // Find the nearest MemoryTile if no uncaptured tile is found

                    if (targetMemoryTile != null)
                    {
                        if (agent != null && agent.isOnNavMesh)
                        {
                            agent.SetDestination(targetMemoryTile.transform.position);
                        }
                    }
                }
            }
        }
    }

    void FindNearestEnemy()
    {
        EnemyTroop[] enemyTroops = FindObjectsOfType<EnemyTroop>();

        float minDistance = Mathf.Infinity;
        EnemyTroop nearestEnemy = null;

        foreach (EnemyTroop enemy in enemyTroops)
        {
            if (!enemy.gameObject.CompareTag("EnemyHealing"))
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        targetEnemy = nearestEnemy;
    }

    void FindNearestUncapturedTile()
    {
        CaptureZone[] captureZones = FindObjectsOfType<CaptureZone>();

        float minDistance = Mathf.Infinity;
        GameObject nearestUncapturedTile = null;

        foreach (CaptureZone zone in captureZones)
        {
            if (!zone.captured)
            {
                float distance = Vector3.Distance(transform.position, zone.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestUncapturedTile = zone.gameObject;
                }
            }
        }

        targetMemoryTile = nearestUncapturedTile;
    }

    void FindNearestMemoryTile()
    {
        GameObject[] memoryTiles = GameObject.FindGameObjectsWithTag("MemoryTile");

        float minDistance = Mathf.Infinity;
        GameObject nearestTile = null;

        foreach (GameObject tile in memoryTiles)
        {
            float distance = Vector3.Distance(transform.position, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }

        targetMemoryTile = nearestTile;
    }

    void AttackEnemy()
    {
        // Perform attack on the enemy
        EnemyStats enemyStats = targetEnemy.GetComponent<EnemyStats>();
        enemyStats.TakeDamage(1.0f);

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

    IEnumerator ShootEnemy()
    {
        while (targetEnemy != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

            yield return new WaitForSeconds(shootInterval);
        }

        shootingCoroutine = null;
    }

    private void OnDestroy()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }
    }
}
