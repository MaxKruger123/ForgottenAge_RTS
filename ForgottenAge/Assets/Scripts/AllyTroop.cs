using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AllyTroop : MonoBehaviour
{
    public float attackRange = 2f; // Range for melee attacking
    public float minDistanceToEnemy = 1.5f; // Minimum distance to enemy to avoid touching
    public GameObject projectilePrefab; // Prefab for the projectile
    public float projectileSpeed = 10f; // Speed of the projectile

    private EnemyStats targetEnemy; // Reference to the nearest enemy
    private bool isAttacking = false; // Flag to indicate if the ally is attacking
    private bool isRanged = false; // Flag to check if the unit is ranged
    private NavMeshAgent agent; // Reference to the NavMeshAgent
    public GameObject targetTile; // Reference to the nearest tile

    void Start()
    {
        // Check if the unit is a ranged ally
        if (gameObject.CompareTag("AllyRanged"))
        {
            isRanged = true;
        }

        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // Ensure the NavMeshAgent operates on the X-Y plane for 2D
            agent.updateUpAxis = false;
            agent.updateRotation = false;
        }
        else
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        }

        FindNearestEnemy();
    }

    void Update()
    {
        if (targetEnemy == null)
        {
            FindNearestEnemy();

            if (targetEnemy == null)
            {
                FindNearestTile();
                MoveTowardsTile();
            }
        }
        else
        {
            MoveTowardsEnemy();
        }
    }

    void FindNearestEnemy()
    {
        EnemyStats[] enemyTroops = FindObjectsOfType<EnemyStats>();

        float minDistance = Mathf.Infinity;
        EnemyStats nearestEnemy = null;

        foreach (EnemyStats enemy in enemyTroops)
        {
            if (enemy.gameObject.CompareTag("Enemy") || enemy.gameObject.CompareTag("EnemyRanged"))
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

    void FindNearestTile()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("MemoryTile");

        float minDistance = Mathf.Infinity;
        GameObject nearestTile = null;

        foreach (GameObject tile in tiles)
        {
            float distance = Vector3.Distance(transform.position, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }

        targetTile = nearestTile;
    }

    void MoveTowardsEnemy()
    {
        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (isRanged)
            {
                if (distanceToEnemy > attackRange)
                {
                    if (agent != null)
                    {
                        agent.SetDestination(targetEnemy.transform.position);
                    }
                }
                else
                {
                    if (!isAttacking)
                    {
                        StartCoroutine(ShootProjectile());
                    }
                }
            }
            else
            {
                if (distanceToEnemy > minDistanceToEnemy)
                {
                    if (agent != null)
                    {
                        agent.SetDestination(targetEnemy.transform.position);
                    }
                }
                else
                {
                    if (!isAttacking)
                    {
                        AttackEnemy();
                    }
                }
            }
        }
    }

    void MoveTowardsTile()
    {
        if (targetTile != null && agent != null)
        {
            float distanceToTile = Vector3.Distance(transform.position, targetTile.transform.position);

            
            if (gameObject.tag == "Player")
            {
                agent.stoppingDistance = 2.0f; // Set a default stopping distance
            }
            else if (gameObject.tag == "AllyRanged")
            {
                agent.stoppingDistance = 5.0f;
            }

            agent.SetDestination(targetTile.transform.position);
        }
    }

    void AttackEnemy()
    {
        if (targetEnemy != null)
        {
            targetEnemy.TakeDamage(1.0f);
            isAttacking = true;
            StartCoroutine(ResetAttackFlag());
        }
    }

    IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
    }

    IEnumerator ShootProjectile()
    {
        isAttacking = true;

        while (isAttacking && targetEnemy != null)
        {
            // Check if the target enemy is still alive
            if (targetEnemy != null && !targetEnemy.IsDead())
            {
                // Create and shoot the projectile
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                if (projectile != null)
                {
                    // Calculate direction to the enemy's center
                    Vector3 direction = (targetEnemy.centerObject.transform.position - transform.position).normalized;

                    // Set velocity of the projectile
                    Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = direction * projectileSpeed;
                    }
                }
            }
            else
            {
                // Find the nearest enemy if the current target is dead
                FindNearestEnemy();
            }

            yield return new WaitForSeconds(1f);
        }

        isAttacking = false;
    }
}
