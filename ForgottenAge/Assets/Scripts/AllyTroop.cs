using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTroop : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of ally movement
    public float attackRange = 2f; // Range for melee attacking
    public float minDistanceToEnemy = 1.5f; // Minimum distance to enemy to avoid touching
    public GameObject projectilePrefab; // Prefab for the projectile
    public float projectileSpeed = 10f; // Speed of the projectile

    private EnemyStats targetEnemy; // Reference to the nearest enemy
    private bool isAttacking = false; // Flag to indicate if the ally is attacking
    private bool isRanged = false; // Flag to check if the unit is ranged

    void Start()
    {
        // Check if the unit is a ranged ally
        if (gameObject.CompareTag("AllyRanged"))
        {
            isRanged = true;
        }
        FindNearestEnemy();
    }

    void Update()
    {
        if (targetEnemy == null)
        {
            FindNearestEnemy();
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
            if (enemy.gameObject.CompareTag("Enemy"))
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

    void MoveTowardsEnemy()
    {
        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (isRanged)
            {
                if (distanceToEnemy > attackRange)
                {
                    Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                    transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position, moveSpeed * Time.deltaTime);
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
                    Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                    transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position, moveSpeed * Time.deltaTime);
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

            yield return new WaitForSeconds(1.0f);
        }

        isAttacking = false;
    }
}
