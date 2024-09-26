using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Extensions;

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

    

    private List<AllyTroop> cachedAllyTroops = new List<AllyTroop>();
    private List<GameObject> cachedAxons = new List<GameObject>();
    private List<GameObject> cachedTanks = new List<GameObject>();
    private List<GameObject> cachedRepairTroops = new List<GameObject>();
    private List<GameObject> cachedNormalEnemies = new List<GameObject>();

    void Start()
    {
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
                Debug.LogError("EnemyTroop " + gameObject.name + " is not on a NavMesh!");
            }
        }
        else
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        }

        enemyStats = GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats component not found on " + gameObject.name);
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
                Debug.Log("Flee");
                FleeAndShoot();
            }
            else if (targetTank != null)
            {
                FollowTank();

                if (IsTankUnderAttack())
                {
                    if (shootingCoroutine == null)
                    {
                        FindAndAttackTargetAttackingTank();
                    }
                }
            }
            else
            {
                if (!IsTargeted())
                {
                    Debug.Log("Atack");
                    FindNearestNormalEnemy();
                }

                if (targetBuilding != null && shootingCoroutine == null)
                {
                    FindAndAttackTargetAttackingNormalEnemy();
                }
                else
                {
                    FindNearestAlly();
                    if (targetAlly != null && shootingCoroutine == null)
                    {
                        shootingCoroutine = StartCoroutine(ShootAlly());
                    }
                }
            }

            if (!IsTargeted() && targetTank == null && targetAlly == null && targetBuilding == null)
            {
                FindAndMoveToNearestAxon();
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
            if (ally.targetEnemyy == gameObject)
            {
                attacker = ally;
                targetAlly = ally;
                return true;
                
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
        cachedNormalEnemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    void FindAllyTroopTargetingTank()
    {
        foreach (AllyTroop troop in cachedAllyTroops)
        {
            if (troop.targetEnemyy == this.gameObject)
            {
                threateningTroop = troop;
                return;
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
            float distance = Vector3.Distance(transform.position, repairTroop.transform.position);
            if (distance < minDistance && distance <= maxSearchDistance)
            {
                minDistance = distance;
                nearestRepairTroop = repairTroop;
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
            if (ally.targetEnemyy == targetTank)
            {
                return true;
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
            float distance = Vector3.Distance(transform.position, ally.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestAlly = ally;
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
            float distance = Vector3.Distance(transform.position, tank.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTank = tank;
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
            if (ally.targetEnemyy == targetTank)
            {
                targetAlly = ally;
                Debug.Log("shoott");
                shootingCoroutine = StartCoroutine(ShootAlly());
                return;
            }
        }
    }

    void FindNearestNormalEnemy()
    {
        float minDistance = protectRange;
        GameObject nearestNormalEnemy = null;

        foreach (GameObject enemy in cachedNormalEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestNormalEnemy = enemy;
            }
        }

        targetBuilding = nearestNormalEnemy;
        if (targetBuilding != null)
        {
            agent.SetDestination(targetBuilding.transform.position);
        }
    }

    void FindAndAttackTargetAttackingNormalEnemy()
    {
        foreach (AllyTroop ally in cachedAllyTroops)
        {
            if (ally.targetEnemyy == targetBuilding)
            {
                targetAlly = ally;
                shootingCoroutine = StartCoroutine(ShootAlly());
                return;
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
                    Vector3 direction = (targetAlly.transform.position - transform.position).normalized;
                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
                }
                
            }
            yield return new WaitForSeconds(shootInterval);
        }

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
            float distance = Vector3.Distance(transform.position, axon.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestAxon = axon;

                if (distance < 2f && gameObject.CompareTag("Kamikaze"))
                {
                    Axon axonScript = axon.GetComponent<Axon>();
                    axonScript.TakeDamage(7f);
                    Destroy(gameObject);
                    // Explode
                }
            }
        }

        if (nearestAxon != null)
        {
            agent.SetDestination(nearestAxon.transform.position);

            // Check if the tank is within melee range of the axon
            if (gameObject.CompareTag("Enemy_Tank") && Vector3.Distance(transform.position, nearestAxon.transform.position) <= 2f || gameObject.CompareTag("Enemy") && Vector3.Distance(transform.position, nearestAxon.transform.position) <= 2f)
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
            shootingCoroutine = StartCoroutine(ShootAlly());
            stopFlee = true;
        }
       
    }


}