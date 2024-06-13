using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AllyTroop : MonoBehaviour
{
    public float attackRange = 2f; // Range for healing range
    public float minDistanceToEnemy = 1.5f; // Minimum distance to enemy to avoid touching
    public GameObject projectilePrefab; // Prefab for the attack projectile
    public GameObject healingProjectilePrefab; // Prefab for the healing projectile
    public float projectileSpeed = 10f; // Speed of the projectile

    private EnemyStats targetEnemy; // Reference to the nearest enemy
    private AllyTroopStats targetAlly; // Reference to the nearest ally for healing
    private bool isAttacking = false; // Flag to indicate if the ally is attacking
    private bool isRanged = false; // Flag to check if the unit is ranged
    private bool isHealing = false; // Flag to check if the unit is a healing troop
    private NavMeshAgent agent; // Reference to the NavMeshAgent
    public GameObject targetTile; // Reference to the nearest tile

    void Start()
    {
        // Check if the unit is a ranged ally or healing ally
        if (gameObject.CompareTag("AllyRanged"))
        {
            isRanged = true;
        }
        else if (gameObject.CompareTag("AllyHealing"))
        {
            isHealing = true;
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

        if (isHealing)
        {
            FindLowestHealthAlly();
        }
        else
        {
            FindNearestEnemy();
        }
    }

    void Update()
    {
        if (isHealing)
        {
            if (targetAlly == null)
            {
                FindLowestHealthAlly();

                if (targetAlly == null)
                {
                    FindNearestTile();
                    MoveTowardsTile();
                }
            }
            else
            {
                MoveTowardsAlly();
            }
        }
        else
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

    void FindLowestHealthAlly()
    {
        AllyTroopStats[] allyTroops = FindObjectsOfType<AllyTroopStats>();

        float minHealth = Mathf.Infinity;
        AllyTroopStats weakestAlly = null;

        foreach (AllyTroopStats ally in allyTroops)
        {
            if (ally.gameObject != gameObject && ally.currentHealth < ally.maxHealth) // Ensure it doesn't target itself and only target allies with less than max health
            {
                if (ally.currentHealth < minHealth)
                {
                    minHealth = ally.currentHealth;
                    weakestAlly = ally;
                }
            }
        }

        targetAlly = weakestAlly;

        // If no ally needs healing, move towards the nearest tile
        if (targetAlly == null)
        {
            FindNearestTile();
            MoveTowardsTile();
        }
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
                        StartCoroutine(ShootProjectile(targetEnemy.transform));
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

    void MoveTowardsAlly()
    {
        if (targetAlly != null)
        {
            float distanceToAlly = Vector3.Distance(transform.position, targetAlly.transform.position);

            if (distanceToAlly > attackRange)
            {
                if (agent != null)
                {
                    agent.SetDestination(targetAlly.transform.position);
                }
            }
            else
            {
                if (!isAttacking)
                {
                    StartCoroutine(ShootHealingProjectile(targetAlly.transform));
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
            else if (gameObject.tag == "AllyRanged" || gameObject.tag == "AllyHealing")
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

    IEnumerator ShootProjectile(Transform target)
    {
        isAttacking = true;

        while (isAttacking && target != null)
        {
            // Check if the target is still valid
            if (target != null && !target.GetComponent<EnemyStats>().IsDead())
            {
                // Create and shoot the projectile
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                if (projectile != null)
                {
                    // Calculate direction to the target's center
                    Vector3 direction = (target.position - transform.position).normalized;

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

    IEnumerator ShootHealingProjectile(Transform target)
    {
        isAttacking = true;

        while (isAttacking && target != null)
        {
            // Check if the target is still valid
            if (target != null)
            {
                // Create and shoot the healing projectile
                GameObject projectile = Instantiate(healingProjectilePrefab, transform.position, Quaternion.identity);
                if (projectile != null)
                {
                    // Calculate direction to the ally's center
                    Vector3 direction = (target.position - transform.position).normalized;

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
                // Find the nearest ally if the current target is invalid
                FindLowestHealthAlly();
            }

            yield return new WaitForSeconds(1f);
        }

        isAttacking = false;
    }
}