using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyTroop : MonoBehaviour
{
    public float attackRange = 2f;
    public float minDistanceToEnemy = 1.5f;
    public float rangedAttackRange = 7f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    public float healRange = 50f;
    public float healAmount = 10f;
    public GameObject healingProjectilePrefab;
    public float healInterval = 2f;

    public EnemyTroop targetEnemy;
    public GameObject targetEnemyy;
    private GameObject targetMemoryTile;
    private AllyTroop targetAlly;
    private bool isAttacking = false;
    private NavMeshAgent agent;
    public Coroutine shootingCoroutine;
    private Coroutine healingCoroutine;

    private CardManager cardManager;

    private Coroutine meleeCoroutine;

    public AllyTroop targetTank;
    public float detectionRange = 10f;
    public float retreatHealthThreshold = 9f;
    public bool isRetreating = false;
    public float retreatDuration = 3f;
    private Coroutine retreatCoroutine;
    private AllyTroopStats stats;

    public float retreatCheckInterval = 0.5f;

    private GameObject healingCircle;
    private HealingCircle healingCircleScript;

    public AllyTroop nearestAllyToHeal;

    private Coroutine updateCacheRoutine;

    public AudioManagerr audioManager;

    public bool hasReachedTile = false;
    


    // Optimization variables
    private float updateInterval = 0.2f;
    private float lastUpdateTime;
    private float cacheUpdateInterval = 1f;
    private float lastCacheUpdateTime;

    private List<EnemyTroop> cachedEnemies = new List<EnemyTroop>();
    private List<AllyTroop> cachedAllies = new List<AllyTroop>();
    private List<GameObject> cachedAxons = new List<GameObject>();

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerr>();
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.updateUpAxis = false;
            agent.updateRotation = false;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                
            }
        }
        else
        {
            
        }

        stats = gameObject.GetComponent<AllyTroopStats>();
        healingCircle = GameObject.Find("HealingCircle");
        //healingCircleScript = healingCircle.GetComponent<HealingCircle>();

        lastUpdateTime = Time.time;
        lastCacheUpdateTime = Time.time;
        StartCoroutine(UpdateCacheRoutine());

    }

    void Update()
    {
        if (Time.time - lastUpdateTime < updateInterval) return;

        lastUpdateTime = Time.time;

        // If the troop has reached the memory tile, stop the agent and check for enemies
        if (agent != null)
        {
           
            isAttacking = false; // Ensure troop isn't locked into moving mode

            // Now check for nearby enemies and switch to attack mode
            FindNearestEnemy();
            if (targetEnemyy != null)
            {
                
                if (gameObject.CompareTag("AllyRanged"))
                {
                    RangedBehavior();
                }
                else if (gameObject.CompareTag("Player") || gameObject.CompareTag("AllyTank"))
                {
                    BasicMeleeBehavior();
                    
                }
            } else
            {
                StartCoroutine(Wait());
                FindNearestEnemy();
                if (targetEnemyy == null)
                {
                    FindAndMoveToNearestAxon();
                }
                
                
            }
        }

        

        // Continue checking for enemies even if troop is not moving
        if (cachedEnemies.Count > 0)
        {
            FindNearestEnemy();
        }

        if (gameObject.CompareTag("AllyHealing"))
        {
            if (healingCoroutine == null)
            {
                healingCoroutine = StartCoroutine(HealNearestAlly());
            }
        }
    }

    void OnReachedTile()
    {
        // Stop moving once the tile is reached
        agent.ResetPath();

        // Immediately start checking for enemies
        FindNearestEnemy();
        if (targetEnemy != null)
        {
            if (gameObject.CompareTag("AllyRanged"))
            {
                RangedBehavior();
            }
            else if (gameObject.CompareTag("Player") || gameObject.CompareTag("AllyTank"))
            {
                BasicMeleeBehavior();
            }
        }
    }


    IEnumerator UpdateCacheRoutine()
    {
        while (true)
        {
            UpdateCache();
            yield return new WaitForSeconds(cacheUpdateInterval);
        }
    }

    void UpdateCache()
    {
        cachedEnemies.Clear();
        cachedEnemies.AddRange(FindObjectsOfType<EnemyTroop>());

        cachedAllies.Clear();
        cachedAllies.AddRange(FindObjectsOfType<AllyTroop>());
    }

    private IEnumerator HealNearestAlly()
    {
        while (true)
        {
            nearestAllyToHeal = FindNearestAllyWithMissingHealth();

            if (nearestAllyToHeal != null)
            {
                agent.SetDestination(nearestAllyToHeal.transform.position);

                while (nearestAllyToHeal != null && Vector3.Distance(transform.position, nearestAllyToHeal.transform.position) > healRange)
                {
                    yield return null;
                }

                while (nearestAllyToHeal != null && nearestAllyToHeal.stats.currentHealth < nearestAllyToHeal.stats.maxHealth)
                {
                    nearestAllyToHeal.stats.currentHealth += healAmount;
                    nearestAllyToHeal.stats.currentHealth = Mathf.Min(nearestAllyToHeal.stats.currentHealth, nearestAllyToHeal.stats.maxHealth);

                    yield return new WaitForSeconds(healInterval);
                }
            } 

            yield return new WaitForSeconds(0.5f);
        }
    }

    AllyTroop FindNearestAllyWithMissingHealth()
    {
        float minDistance = Mathf.Infinity;
        AllyTroop nearestAlly = null;

        foreach (AllyTroop ally in cachedAllies)
        {
            if (ally.CompareTag("AllyHealing") || ally.CompareTag("Player") || ally.CompareTag("AllyTank") || ally.CompareTag("AllyRanged"))
            {
                if (ally.stats.currentHealth < ally.stats.maxHealth)
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

        // Check for retreat only for ranged troops
        CheckForRetreat();

        FindNearestAllyTank();
        if (targetTank != null)
        {
            ProtectAllyTank();
        }
        else
        {
            FindNearestEnemy();
            if (targetEnemy != null)
            {
                FindAndAttack();
            }
            else
            {
                FindAndMoveToNearestAxon();
            }
        }
    }

    IEnumerator RetreatFromEnemy(EnemyTroop enemy)
    {
        float retreatEndTime = Time.time + retreatDuration;

        while (Time.time < retreatEndTime && enemy != null)
        {
            Vector3 retreatDirection = (transform.position - enemy.transform.position).normalized * detectionRange;
            agent.SetDestination(transform.position + retreatDirection);

            if (shootingCoroutine == null)
            {
                targetEnemy = enemy;
                shootingCoroutine = StartCoroutine(ShootEnemy());
            }

            yield return new WaitForSeconds(0.1f);
        }

        isRetreating = false;
        retreatCoroutine = null;
        targetEnemy = null;
    }

    void CheckForRetreat()
    {
        foreach (EnemyTroop enemy in cachedEnemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= detectionRange && enemy.targetAlly == this)
                {
                    if (!isRetreating)
                    {
                        isRetreating = true;
                        StartCoroutine(RetreatFromEnemy(enemy));
                        return; // Exit the method once we start retreating
                    }
                }
            }
            else
            {
                continue;
            }
            
        }
    }

    private IEnumerator RetreatCoroutine()
    {
        EnemyTroop newTargetEnemy = null;

        if (cachedEnemies.Count > 0)
        {
            foreach (EnemyTroop enemy in cachedEnemies)
            {
                if (enemy.targetAlly == this)
                {
                    newTargetEnemy = enemy;
                    break;
                }
            }
        } 

        

        if (newTargetEnemy != null)
        {
            targetEnemy = newTargetEnemy;
        }

        float retreatEndTime = Time.time + retreatDuration;
        while (Time.time < retreatEndTime && stats.currentHealth <= retreatHealthThreshold)
        {
            Vector3 retreatDirection = (transform.position - targetEnemy.transform.position).normalized * detectionRange;
            agent.SetDestination(transform.position + retreatDirection);

            if (shootingCoroutine == null)
            {
                shootingCoroutine = StartCoroutine(ShootEnemy());
            }

            if (targetEnemy == null)
            {
                isRetreating = false;
            }

            yield return new WaitForSeconds(1.0f);
        }

        isRetreating = false;
        retreatCoroutine = null;
    }

    void FindNearestAllyTank()
    {
        float minDistance = detectionRange;
        AllyTroop nearestTank = null;

        foreach (AllyTroop ally in cachedAllies)
        {

            if (ally != null)
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
            else
            {
                continue;
            }
            
        }

        targetTank = nearestTank;
    }

    void ProtectAllyTank()
    {
        if (targetTank != null)
        {
            agent.SetDestination(targetTank.transform.position);

            if (IsTankUnderAttack())
            {
                if (shootingCoroutine == null)
                {
                    shootingCoroutine = StartCoroutine(ShootEnemy());
                }
            }
        }
    }

    bool IsTankUnderAttack()
    {
        if (targetTank == null)
        {
            return false;
        }

        foreach (EnemyTroop enemy in cachedEnemies)
        {
            if (enemy.targetAlly == targetTank)
            {
                targetEnemy = enemy;
                return true;
            }
        }

        return false;
    }

    IEnumerator ShootEnemy()
    {
        while (targetEnemy != null)
        {
            if (targetTank != null)
            {
                agent.SetDestination(targetTank.transform.position);
            }

            if (targetEnemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
                if (distanceToEnemy <= rangedAttackRange)
                {
                    Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    audioManager.SFX.PlayOneShot(audioManager.shoot1);
                    projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
                }
            }

            yield return new WaitForSeconds(1.0f);
        }

        shootingCoroutine = null;
    }

    

    

    void BasicMeleeBehavior()
    {
        FindNearestEnemy();
        if (targetEnemyy != null)
        {
            
            agent.SetDestination(targetEnemyy.transform.position);
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
                agent.SetDestination(targetEnemyy.transform.position);
                hasReachedTile = false;
            }
        }
        else
        {
            return;
        }
       
    }

    void FindNearestEnemy()
    {
        float searchRange = 40f;
        float minDistance = searchRange;
        EnemyTroop nearestEnemy = null;

        foreach (EnemyTroop enemy in cachedEnemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }
            else
            {
                continue;
            }
            
            
        }

        targetEnemy = nearestEnemy;
        targetEnemyy = nearestEnemy?.gameObject;
    }

    void FindAndMoveToNearestAxon()
    {
        // Find all memory tiles
        GameObject[] memoryTiles = GameObject.FindGameObjectsWithTag("MemoryTile");
        if (memoryTiles.Length == 0) return; // No memory tiles to move to

        // Find all troops with relevant tags
        List<GameObject> friendlyTroops = new List<GameObject>();
        friendlyTroops.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        friendlyTroops.AddRange(GameObject.FindGameObjectsWithTag("AllyRanged"));
        friendlyTroops.AddRange(GameObject.FindGameObjectsWithTag("AllyTank"));

        int totalTroops = friendlyTroops.Count;
        if (totalTroops == 0) return; // No troops found

        // Calculate how many troops to send to each memory tile (totalTroops / 4)
        int troopsPerTile = Mathf.CeilToInt((float)totalTroops / 4);

        // Loop through troops and assign them to memory tiles
        int tileIndex = 0;
        foreach (GameObject troop in friendlyTroops)
        {
            NavMeshAgent agent = troop.GetComponent<NavMeshAgent>();
            if (agent == null) continue; // Skip if troop has no agent

            // If the troop has an enemy target, attack
            if (targetEnemyy != null)
            {
                agent.SetDestination(targetEnemyy.transform.position);
                
                continue; // Skip moving to tile if there's an enemy
            }

            // Move troop to a memory tile
            GameObject assignedTile = memoryTiles[tileIndex];
            agent.SetDestination(assignedTile.transform.position);
            

            // Increment the tile index, and wrap around if we reach the last tile
            tileIndex = (tileIndex + 1) % memoryTiles.Length;
        }
    }

    IEnumerator MeleeAttack()
    {
        while (targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange)
        {
            EnemyStats enemyStats = targetEnemy.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(1);

            yield return new WaitForSeconds(1.0f);
        }

        if (targetEnemy == null)
        {
           //nothing happens
        }

        meleeCoroutine = null;
    }

    IEnumerator Wait()
    {
        

        yield return new WaitForSeconds(2.0f);
        

        
    }

    void FindAndAttack()
    {
        FindNearestEnemy();

        if (targetEnemy != null)
        {
            agent.SetDestination(targetEnemy.transform.position);

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

