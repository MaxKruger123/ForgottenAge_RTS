using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingType
    {
        Default,
        DefenseTower,
        UpgradedBarracks
    }

    public BuildingType buildingType = BuildingType.Default;
    public GameObject projectilePrefab; // Projectile prefab to be shot by the defense tower
    public float projectileSpeed = 10f;
    public float shootInterval = 1f;
    public float shootTimer = 0f; // Made public for debugging purposes
    public float detectionRadius = 5f; // Radius for detecting enemies

    public GameObject allyTroopPrefab;
    public GameObject rangedAllyTroopPrefab;
    public GameObject rangedHealingTroopPrefab;
    public GameObject tankTroopPrefab;
    public GameObject recruitmentMenu;
    public GameObject recruitmentMenuTwo;
    public MenuManager menuManager;

    public MemoryTileConstruction memoryTile;
    public Concentration concentration;
    public float spawnRadius = 4f;

    private Coroutine shootingCoroutine;
    public GameObject spawnEffect;

    void Start()
    {
        concentration = FindAnyObjectByType<Concentration>();
        menuManager = concentration.gameObject.GetComponent<MenuManager>();
        recruitmentMenu = menuManager.GetMenuObject("RecruitmentMenu");
        recruitmentMenuTwo = menuManager.GetMenuObject("RecruitmentMenuTwo");

        // Start shooting coroutine for defense towers
        if (buildingType == BuildingType.DefenseTower)
        {
            
            shootingCoroutine = StartCoroutine(ShootRoutine());
        }
    }

    void Update()
    {
        // Increment the shoot timer
        shootTimer += Time.deltaTime;
    }

    // Coroutine for shooting at regular intervals
    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Debug.Log("Shoot?");
            // Shoot at the nearest enemy
            ShootAtNearestEnemy();

            // Wait for the shoot interval
            yield return new WaitForSeconds(shootInterval);
        }
    }

    // Method to shoot at the nearest enemy
    private void ShootAtNearestEnemy()
    {
        // Find all enemies within detection radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));

        if (colliders.Length > 0)
        {
            // Find the nearest enemy
            float minDistance = Mathf.Infinity;
            Transform nearestEnemy = null;
            foreach (Collider2D collider in colliders)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = collider.transform;
                    Debug.Log(nearestEnemy.gameObject.name);
                }
            }

            // Shoot at the nearest enemy
            if (nearestEnemy != null)
            {
                ShootProjectile(nearestEnemy.position);
            }
        }
    }

    // Method to shoot a projectile
    private void ShootProjectile(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

        shootTimer = 0f;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && buildingType == BuildingType.Default)
        {
            // Set the selected building to this building
            MemoryTileConstruction.selectedBuilding = this;

            Vector3 mouseScreenPosition = Input.mousePosition;
            recruitmentMenu.gameObject.SetActive(true);
            recruitmentMenu.transform.position = mouseScreenPosition;
            recruitmentMenu.GetComponent<RecruitmentMenu>().SetButton(gameObject.GetComponent<Building>());
        } else if (Input.GetMouseButtonDown(1) && buildingType == BuildingType.DefenseTower)
        {
            
        }
        else if (Input.GetMouseButtonDown(1) && buildingType == BuildingType.UpgradedBarracks)
        {
            // Set the selected building to this building
            MemoryTileConstruction.selectedBuilding = this;

            Vector3 mouseScreenPosition = Input.mousePosition;
            recruitmentMenuTwo.gameObject.SetActive(true);
            recruitmentMenuTwo.transform.position = mouseScreenPosition;
            recruitmentMenuTwo.GetComponent<RecruitmentMenuTwo>().SetButton(gameObject.GetComponent<Building>());
        }
    }

    public void SpawnTroop()
    {
        if (concentration.GetConcentration() >= 5)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

            Instantiate(allyTroopPrefab, spawnPosition, Quaternion.identity);
            GameObject effect = Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
            effect.transform.parent = null;
            concentration.SubtractConcentration(5);
        }
    }

    public void SpawnTankTroop()
    {
        
        if (concentration.GetConcentration() >= 25)
        {
            Debug.Log("Spawn Tank Troop");
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

            Instantiate(tankTroopPrefab, spawnPosition, Quaternion.identity);
            Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
            concentration.SubtractConcentration(25);

        }
        else
        {
            Debug.Log("Error");
        }
    }

    public void SpawnRangedTroop()
    {
        if (concentration.GetConcentration() >= 10)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

            Instantiate(rangedAllyTroopPrefab, spawnPosition, Quaternion.identity);
            Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
            concentration.SubtractConcentration(10);
        }
    }

    public void SpawnHealingTroop()
    {
        if (concentration.GetConcentration() >= 10)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

            Instantiate(rangedHealingTroopPrefab, spawnPosition, Quaternion.identity);
            Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
            concentration.SubtractConcentration(10);
        }
    }

}
