using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    public enum BuildingType
    {
        Default,
        DefenseTower,
        UpgradedBarracks,
        ConcentrationStorage
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
    private Queue<TroopType> troopQueue = new Queue<TroopType>();
    private Coroutine spawnTroopCoroutine;

    public GameObject spawnEffect;
    public TextMeshProUGUI queueText;
    public Image queueImage;


    public CardManager cardManager;
    

    private enum TroopType
    {
        Ally,
        Ranged,
        Healing,
        Tank
        
    }

    void Start()
    {
        concentration = FindAnyObjectByType<Concentration>();
        menuManager = concentration.gameObject.GetComponent<MenuManager>();
        recruitmentMenu = menuManager.GetMenuObject("RecruitmentMenu");
        recruitmentMenuTwo = menuManager.GetMenuObject("RecruitmentMenuTwo");
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();

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

        // Update queue UI
        queueText.text = troopQueue.Count.ToString();
        if (spawnTroopCoroutine != null)
        {
            float buildTime = GetBuildTime(troopQueue.Peek());
            queueImage.fillAmount += Time.deltaTime / buildTime;
        }
    }

    // Coroutine for shooting at regular intervals
    private IEnumerator ShootRoutine()
    {
        while (true)
        {
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
        }
        else if (Input.GetMouseButtonDown(1) && buildingType == BuildingType.DefenseTower)
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

    private void EnqueueTroop(TroopType troopType, int concentrationCost)
    {
        if (concentration.GetConcentration() >= concentrationCost)
        {
            troopQueue.Enqueue(troopType);
            concentration.SubtractConcentration(concentrationCost);

            if (spawnTroopCoroutine == null)
            {
                spawnTroopCoroutine = StartCoroutine(SpawnTroopQueue());
            }
        }
    }

    private float GetBuildTime(TroopType troopType)
    {
        switch (troopType)
        {
            case TroopType.Ally:
                return 8f;
            case TroopType.Ranged:
                return 15f;
            case TroopType.Healing:
                return 10f;
            case TroopType.Tank:
                return 25f;
            default:
                return 5f;
        }
    }

    private IEnumerator SpawnTroopQueue()
    {
        while (troopQueue.Count > 0)
        {
            TroopType troopType = troopQueue.Peek();
            float buildTime = GetBuildTime(troopType);
            queueImage.fillAmount = 0; // Reset the progress bar

            for (float timer = 0; timer < buildTime; timer += Time.deltaTime)
            {
                queueImage.fillAmount = timer / buildTime;
                yield return null;
            }

            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

            switch (troopType)
            {
                case TroopType.Ally:
                    Instantiate(allyTroopPrefab, spawnPosition, Quaternion.identity);
                    break;
                case TroopType.Ranged:
                    Instantiate(rangedAllyTroopPrefab, spawnPosition, Quaternion.identity);
                    break;
                case TroopType.Healing:
                    Instantiate(rangedHealingTroopPrefab, spawnPosition, Quaternion.identity);
                    break;
                case TroopType.Tank:
                    Instantiate(tankTroopPrefab, spawnPosition, Quaternion.identity);
                    break;
            }

            Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
            troopQueue.Dequeue();
        }

        spawnTroopCoroutine = null;
    }

    public void SpawnTroop()
    {
        EnqueueTroop(TroopType.Ally, 5 + cardManager.troopCostModifier);
    }

    public void SpawnTroopInstant()
    {

        if (concentration.dreamTokens > 1)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);
            Instantiate(allyTroopPrefab, spawnPosition, Quaternion.identity);
            concentration.dreamTokens--;
        }
        
    }

    public void SpawnTankTroop()
    {
        EnqueueTroop(TroopType.Tank, 25 + cardManager.troopCostModifier);
    }

    public void SpawnTankTroopInstant()
    {
        if (concentration.dreamTokens > 1)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);
            Instantiate(tankTroopPrefab, spawnPosition, Quaternion.identity);
            concentration.dreamTokens --;
        }
    }

    public void SpawnRangedTroop()
    {
        EnqueueTroop(TroopType.Ranged, 10 + cardManager.troopCostModifier);
    }

    public void SpawnRangedTroopInstant()
    {
        if (concentration.dreamTokens > 1)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);
            Instantiate(allyTroopPrefab, spawnPosition, Quaternion.identity);
            concentration.dreamTokens--;
        }
    }

    public void SpawnHealingTroop()
    {
        EnqueueTroop(TroopType.Healing, 10 + cardManager.troopCostModifier);
    }

    public void SpawnHealingTroopInstant()
    {
        if (concentration.dreamTokens > 1)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);
            Instantiate(rangedHealingTroopPrefab, spawnPosition, Quaternion.identity);
            concentration.dreamTokens--;
        }
    }
}
