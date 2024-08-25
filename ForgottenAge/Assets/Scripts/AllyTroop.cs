using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyTroop : MonoBehaviour
{
    public float attackRange = 2f; // Range for attacking
    public float minDistanceToEnemy = 1.5f; // Minimum distance to enemy to avoid touching
    public float rangedAttackRange = 7f; // Range for ranged attacking
    public GameObject projectilePrefab; // Projectile prefab to be shot by the ranged ally
    public float projectileSpeed = 10f; // Speed of the projectile

    public float healRange = 5f; // Range for healing
    public float healAmount = 10f; // Amount of health to restore
    public GameObject healingProjectilePrefab; // Projectile prefab for healing
    public float healInterval = 2f; // Interval between heals

    public EnemyTroop targetEnemy; // Reference to the nearest enemy
    public GameObject targetEnemyy;
    private GameObject targetMemoryTile; // Reference to the nearest MemoryTile
    private AllyTroop targetAlly; // Reference to the nearest ally to heal
    private bool isAttacking = false; // Flag to indicate if the ally is attacking
    private NavMeshAgent agent; // Reference to the NavMeshAgent
    public Coroutine shootingCoroutine; // Coroutine for shooting
    private Coroutine healingCoroutine; // Coroutine for healing

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
                else if (gameObject.CompareTag("Player") || gameObject.CompareTag("AllyTank"))
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
                        // Check if enemy is within ranged attack range
                        if (distanceToEnemy < rangedAttackRange)
                        {
                            shootingCoroutine = StartCoroutine(ShootEnemy());
                        }
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

            if (gameObject.CompareTag("AllyHealing"))
            {
                if (targetAlly == null || targetAlly.GetComponent<AllyTroopStats>().currentHealth >= targetAlly.GetComponent<AllyTroopStats>().maxHealth)
                {
                    FindNearestAllyToHeal(); // Find the nearest ally to heal with lowest health
                }

                if (targetAlly != null)
                {
                    float distanceToAlly = Vector3.Distance(transform.position, targetAlly.transform.position);

                    // Move towards the ally, but maintain heal range
                    if (distanceToAlly > healRange)
                    {
                        if (agent != null && agent.isOnNavMesh)
                        {
                            agent.SetDestination(targetAlly.transform.position);
                        }
                    }
                    else
                    {
                        // Heal the ally when in range
                        agent.ResetPath();
                        if (healingCoroutine == null)
                        {
                            healingCoroutine = StartCoroutine(ShootHealingProjectile());
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
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        targetEnemy = nearestEnemy;
        targetEnemyy = nearestEnemy.gameObject;
    }

    void FindNearestUncapturedTile()
    {
        CaptureZone[] captureZones = FindObjectsOfType<CaptureZone>();

        float minDistance = Mathf.Infinity;
        GameObject nearestUncapturedTile = null;

        foreach (CaptureZone zone in captureZones)
        {
            if (!zone.captured || zone.capturingSide == "Enemy")
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

    void FindNearestAllyToHeal()
    {
        AllyTroop[] allyTroops = FindObjectsOfType<AllyTroop>();

        float minDistance = Mathf.Infinity;
        float minHealth = Mathf.Infinity;
        AllyTroop nearestAlly = null;

        foreach (AllyTroop ally in allyTroops)
        {
            float allyHealth = ally.GetComponent<AllyTroopStats>().currentHealth;
            if (ally != this && allyHealth < ally.GetComponent<AllyTroopStats>().maxHealth)
            {
                float distance = Vector3.Distance(transform.position, ally.transform.position);
                if (allyHealth < minHealth || (allyHealth == minHealth && distance < minDistance))
                {
                    minHealth = allyHealth;
                    minDistance = distance;
                    nearestAlly = ally;
                }
            }
        }

        targetAlly = nearestAlly;
    }

    void AttackEnemy()
    {
        // Perform attack on the enemy
        EnemyStats enemyStats = targetEnemy.GetComponent<EnemyStats>();
        enemyStats.TakeDamage(cardManager.allyMeleeDamage);

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
            // Check distance again to ensure enemy is still within range
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distanceToEnemy <= rangedAttackRange)
            {
                Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
            }

            yield return new WaitForSeconds(cardManager.allyShootInterval);
        }

        shootingCoroutine = null;
    }

    IEnumerator ShootHealingProjectile()
    {
        while (targetAlly != null && targetAlly.GetComponent<AllyTroopStats>().currentHealth < targetAlly.GetComponent<AllyTroopStats>().maxHealth)
        {
            Vector3 direction = (targetAlly.transform.position - transform.position).normalized;
            GameObject healingProjectile = Instantiate(healingProjectilePrefab, transform.position, Quaternion.identity);
            healingProjectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

            yield return new WaitForSeconds(healInterval);
        }

        healingCoroutine = null;
    }

    private void OnDestroy()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
        }
    }

}

