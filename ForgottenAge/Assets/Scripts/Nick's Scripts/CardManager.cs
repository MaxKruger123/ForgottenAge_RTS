using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviour
{

    public CardScreen cardScreen;
    public Concentration concentration;
    public WaveManager waveManager;
    

    public List<GameObject> allyMeleeTroops;
    public List<GameObject> currentAllyMeleeTroops;

    public float allyMeleeDamage = 1.0f;
    public float allyRangedDamage = 1.0f;
    public float allyShootInterval = 1.0f;
    public float enemyMeleeDamage = 1.0f;
    public float allyMeleeMaxHealth = 10.0f;
    public int troopCostModifier = 0;
    public bool passiveHealing = false;
    public int incomeModifier;
    public float towerDamage = 2.0f;

    public GameObject memoryTile1;
    public GameObject memoryTile2;

    public GameObject enemyPrefab;

    public TextMeshProUGUI dreamTokenText;


    // Start is called before the first frame update
    void Start()
    {
        allyMeleeTroops = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void ConcentrationCard()
    {
        incomeModifier++;
        
    }

    public void GlassCannonCard()
    {
        allyMeleeMaxHealth = allyMeleeMaxHealth / 2;
        allyMeleeDamage = allyMeleeDamage * 2;
        currentAllyMeleeTroops = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach(GameObject meleeTroops in currentAllyMeleeTroops)
        {
            AllyTroopStats allyTroopStats = meleeTroops.GetComponent<AllyTroopStats>();
            if (allyTroopStats != null)
            {
                allyTroopStats.maxHealth = allyTroopStats.maxHealth / 2;
                allyTroopStats.currentHealth = allyTroopStats.maxHealth;
            }
            else
            {
                Debug.Log("Cannot find ally troop stats");
            }
            
        }


       
    }
    public void TheDreamerCard()
    {
        concentration.dreamTokens++;
        dreamTokenText.text = " " + concentration.dreamTokens;
    }

    public void FocusCard() // WORKS
    {
        concentration.AddConcentration(100);
        troopCostModifier = troopCostModifier + 5;
        // troop cost +5
        Debug.Log("concentration added " + cardScreen.GetCardData(3).buffValue + "\n new total concentration is " + concentration.GetConcentration());
    }

    public void DreamersResolveCard()
    {
        concentration.dreamTokens += 3;
        dreamTokenText.text = " " + concentration.dreamTokens;

        // Find all buildings with the "Building" tag
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        // Filter out the "MainBrain" building
        buildings = buildings.Where(building => building.name != "MainBrain").ToArray();

        // Check if there are at least two buildings left to destroy
        if (buildings.Length < 2)
        {
            Debug.Log("Not enough buildings to destroy.");
            return;
        }

        // Shuffle the array to get random buildings
        System.Random random = new System.Random();
        for (int i = buildings.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            GameObject temp = buildings[i];
            buildings[i] = buildings[j];
            buildings[j] = temp;
        }

        // Select the first two buildings for destruction
        GameObject building1 = buildings[0];
        GameObject building2 = buildings[1];

        // Find the closest MemoryTileConstruction component to each building
        MemoryTileConstruction closestTile1 = FindClosestTile(building1.transform.position);
        MemoryTileConstruction closestTile2 = FindClosestTile(building2.transform.position);

        // Decrement numBuildings for the respective tiles
        if (closestTile1 != null)
        {
            closestTile1.numBuildings--;
        }

        if (closestTile2 != null)
        {
            closestTile2.numBuildings--;
        }

        // Destroy the selected buildings
        Destroy(building1);
        Destroy(building2);

        // Log the destruction for debugging purposes
        Debug.Log("Destroyed two random buildings: " + building1.name + " and " + building2.name);
    }

    public MemoryTileConstruction FindClosestTile(Vector3 position)
    {
        MemoryTileConstruction[] tiles = FindObjectsOfType<MemoryTileConstruction>();
        MemoryTileConstruction closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (MemoryTileConstruction tile in tiles)
        {
            float distance = Vector3.Distance(position, tile.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }

    public void PerilousInsightCard() 
    {
        incomeModifier++;
        enemyMeleeDamage = enemyMeleeDamage + 2;
    }

    public void FortifiedRespiteCard()
    {
        //Your ally tanks heal passively - Your ranged units take longer to shoot
        passiveHealing = true;
        allyShootInterval = allyShootInterval + 0.5f;
    }

    public void CursedBlessingCard()
    {
        // All friendly troops deal 2x damage
        allyMeleeDamage *= 2;
        allyRangedDamage *= 2;

        // Find all memory tiles
        MemoryTile[] memoryTiles = FindObjectsOfType<MemoryTile>();

        // Check if there are at least two memory tiles to lose
        if (memoryTiles.Length < 2)
        {
            Debug.Log("Not enough memory tiles to destroy.");
            return;
        }

        // Shuffle the array to get random memory tiles
        System.Random random = new System.Random();
        for (int i = memoryTiles.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            MemoryTile temp = memoryTiles[i];
            memoryTiles[i] = memoryTiles[j];
            memoryTiles[j] = temp;
        }

        // Destroy buildings and tiles on the first two memory tiles in the shuffled array
        DestroyBuildingAndTile(memoryTiles[0]);
        DestroyBuildingAndTile(memoryTiles[1]);

        // Log the destruction for debugging purposes
        Debug.Log("Destroyed buildings and tiles on two random memory tiles: " + memoryTiles[0].name + " and " + memoryTiles[1].name);
    }

    private void DestroyBuildingAndTile(MemoryTile memoryTile)
    {
        if (memoryTile.building != null)
        {
            Destroy(memoryTile.building);
            Debug.Log("Destroyed building on memory tile: " + memoryTile.name);
        }

        Destroy(memoryTile.gameObject);
        Debug.Log("Destroyed memory tile: " + memoryTile.name);
    }


    public void NeuronActivationCard()
    {
        //Gain 400 concentration immediately - skip 3 enemy waves
        concentration.AddConcentration(400);
        waveManager.SetCurrentWave(waveManager.GetCurrentWave() + 3);
        
    }

    public void ShiftingTidesCard()
    {
        // Gain 2 new memory tiles to build on - Every wave has a chance for allies to turn into enemies
        memoryTile1.SetActive(true);
        memoryTile2.SetActive(true);

        // Find all current allies with "Player" and "AllyRanged" tags
        List<GameObject> allies = new List<GameObject>();
        allies.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        allies.AddRange(GameObject.FindGameObjectsWithTag("AllyRanged"));

        // Iterate over each ally and apply the 2 percent chance logic
        foreach (GameObject ally in allies)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue < 5f)
            {
                Vector3 allyPosition = ally.transform.position;
                Destroy(ally);
                Instantiate(enemyPrefab, allyPosition, Quaternion.identity);
            }
        }

    }

    public void CognitiveFortificationCard()
    {
        // Find all ally troops with the "Player" and "AllyRanged" tags
        List<GameObject> allies = new List<GameObject>();
        allies.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        allies.AddRange(GameObject.FindGameObjectsWithTag("AllyRanged"));

        // Iterate over each ally and increase their health by 20%
        foreach (GameObject ally in allies)
        {
            AllyTroopStats allyTroopStats = ally.GetComponent<AllyTroopStats>();
            if (allyTroopStats != null)
            {
                allyTroopStats.maxHealth *= 1.2f;
                allyTroopStats.currentHealth *= 1.2f;
            }
            else
            {
                Debug.LogWarning("AllyTroopStats component not found on " + ally.name);
            }
        }
    }

    public void StructuralRecallCard()
    {
        // Heal all Buildings
        List<GameObject> buidlings = new List<GameObject>(GameObject.FindGameObjectsWithTag("Building"));
        for(int i =0; i < buidlings.Count; i++)
        {
            buidlings[i].GetComponent<BuildingStats>().currentHealth = buidlings[i].GetComponent<BuildingStats>().maxHealth;
        }
    }

    public void SynapticOverloadCard()
    {
        towerDamage = towerDamage *= 1.5f;
    }


}
