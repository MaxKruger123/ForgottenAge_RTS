using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;



public class EnemyTroop : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackRangeTwo = 10f;
    public float minDistanceToAlly = 1.5f;
    public float rangedAttackRange = 30f;
    public float protectRange = 100f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float shootInterval = 1f;
    public float retreatHealthThreshold = 20f;
    public float retreatTime = 7f;

    public AllyTroop targetAlly;
    public RepairTroop targettroop;
    private GameObject targetMemoryTile;
    public GameObject targetBuilding;
    private bool isAttacking = false;
    private NavMeshAgent agent;
    private Coroutine shootingCoroutine;
    private Coroutine meleeCoroutine;

    private EnemyStats enemyStats;
    private CardManager cardManager;
    private GameObject targetAxon;
    public GameObject targetTank;
    private Coroutine retreatCoroutine;
    GameObject nearestTank;
    public bool tankUnderAttack = false;

    private AllyTroop attacker;

    private Coroutine damageCoroutine;

    private int tankRange = 15;

    public AllyTroop threateningTroop;

    // Optimization variables
    private float updateInterval = 0.2f;
    private float lastUpdateTime;
    private float cacheUpdateInterval = 1f;
    private float lastCacheUpdateTime;

    public bool stopFlee = false;

    public AudioManagerr audioManager;

    public float rotationSpeed = 5f;

    private List<AllyTroop> cachedAllyTroops = new List<AllyTroop>();
    private List<GameObject> cachedAxons = new List<GameObject>();
    private List<GameObject> cachedTanks = new List<GameObject>();
    private List<GameObject> cachedRepairTroops = new List<GameObject>();
    private List<GameObject> cachedNormalEnemies = new List<GameObject>();

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerr>();
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
        agent = GetComponent<NavMeshAgent>();
        shootingCoroutine = null;

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

        enemyStats = GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            
        }

        meleeCoroutine = null;

        lastUpdateTime = Time.time;
        lastCacheUpdateTime = Time.time;
        StartCoroutine(UpdateCacheRoutine());
    }

    void Update()
    {
        if (Time.time - lastUpdateTime < updateInterval) return;

        lastUpdateTime = Time.time;

        if (gameObject.CompareTag("Enemy"))
        {
            FindNearestAlly();
            if (targetAlly != null)
            {

                MoveTowardsAlly();
                
                if (IsInMeleeRange())
                {
                    if (meleeCoroutine == null)
                    {
                        meleeCoroutine = StartCoroutine(MeleeAttack());
                    }
                }
            }
            else
            {
                FindNearestRepairTroop();

                if (targettroop != null)
                {
                    if (IsInMeleeRangee())
                    {
                        if (meleeCoroutine == null)
                        {
                            meleeCoroutine = StartCoroutine(MeleeAttackk());
                        }
                    }
                }
                else
                {
                    FindAndMoveToNearestAxon();
                }
            }
        }

        if (gameObject.CompareTag("Enemy_Tank"))
        {
            FindNearestRepairTroop();
            FindAllyTroopTargetingTank();

            if (threateningTroop != null)
            {
                targetAlly = threateningTroop;
                MoveTowardsAlly();
                if (IsInMeleeRange())
                {
                    if (meleeCoroutine == null)
                    {
                        meleeCoroutine = StartCoroutine(MeleeAttack());
                    }
                }
            }
            else if (targettroop != null)
            {
                if (IsInMeleeRangee())
                {
                    if (meleeCoroutine == null)
                    {
                        meleeCoroutine = StartCoroutine(MeleeAttackk());
                    }
                }
            }
            else
            {
                FindAndMoveToNearestAxon();
            }
        }

        if (gameObject.CompareTag("EnemyRanged"))
        {
            FindNearestTank();

            if (IsTargeted())
            {
                FleeAndShoot();
            }
            else if (targetTank != null)
            {
                FollowTank();

                if (IsTankUnderAttack())
                {
                    if (shootingCoroutine == null) // Ensure only one shooting coroutine is running
                    {
                        FindAndAttackTargetAttackingTank();
                    }
                }
            }
            else
            {
                if (!IsTargeted())
                {
                    FindNearestAlly();

                    if (targetAlly != null && shootingCoroutine == null) // Ensure only one shooting coroutine is running
                    {
                        agent.SetDestination(targetAlly.transform.position);
                        shootingCoroutine = StartCoroutine(ShootAlly());
                    }
                }
                else
                {
                    FindAndMoveToNearestAxon();
                }
            }

            if (!IsTargeted() && targetTank == null && targetAlly == null && targetBuilding == null)
            {
                FindAndMoveToNearestAxon();
            }

            if (stopFlee == true)
            {
                FindNearestNormalEnemy();
            }
        }

        if (gameObject.CompareTag("Kamikaze"))
        {
            FindAndMoveToNearestAxon();
        }
    }

    

    bool IsTargeted()
    {
        // Check if any ally is targeting this ranged enemy
        AllyTroop[] allyTroops = FindObjectsOfType<AllyTroop>();

        foreach (AllyTroop ally in allyTroops)
        {
            if (ally != null)
            {
                if (ally.targetEnemyy == gameObject)
                {
                    attacker = ally;
                    targetAlly = ally;
                    return true;

                }
            }
            else
            {
                continue;
            }
            
        } 
        return false;
       
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
        cachedAllyTroops.Clear();
        cachedAllyTroops.AddRange(FindObjectsOfType<AllyTroop>());

        cachedAxons.Clear();
        cachedAxons.AddRange(GameObject.FindGameObjectsWithTag("Axon"));

        cachedTanks.Clear();
        cachedTanks.AddRange(GameObject.FindGameObjectsWithTag("Enemy_Tank"));

        cachedRepairTroops.Clear();
        cachedRepairTroops.AddRange(GameObject.FindGameObjectsWithTag("RepairTroop"));

        cachedNormalEnemies.Clear();
        cachedNormalEnemies.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        cachedNormalEnemies.Clear();
        cachedNormalEnemies.AddRange(GameObject.FindGameObjectsWithTag("AllyRanged"));
    }

    void FindAllyTroopTargetingTank()
    {
        foreach (AllyTroop troop in cachedAllyTroops)
        {
            if (troop != null)
            {
                if (troop.targetEnemyy == this.gameObject)
                {
                    threateningTroop = troop;
                    return;
                }
            }
            else
            {
                continue;
            }
            
        }
        threateningTroop = null;
    }

    void FindNearestRepairTroop()
    {
        float maxSearchDistance = 5f;
        float minDistance = Mathf.Infinity;
        GameObject nearestRepairTroop = null;

        foreach (GameObject repairTroop in cachedRepairTroops)
        {
            if (repairTroop != null)
            {
                float distance = Vector3.Distance(transform.position, repairTroop.transform.position);
                if (distance < minDistance && distance <= maxSearchDistance)
                {
                    minDistance = distance;
                    nearestRepairTroop = repairTroop;
                }
            }
            else
            {
                continue;
            }
            
        }

        if (nearestRepairTroop != null)
        {
            targettroop = nearestRepairTroop.GetComponent<RepairTroop>();
            agent.SetDestination(nearestRepairTroop.transform.position);
        }
        else
        {
            targettroop = null;
        }
    }

    void MoveTowardsAlly()
    {
        if (targetAlly != null)
        {
            agent.SetDestination(targetAlly.transform.position);
        }
    }

    bool IsInMeleeRange()
    {
        if (targetAlly != null)
        {
            float distance = Vector3.Distance(transform.position, targetAlly.transform.position);
            return distance <= attackRange;
        }
        else if (targettroop != null)
        {
            float distance = Vector3.Distance(transform.position, targettroop.transform.position);
            return distance <= attackRange;
        }
        return false;
    }

    bool IsInMeleeRangee()
    {
        if (targettroop != null)
        {
            float distance = Vector3.Distance(transform.position, targettroop.transform.position);
            return distance <= attackRange;
        }
        else if (targetAlly != null)
        {
            float distance = Vector3.Distance(transform.position, targetAlly.transform.position);
            return distance <= attackRangeTwo;
        }
        return false;
    }

    IEnumerator MeleeAttack()
    {
        if (targetAlly != null)
        {
            AllyTroopStats targetAllyStats = targetAlly.GetComponent<AllyTroopStats>();
            targetAllyStats.TakeDamage(1.0f);
        }
        yield return new WaitForSeconds(1.0f);
        meleeCoroutine = null;
    }

    IEnumerator MeleeAttackk()
    {
        if (targettroop != null)
        {
            AllyTroopStats targetAllyStats = targettroop.GetComponent<AllyTroopStats>();
            targetAllyStats.TakeDamage(2.0f);
        }
        yield return new WaitForSeconds(2.0f);
        meleeCoroutine = null;
    }

    void FollowTank()
    {
        if (targetTank != null)
        {
            agent.SetDestination(targetTank.transform.position);
        }
    }

    bool IsTankUnderAttack()
    {
        foreach (AllyTroop ally in cachedAllyTroops)
        {
            if (ally != null)
            {
                if (ally.targetEnemyy == targetTank)
                {
                    return true;
                }
            }
            else
            {
                continue;
            }
            
        }
        return false;
    }

    void FindNearestAlly()
    {
        float minDistance = Mathf.Infinity;
        AllyTroop nearestAlly = null;

        foreach (AllyTroop ally in cachedAllyTroops)
        {
            if (ally != null)
            {
                float distance = Vector3.Distance(transform.position, ally.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAlly = ally;
                }
            }
            else
            {
                continue;
            }
            
        }

        targetAlly = nearestAlly;
    }

    void FindAndMoveToNearestAlly()
    {
        float minDistance = Mathf.Infinity;
        AllyTroop nearestAlly = null;

        foreach (AllyTroop ally in cachedAllyTroops)
        {
            if (ally != null)
            {
                float distance = Vector3.Distance(transform.position, ally.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAlly = ally;
                    agent.SetDestination(nearestAlly.transform.position);
                }
            }
            else
            {
                continue;
            }
           
        }

        targetAlly = nearestAlly;
    }

    void FindNearestTank()
    {
        float minDistance = protectRange;
        GameObject nearestTank = null;

        foreach (GameObject tank in cachedTanks)
        {
            if (tank != null)
            {
                float distance = Vector3.Distance(transform.position, tank.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTank = tank;
                }
            }
            else
            {
                continue;
            }
            
        }

        targetTank = nearestTank;
        if (targetTank != null)
        {
            agent.SetDestination(targetTank.transform.position);
        }
    }

    void FindAndAttackTargetAttackingTank()
    {
        foreach (AllyTroop ally in cachedAllyTroops)
        {
            if (ally != null)
            {
                if (ally.targetEnemyy == targetTank)
                {
                    targetAlly = ally;
                    Debug.Log("shoott");
                    if (shootingCoroutine == null)
                    {
                        shootingCoroutine = StartCoroutine(ShootAlly());
                    }
                    
                    return;
                }
            }
            else
            {
                continue;
            }
            
        }
    }

    void FindNearestNormalEnemy()
    {
        float minDistance = protectRange;
        GameObject nearestNormalEnemy = null;

        foreach (GameObject enemy in cachedNormalEnemies)
        {

            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNormalEnemy = enemy;
                }
            }
            else
            {
                continue;
            }
            
        }

        targetBuilding = nearestNormalEnemy;
        agent.SetDestination(targetBuilding.transform.position);
        if (targetBuilding != null && shootingCoroutine == null)
        {
            
            shootingCoroutine = StartCoroutine(ShootAlly());
        }
    }

    void FindAndAttackTargetAttackingNormalEnemy()
    {
        foreach (AllyTroop ally in cachedAllyTroops)
        {
            if (ally != null)
            {
                if (ally.targetEnemyy == targetBuilding)
                {

                    targetAlly = ally;
                    if (shootingCoroutine == null)
                    {
                        shootingCoroutine = StartCoroutine(ShootAlly());
                    }
                    
                    return;
                }
            }
            else
            {
                continue;
            }
            
            
        }
    }

    IEnumerator ShootAlly()
    {
        while (targetAlly != null)
        {
            if (targetAlly != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, targetAlly.transform.position);

                if (distanceToEnemy <= rangedAttackRange)
                {
                    // Only shoot if the target is within range
                    Vector3 direction = (targetAlly.transform.position - transform.position).normalized;
                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

                    // Play shooting sound
                    audioManager.SFX.PlayOneShot(audioManager.shoot1);

                    // Set projectile velocity
                    projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
                }
            }

            // Wait for the shoot interval before firing again
            yield return new WaitForSeconds(shootInterval);
        }

        // Reset the shooting coroutine when finished
        shootingCoroutine = null;
    }

    IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(2.0f);
        isAttacking = false;
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
            if (axon != null)
            {
                Vector2 axonCentre = axon.GetComponent<LineRenderer>().GetPosition(axon.GetComponent<LineRenderer>().positionCount / 2);
                float distance = Vector3.Distance(transform.position, axonCentre);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAxon = axon;

                    if (distance < 2.5f && gameObject.CompareTag("Kamikaze"))
                    {
                        Axon axonScript = axon.GetComponent<Axon>();
                        axonScript.TakeDamage(20f);
                        Destroy(gameObject);
                        // Explode
                    }
                }
            }
            else
            {
                continue;
            }
            
        }

        if (nearestAxon != null)
        {
            Vector2 axonCentre = nearestAxon.GetComponent<LineRenderer>().GetPosition(nearestAxon.GetComponent<LineRenderer>().positionCount / 2);
            agent.SetDestination(axonCentre);

            // Check if the tank is within melee range of the axon
            if (gameObject.CompareTag("Enemy_Tank") && Vector3.Distance(transform.position, axonCentre) <= 2f || gameObject.CompareTag("Enemy") && Vector3.Distance(transform.position, axonCentre) <= 2f)
            {
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(DealDamageToAxon(nearestAxon));
                }
            }
            else
            {
                // Stop the coroutine if the tank is no longer within melee range
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }
            }
        }
    }

    private IEnumerator DealDamageToAxon(GameObject axon)
    {
        Axon axonScript = axon.GetComponent<Axon>();
        while (axonScript != null)
        {
            axonScript.TakeDamage(5f); // Deal 5 damage every 2 seconds
            yield return new WaitForSeconds(2.0f);
        }
    }

    void FleeAndShoot()
    {
        

        // Distance between the ranged unit and its attacker (targetEnemy)
        float distanceToEnemy = Vector3.Distance(transform.position, targetAlly.transform.position);

        // Define a safe distance the ranged unit will flee to
        float safeDistance = 10.0f;  // Adjust as needed

        // If within the safe distance, flee from the enemy
        if (distanceToEnemy < safeDistance && !stopFlee)
        {
            // Calculate the flee direction (away from the enemy)
            Vector3 fleeDirection = (transform.position - targetAlly.transform.position).normalized;
            Vector3 fleePosition = transform.position + fleeDirection * safeDistance;

            // Set the destination to the calculated flee position
            agent.SetDestination(fleePosition);
            agent.speed = 2f;  // Increase the agent's speed to flee faster
            if (shootingCoroutine == null)
            {
                shootingCoroutine = StartCoroutine(ShootAlly());
            }
            
            StartCoroutine(Wait());
            stopFlee = true;
            
        }

        
       
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
    }


}