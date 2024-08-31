using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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

    private Coroutine meleeCoroutine;

    public AllyTroop targetTank; // Reference to the nearest AllyTank
    public float detectionRange = 10f; // Range to detect AllyTank
    public float retreatHealthThreshold = 9f; // Health threshold for retreating
    public bool isRetreating = false; // Flag to check if retreating
    public float retreatDuration = 3f; // Duration to retreat
    private Coroutine retreatCoroutine;
    private AllyTroopStats stats;

    public float retreatCheckInterval = 0.5f;

    private GameObject healingCircle;
    private HealingCircle healingCircleScript;

    public AllyTroop nearestAllyToHeal;

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

        stats = gameObject.GetComponent<AllyTroopStats>();
        healingCircle = GameObject.Find("HealingCircle");

        healingCircleScript = healingCircle.GetComponent<HealingCircle>();
        
       
    }

    void Update()
    {
        if (gameObject.CompareTag("AllyRanged"))
        {
            RangedBehavior();
        }
        else if (gameObject.CompareTag("Player"))
        {
            BasicMeleeBehavior();
        }
        else if (gameObject.CompareTag("AllyTank"))
        {
            BasicMeleeBehavior();
        }else if (gameObject.CompareTag("AllyHealing"))
        {
            if (healingCoroutine == null)
            {
                healingCoroutine = StartCoroutine(HealNearestAlly());
            }
        }
    }

    private IEnumerator HealNearestAlly()
    {
        while (true)
        {
            nearestAllyToHeal = FindNearestAllyWithMissingHealth();

            if (nearestAllyToHeal != null)
            {
                agent.SetDestination(nearestAllyToHeal.transform.position);

                // Wait until the ally is within heal range
                while (nearestAllyToHeal != null && Vector3.Distance(transform.position, nearestAllyToHeal.transform.position) > healRange)
                {
                    yield return null;
                }

                // Heal the ally until fully healed or the ally is out of range
                while (nearestAllyToHeal != null && nearestAllyToHeal.stats.currentHealth < nearestAllyToHeal.stats.maxHealth)
                {
                    nearestAllyToHeal.stats.currentHealth += healAmount;
                    nearestAllyToHeal.stats.currentHealth = Mathf.Min(nearestAllyToHeal.stats.currentHealth, nearestAllyToHeal.stats.maxHealth);

                    yield return new WaitForSeconds(healInterval);
                }
            }

            // Wait a bit before finding another ally to heal
            yield return new WaitForSeconds(0.5f);
        }
    }

    AllyTroop FindNearestAllyWithMissingHealth()
    {
        AllyTroop[] allyTroops = FindObjectsOfType<AllyTroop>();
        Debug.Log(allyTroops[0]);
        float minDistance = Mathf.Infinity;
        AllyTroop nearestAlly = null;

        foreach (AllyTroop ally in allyTroops)
        {
            if (ally.CompareTag("AllyHealing") || ally.CompareTag("Player") || ally.CompareTag("AllyTank") || ally.CompareTag("AllyRanged"))
            {
                if (ally.stats.currentHealth < ally.stats.maxHealth) // Check if the ally needs healing
                {
                    float distance = Vector3.Distance(transform.position, ally.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestAlly = ally;
                    }
                }
            }
        }

        return nearestAlly;
    }

    void RangedBehavior()
    {
        if (isRetreating) return;

        StartCoroutine(CheckForRetreat());

        // First, try to find the nearest tank and protect it
        FindNearestAllyTank();
        if (targetTank != null)
        {
            ProtectAllyTank();
        }
        else
        {
            // If there's no tank nearby, find the nearest enemy and attack
            FindNearestEnemy();
            if (targetEnemy != null)
            {
                FindAndAttack();
            }
            else
            {
                // If there's no tank or enemy nearby, find and move to the nearest axon
                FindAndMoveToNearestAxon();
            }
        }
    }

    private IEnumerator RetreatFromEnemy(EnemyTroop enemy)
    {
        // Retreat for a fixed duration
        float retreatEndTime = Time.time + retreatDuration;

        while (Time.time < retreatEndTime && enemy != null)
        {
            // Move in the opposite direction of the enemy
            Vector3 retreatDirection = (transform.position - enemy.transform.position).normalized * detectionRange;
            agent.SetDestination(transform.position + retreatDirection);

            // Shoot at the enemy while retreating
            if (shootingCoroutine == null)
            {
                targetEnemy = enemy;
                shootingCoroutine = StartCoroutine(ShootEnemy());
            }

            yield return new WaitForSeconds(0.1f);
        }

        // Stop retreating and reset
        isRetreating = false;
        retreatCoroutine = null;

        // Optionally, reset the target enemy after retreating
        targetEnemy = null;
    }

    private IEnumerator CheckForRetreat()
    {
        while (true)
        {
            EnemyTroop[] enemyTroops = FindObjectsOfType<EnemyTroop>();
            foreach (EnemyTroop enemy in enemyTroops)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= detectionRange && enemy.targetAlly == this)
                {
                    // Start retreating if targeted by an enemy
                    if (!isRetreating)
                    {
                        isRetreating = true;
                        StartCoroutine(RetreatFromEnemy(enemy));
                    }
                }
            }

            yield return new WaitForSeconds(retreatCheckInterval);
        }
    }

    private IEnumerator RetreatCoroutine()
    {
        // Find enemies that are targeting this unit
        EnemyTroop[] enemyTroops = FindObjectsOfType<EnemyTroop>();
        EnemyTroop newTargetEnemy = null;

        foreach (EnemyTroop enemy in enemyTroops)
        {
            if (enemy.targetAlly == this)
            {
                newTargetEnemy = enemy;
                break; // Prioritize the first enemy found that is targeting this unit
            }
        }

        if (newTargetEnemy != null)
        {
            targetEnemy = newTargetEnemy;
        }

        // Retreat while targeting the enemy that’s attacking this unit
        float retreatEndTime = Time.time + retreatDuration;
        while (Time.time < retreatEndTime && stats.currentHealth <= retreatHealthThreshold)
        {
            Vector3 retreatDirection = (transform.position - targetEnemy.transform.position).normalized * detectionRange;
            agent.SetDestination(transform.position + retreatDirection);

            // Shoot at the enemy while retreating
            if (shootingCoroutine == null)
            {
                shootingCoroutine = StartCoroutine(ShootEnemy());
            }

            // If there is no target enemy, stop retreating
            if (targetEnemy == null)
            {
                isRetreating = false;
            }

            yield return new WaitForSeconds(1.0f); // Adjust this interval for retreat rate
        }

        // Stop retreating and reset
        isRetreating = false;
        retreatCoroutine = null;
    }

    private void StartRetreating()
    {
        if (!isRetreating)
        {
            isRetreating = true;
            if (retreatCoroutine == null)
            {
                retreatCoroutine = StartCoroutine(RetreatCoroutine());
            }
        }
    }

    void FindNearestAllyTank()
    {
        AllyTroop[] allyTanks = FindObjectsOfType<AllyTroop>();
        float minDistance = detectionRange;
        AllyTroop nearestTank = null;

        foreach (AllyTroop ally in allyTanks)
        {
            if (ally.CompareTag("AllyTank"))
            {
                float distance = Vector3.Distance(transform.position, ally.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTank = ally;
                }
            }
        }

        targetTank = nearestTank;
    }

    void ProtectAllyTank()
    {
        if (targetTank != null)
        {
            // Move towards the AllyTank to protect it
            agent.SetDestination(targetTank.transform.position);

            if (IsTankUnderAttack())
            {
                // If the AllyTank is under attack, target the attacker
                if (shootingCoroutine == null)
                {
                    shootingCoroutine = StartCoroutine(ShootEnemy());
                }
            }
        }
    }

    bool IsTankUnderAttack()
    {
        // Check if the targetTank is assigned
        if (targetTank == null)
        {
            return false;
        }

        // Get all enemies in the scene
        EnemyTroop[] enemyTroops = FindObjectsOfType<EnemyTroop>();

        // Check if any enemy is targeting the tank
        foreach (EnemyTroop enemy in enemyTroops)
        {
            if (enemy.targetAlly == targetTank)
            {
                targetEnemy = enemy;
                return true; // The tank is under attack
            }
        }

        return false; // No enemies are targeting the tank
    }

    IEnumerator ShootEnemy()
    {
        while ( targetEnemy != null)
        {
            // Stick to the AllyTank while attacking the enemy
            if (targetTank != null)
            {
                agent.SetDestination(targetTank.transform.position);
            }
            

            
            if (targetEnemy!= null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
                if (distanceToEnemy <= rangedAttackRange)
                {
                    Debug.Log("Shoot");
                    Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
                }
            }

            yield return new WaitForSeconds(1.0f); // Adjust this interval for the attack rate
        }

        shootingCoroutine = null; // Reset the coroutine reference when done
    }

    

    void FindNearestEnemy()
    {
        EnemyTroop[] enemyTroops = FindObjectsOfType<EnemyTroop>();
        float searchRange = 10f; // Define the range within which to search for enemies
        float minDistance = searchRange;
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
        if (nearestEnemy != null)
        {
            targetEnemyy = nearestEnemy.gameObject;
        }
        else
        {
            targetEnemyy = null; // No enemy found within range
        }
    }

    void FindAndMoveToNearestAxon()
    {
        GameObject[] axons = GameObject.FindGameObjectsWithTag("Axon");

        if (axons.Length == 0)
        {
            return;
        }

        GameObject nearestAxon = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject axon in axons)
        {
            float distance = Vector3.Distance(transform.position, axon.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestAxon = axon;
            }
        }

        if (nearestAxon != null)
        {
            agent.SetDestination(nearestAxon.transform.position);
        }
        else
        {
            Debug.Log("doodoo water");
        }
    }

    

    void FindNearestAllyToHeal()
    {
        AllyTroop[] allyTroops = FindObjectsOfType<AllyTroop>();
        float minDistance = healRange;
        AllyTroop nearestAlly = null;

        foreach (AllyTroop ally in allyTroops)
        {
            if (ally.CompareTag("AllyHealing") || ally.CompareTag("Player") || ally.CompareTag("Ally_Tank") || ally.CompareTag("AllyRanged"))
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
        agent.SetDestination(targetAlly.transform.position);
    }

    void BasicMeleeBehavior()
    {
        FindNearestEnemy();
        if (targetEnemy != null)
        {
            agent.SetDestination(targetEnemy.transform.position);
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distanceToEnemy <= attackRange)
            {
                if (meleeCoroutine == null)
                {
                    meleeCoroutine = StartCoroutine(MeleeAttack());
                }
            }
            else
            {
                // If too far, continue moving towards the enemy
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
        else
        {
            FindAndMoveToNearestAxon();
        }
    }

    IEnumerator MeleeAttack()
    {
        while (targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange)
        {
            // Perform melee attack on the targetEnemy
            EnemyStats enemyStats = targetEnemy.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(1); // Adjust damage as needed

            yield return new WaitForSeconds(1.0f); // Adjust this interval for the attack rate
        }

        meleeCoroutine = null; // Reset the coroutine reference when done
    }

    void FindAndAttack()
    {
        // Find the nearest enemy
        FindNearestEnemy();

        if (targetEnemy != null)
        {
            // Move towards the enemy
            agent.SetDestination(targetEnemy.transform.position);

            // Check if within attack range and shoot
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distanceToEnemy <= rangedAttackRange)
            {
                if (shootingCoroutine == null)
                {
                    shootingCoroutine = StartCoroutine(ShootEnemy());
                }
            }
        }
    }

    
}
