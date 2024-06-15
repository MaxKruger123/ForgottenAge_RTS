using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTroop : MonoBehaviour
{
    public float attackRange = 2f; // Range for attacking
    public float minDistanceToAlly = 1.5f; // Minimum distance to ally to avoid touching
    public float rangedAttackRange = 5f; // Range for ranged attacking
    public GameObject projectilePrefab; // Projectile prefab to be shot by the ranged enemy
    public float projectileSpeed = 10f; // Speed of the projectile
    public float shootInterval = 1f; // Interval between shots

    private AllyTroopStats targetAlly; // Reference to the nearest ally
    private GameObject targetMemoryTile; // Reference to the nearest MemoryTile
    public GameObject targetBuilding; // Reference to the nearest building
    private bool isAttacking = false; // Flag to indicate if the enemy is attacking
    private NavMeshAgent agent; // Reference to the NavMeshAgent
    private Coroutine shootingCoroutine; // Coroutine for shooting

    private EnemyStats enemyStats; // Reference to the enemy's stats
   

    private CardManager cardManager;

    void Start()
    {
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
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

        // Get the EnemyStats component
        enemyStats = GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (enemyStats.currentHealth < enemyStats.maxHealth)
        {
            FindNearestAlly(); // Prioritize attacking allies if damaged
        }
        else
        {
            FindNearestBuilding(); // Prioritize attacking buildings if not damaged
        }

        if (!isAttacking)
        {
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
                else if (gameObject.CompareTag("Enemy"))
                {
                    // Attack the ally when in range
                    AttackAlly();
                }
                else if (gameObject.CompareTag("EnemyRanged"))
                {
                    // Stop moving and shoot the ally
                    agent.ResetPath();
                    if (shootingCoroutine == null)
                    {
                        shootingCoroutine = StartCoroutine(ShootAlly());
                    }
                }
            }
            else if (targetBuilding != null)
            {
                float distanceToBuilding = Vector3.Distance(transform.position, targetBuilding.transform.position);

                if (gameObject.CompareTag("Enemy") && distanceToBuilding > attackRange || gameObject.CompareTag("EnemyRanged") && distanceToBuilding > rangedAttackRange)
                {
                    if (agent != null && agent.isOnNavMesh)
                    {
                        agent.SetDestination(targetBuilding.transform.position);
                    }
                }
                else if (gameObject.CompareTag("Enemy") && distanceToBuilding <= attackRange)
                {
                    // Attack the building when in range
                    AttackBuilding();
                }
                else if (gameObject.CompareTag("EnemyRanged") && distanceToBuilding <= rangedAttackRange)
                {
                    // Stop moving and shoot the building
                    agent.ResetPath();
                    if (shootingCoroutine == null)
                    {
                        shootingCoroutine = StartCoroutine(ShootBuilding());
                    }
                }
            }
            else
            {
                if (targetAlly == null)
                {
                    FindNearestBuilding();
                }
                FindNearestMemoryTile(); // Find the nearest MemoryTile if no building or ally is found

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

    void FindNearestAlly()
    {
        AllyTroopStats[] allyTroops = FindObjectsOfType<AllyTroopStats>();

        float minDistance = Mathf.Infinity;
        AllyTroopStats nearestAlly = null;

        foreach (AllyTroopStats ally in allyTroops)
        {
            if (ally.gameObject.CompareTag("Player") || ally.gameObject.CompareTag("AllyRanged") || ally.gameObject.CompareTag("AllyHealing") || ally.gameObject.CompareTag("AllyTank"))
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

    void FindNearestBuilding()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        float minDistance = Mathf.Infinity;
        GameObject nearestBuilding = null;

        foreach (GameObject building in buildings)
        {
            float distance = Vector3.Distance(transform.position, building.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestBuilding = building;
            }
        }

        targetBuilding = nearestBuilding;
    }

    void AttackAlly()
    {
        // Perform attack on the ally
        targetAlly.TakeDamage(cardManager.enemyMeleeDamage);

        // Set flag to indicate attacking
        isAttacking = true;

        // Wait for a short duration before resetting the attack flag
        StartCoroutine(ResetAttackFlag());
    }

    void AttackBuilding()
    {
        // Perform attack on the building
        if (targetBuilding != null)
        {
            // Assuming the building has a Health component
            BuildingStats buildingHealth = targetBuilding.GetComponent<BuildingStats>();
            if (buildingHealth != null)
            {
                buildingHealth.TakeDamage(cardManager.enemyMeleeDamage);
            }

            // Set flag to indicate attacking
            isAttacking = true;

            // Wait for a short duration before resetting the attack flag
            StartCoroutine(ResetAttackFlag());
        }
    }

    IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(2.0f); // Adjust the duration as needed
        isAttacking = false;
    }

    IEnumerator ShootAlly()
    {
        while (targetAlly != null)
        {
            Vector3 direction = (targetAlly.transform.position - transform.position).normalized;
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

            yield return new WaitForSeconds(shootInterval);
        }

        shootingCoroutine = null;
    }

    IEnumerator ShootBuilding()
    {
        while (targetBuilding != null)
        {
            Vector3 direction = (targetBuilding.transform.position - transform.position).normalized;
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
