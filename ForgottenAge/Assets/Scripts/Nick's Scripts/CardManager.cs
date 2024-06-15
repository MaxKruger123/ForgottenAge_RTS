using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    public CardScreen cardScreen;
    public Concentration concentration;
    public WaveManager waveManager;

    public List<GameObject> allyMeleeTroops;

    public float allyMeleeDamage = 1.0f;
    public float allyRangedDamage = 1.0f;
    public float allyShootInterval = 1.0f;
    public float enemyMeleeDamage = 1.0f;
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
        allyMeleeDamage = allyMeleeDamage * 2;
        // half ally health !!!!!!!!!!!!!!!!!!
    }
    public void TheDreamerCard()
    {
        // dream token !!!!!!!!!!!!!!!!
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
        // dream tokens and two building destroyed !!!!!!!!!!!!!!!!!!!!
    }

    public void PerilousInsightCard() // doesnt work con/ps added 3 was 6 and still is 6
    {
        // Increase concentration generation by 3 per second - Enemies deal 1.5x increased damage
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
        //All friendly troops deal 2x damage - Lose 2 Random Memory Tiles permanently
        allyMeleeDamage = allyMeleeDamage * 2;
        allyRangedDamage = allyRangedDamage * 2;
        // !!!!!!!!!!!!!!!!!!!!!!!!!!
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
