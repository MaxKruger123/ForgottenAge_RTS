using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public int incomeModifier = 0;
    
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
        incomeModifier = incomeModifier + cardScreen.GetCardData(0).buffValue;
        Debug.Log("concentration added " + cardScreen.GetCardData(0).buffValue + "\n new total concentration is " + concentration.GetConcentration());
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


        // half ally health !!!!!!!!!!!!!!!!!!
    }
    public void TheDreamerCard()
    {
        concentration.dreamTokens++;
    }

    public void FocusCard() // WORKS
    {
        concentration.AddConcentration(cardScreen.GetCardData(3).buffValue);
        troopCostModifier = troopCostModifier + 5;
        // troop cost +5
        Debug.Log("concentration added " + cardScreen.GetCardData(3).buffValue + "\n new total concentration is " + concentration.GetConcentration());
    }

    public void DreamersResolveCard()
    {
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

        // Destroy the first two buildings in the shuffled array
        Destroy(buildings[0]);
        Destroy(buildings[1]);

        // Log the destruction for debugging purposes
        Debug.Log("Destroyed two random buildings: " + buildings[0].name + " and " + buildings[1].name);
    }

    public void PerilousInsightCard() 
    {
        incomeModifier = incomeModifier + cardScreen.GetCardData(5).buffValue;
        enemyMeleeDamage = enemyMeleeDamage + cardScreen.GetCardData(5).debuffValue;
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

        // Destroy buildings on the first two memory tiles in the shuffled array
        DestroyBuildingOnTile(memoryTiles[0]);
        DestroyBuildingOnTile(memoryTiles[1]);



        memoryTiles[0]

        // Log the destruction for debugging purposes
        Debug.Log("Destroyed buildings on two random memory tiles: " + memoryTiles[0].name + " and " + memoryTiles[1].name);
    }

    private void DestroyBuildingOnTile(MemoryTile memoryTile)
    {
        if (memoryTile.building != null)
        {
            Destroy(memoryTile.building);
            memoryTile.building = null;
            Debug.Log("Destroyed building on memory tile: " + memoryTile.name);
        }
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
        // !!!!!!!!!!!!!!!!!!!
    }

    public void CognitiveFortificationCard()
    {
        //Allies gain 20% increased health
        //!!!!!!!!!!!!!!!!!!!
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
        //Your Defense towers deal 1.5x more damage.
    }
}
