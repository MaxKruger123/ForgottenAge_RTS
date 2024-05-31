using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTroop : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of ally movement
    public float attackRange = 2f; // Range for attacking
    public float minDistanceToEnemy = 1.5f; // Minimum distance to enemy to avoid touching

    private EnemyStats targetEnemy; // Reference to the nearest enemy
    private bool isAttacking = false; // Flag to indicate if the ally is attacking

    void Start()
    {
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
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distanceToEnemy > minDistanceToEnemy)
            {
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
}
