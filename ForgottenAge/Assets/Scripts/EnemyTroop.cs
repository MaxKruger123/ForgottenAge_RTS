using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTroop : MonoBehaviour
{
    public float attackRange = 2f; // Range for attacking
    public float minDistanceToAlly = 1.5f; // Minimum distance to ally to avoid touching
    public float rangedAttackRange = 5f; // Range for ranged attacking
    public float protectRange = 100f; // Range to find and protect the nearest Tank
    public GameObject projectilePrefab; // Projectile prefab to be shot by the ranged enemy
    public float projectileSpeed = 10f; // Speed of the projectile
    public float shootInterval = 1f; // Interval between shots
    public float retreatHealthThreshold = 20f; // Health threshold to trigger retreat
    public float retreatTime = 7f; // Time to retreat before returning

    public AllyTroop targetAlly; // Reference to the nearest ally
    private GameObject targetMemoryTile; // Reference to the nearest MemoryTile
    public GameObject targetBuilding; // Reference to the nearest building
    private bool isAttacking = false; // Flag to indicate if the enemy is attacking
    private NavMeshAgent agent; // Reference to the NavMeshAgent
    private Coroutine shootingCoroutine; // Coroutine for shooting
    private Coroutine meleeCoroutine;

    private EnemyStats enemyStats; // Reference to the enemy's stats
    private CardManager cardManager;
    private GameObject targetAxon;
    public GameObject targetTank; // Reference to the nearest Tank to protect
    private Coroutine retreatCoroutine;
    GameObject nearestTank;
    public bool tankUnderAttack = false;

    private AllyTroop attacker;



    void Start()
    {
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
    }

    void Update()
    {

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
                FindAndMoveToNearestAxon();
            }
        }

        if (gameObject.CompareTag("Enemy_Tank"))
        {
           FindAndMoveToNearestAxon();
        }

        if (gameObject.CompareTag("EnemyRanged"))
        {
            FindNearestTank();

            if (IsTargeted())
            {
                Debug.Log("Targeted");
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
                FindNearestNormalEnemy();
                if (targetBuilding != null && shootingCoroutine == null)
                {
                    
                    FindAndAttackTargetAttackingNormalEnemy();
                }
                else
                {
                    FindNearestAlly();
                    if (targetAlly != null && shootingCoroutine == null)
                    {
                        Debug.Log("Wtf is going on");
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
        return false;
    }

    IEnumerator MeleeAttack()
    {
        if (targetAlly != null)
        {
            // Add your melee attack logic here, for example:
             AllyTroopStats targetAllyStats = targetAlly.GetComponent<AllyTroopStats>();
            targetAllyStats.TakeDamage(1.0f);
            
            
            
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
        
        AllyTroop[] allyTroops = FindObjectsOfType<AllyTroop>();
        

        foreach (AllyTroop ally in allyTroops)
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
        AllyTroop[] allyTroop = FindObjectsOfType<AllyTroop>();

        float minDistance = Mathf.Infinity;
        AllyTroop nearestAlly = null;

        foreach (AllyTroop ally in allyTroop)
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
       
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Enemy_Tank");

       

        float minDistance = protectRange;
        

        foreach (GameObject tank in tanks)
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
        else
        {
            
        }
    }


    void FindAndAttackTargetAttackingTank()
    {
        
        AllyTroop[] allyTroop = FindObjectsOfType<AllyTroop>();

        foreach (AllyTroop ally in allyTroop)
        {
            if (ally.targetEnemyy == targetTank)
            {
                targetAlly = ally;
                shootingCoroutine = StartCoroutine(ShootAlly());
                return;
            }
        }
    }

    void FindNearestNormalEnemy()
    {
        GameObject[] normalEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        float minDistance = protectRange;
        GameObject nearestNormalEnemy = null;

        foreach (GameObject enemy in normalEnemies)
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
        AllyTroop[] allyTroop = FindObjectsOfType<AllyTroop>();

        foreach (AllyTroop ally in allyTroop)
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
        if (targetAlly != null)
        {
            Vector3 direction = (targetAlly.transform.position - transform.position).normalized;
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

            
        }
        yield return new WaitForSeconds(2.0f);
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
            Debug.Log("No Axons found on the map.");
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

                if (distance < 2f)
                {
                    Destroy(gameObject, 1.0f);
                    //Explode
                }
            } 
        }

        if (nearestAxon != null)
        {
            agent.SetDestination(nearestAxon.transform.position);
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

    void FleeAndShoot()
    {
        if (attacker != null)
        {
            // Calculate the direction to move away from the attacker
            Vector3 fleeDirection = (transform.position - attacker.transform.position).normalized;
            Vector3 fleePosition = transform.position + fleeDirection * 5f; // Adjust the distance as needed

            // Move the ranged enemy away from the attacker
            agent.SetDestination(fleePosition);

            // Shoot at the attacker while fleeing
            if (!isAttacking && shootingCoroutine == null)
            {
                shootingCoroutine = StartCoroutine(ShootAlly());
                Debug.Log("Flee and Shoot");
            }
        }
    }

    
}
