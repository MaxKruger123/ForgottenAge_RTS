using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxonManager : MonoBehaviour
{
    public int deadAxons = 0; // To store the number of dead Axons

    public MemoryTileConstruction memoryTileConstruction;

    // Update is called once per frame
    void Update()
    {
        CountDeadAxons();

        if (deadAxons == 1)
        {
            memoryTileConstruction.areaTowerPrice = 255;
            memoryTileConstruction.upgradedTowerPrice = 155;
            memoryTileConstruction.conStoragePrice = 205;
            memoryTileConstruction.upgradedBarracksPrice = 105;
            memoryTileConstruction.towerPrice = 45;
            memoryTileConstruction.barracksPrice = 25;
            memoryTileConstruction.constructionMenu.GetComponent<ConstructionMenu>().SetPrices(memoryTileConstruction.barracksPrice, memoryTileConstruction.towerPrice, memoryTileConstruction.upgradedBarracksPrice, memoryTileConstruction.conStoragePrice, memoryTileConstruction.upgradedTowerPrice, memoryTileConstruction.areaTowerPrice);
            

        } else if( deadAxons == 2)
        {
            memoryTileConstruction.areaTowerPrice = 260;
            memoryTileConstruction.upgradedTowerPrice = 160;
            memoryTileConstruction.conStoragePrice = 210;
            memoryTileConstruction.upgradedBarracksPrice = 110;
            memoryTileConstruction.towerPrice = 50;
            memoryTileConstruction.barracksPrice = 30;
            memoryTileConstruction.constructionMenu.GetComponent<ConstructionMenu>().SetPrices(memoryTileConstruction.barracksPrice, memoryTileConstruction.towerPrice, memoryTileConstruction.upgradedBarracksPrice, memoryTileConstruction.conStoragePrice, memoryTileConstruction.upgradedTowerPrice, memoryTileConstruction.areaTowerPrice);
           
        } else if ( deadAxons == 3)
        {
            memoryTileConstruction.areaTowerPrice = 265;
            memoryTileConstruction.upgradedTowerPrice = 165;
            memoryTileConstruction.conStoragePrice = 215;
            memoryTileConstruction.upgradedBarracksPrice = 115;
            memoryTileConstruction.towerPrice = 55;
            memoryTileConstruction.barracksPrice = 35;
            memoryTileConstruction.constructionMenu.GetComponent<ConstructionMenu>().SetPrices(memoryTileConstruction.barracksPrice, memoryTileConstruction.towerPrice, memoryTileConstruction.upgradedBarracksPrice, memoryTileConstruction.conStoragePrice, memoryTileConstruction.upgradedTowerPrice, memoryTileConstruction.areaTowerPrice);
        } else if( deadAxons == 4)
        {
            memoryTileConstruction.areaTowerPrice = 270;
            memoryTileConstruction.upgradedTowerPrice = 170;
            memoryTileConstruction.conStoragePrice = 220;
            memoryTileConstruction.upgradedBarracksPrice = 120;
            memoryTileConstruction.towerPrice = 60;
            memoryTileConstruction.barracksPrice = 40;
            memoryTileConstruction.constructionMenu.GetComponent<ConstructionMenu>().SetPrices(memoryTileConstruction.barracksPrice, memoryTileConstruction.towerPrice, memoryTileConstruction.upgradedBarracksPrice, memoryTileConstruction.conStoragePrice, memoryTileConstruction.upgradedTowerPrice, memoryTileConstruction.areaTowerPrice);
        } else if ( deadAxons == 5)
        {
            memoryTileConstruction.areaTowerPrice = 275;
            memoryTileConstruction.upgradedTowerPrice = 175;
            memoryTileConstruction.conStoragePrice = 225;
            memoryTileConstruction.upgradedBarracksPrice = 125;
            memoryTileConstruction.towerPrice = 65;
            memoryTileConstruction.barracksPrice = 45;
            memoryTileConstruction.constructionMenu.GetComponent<ConstructionMenu>().SetPrices(memoryTileConstruction.barracksPrice, memoryTileConstruction.towerPrice, memoryTileConstruction.upgradedBarracksPrice, memoryTileConstruction.conStoragePrice, memoryTileConstruction.upgradedTowerPrice, memoryTileConstruction.areaTowerPrice);
        } else if( deadAxons == 6)
        {
            memoryTileConstruction.areaTowerPrice = 280;
            memoryTileConstruction.upgradedTowerPrice = 180;
            memoryTileConstruction.conStoragePrice = 230;
            memoryTileConstruction.upgradedBarracksPrice = 130;
            memoryTileConstruction.towerPrice = 70;
            memoryTileConstruction.barracksPrice = 50;
            memoryTileConstruction.constructionMenu.GetComponent<ConstructionMenu>().SetPrices(memoryTileConstruction.barracksPrice, memoryTileConstruction.towerPrice, memoryTileConstruction.upgradedBarracksPrice, memoryTileConstruction.conStoragePrice, memoryTileConstruction.upgradedTowerPrice, memoryTileConstruction.areaTowerPrice);
        } else if ( deadAxons == 7)
        {
            memoryTileConstruction.areaTowerPrice = 285;
            memoryTileConstruction.upgradedTowerPrice = 185;
            memoryTileConstruction.conStoragePrice = 235;
            memoryTileConstruction.upgradedBarracksPrice = 135;
            memoryTileConstruction.towerPrice = 75;
            memoryTileConstruction.barracksPrice = 55;
            memoryTileConstruction.constructionMenu.GetComponent<ConstructionMenu>().SetPrices(memoryTileConstruction.barracksPrice, memoryTileConstruction.towerPrice, memoryTileConstruction.upgradedBarracksPrice, memoryTileConstruction.conStoragePrice, memoryTileConstruction.upgradedTowerPrice, memoryTileConstruction.areaTowerPrice);
        }
        else
        {
            return;
        }
    }

    void CountDeadAxons()
    {
        deadAxons = 0; // Reset deadAxons count

        // Find all Axon objects in the scene using both "Axon" and "DeadAxon" tags
        GameObject[] axons = GameObject.FindGameObjectsWithTag("Axon");
        GameObject[] deadAxonsList = GameObject.FindGameObjectsWithTag("DeadAxon");

        // Loop through all objects tagged as "Axon"
        foreach (GameObject axonObject in axons)
        {
            Axon axonScript = axonObject.GetComponent<Axon>();
            if (axonScript != null && axonScript.dead) // Check if the Axon is dead
            {
                deadAxons++; // Increment the deadAxons count
            }
        }

        // Loop through all objects tagged as "DeadAxon"
        foreach (GameObject axonObject in deadAxonsList)
        {
            Axon axonScript = axonObject.GetComponent<Axon>();
            if (axonScript != null && axonScript.dead) // Check if the Axon is dead
            {
                deadAxons++; // Increment the deadAxons count
            }
        }

        // Now the deadAxons variable holds the total number of dead Axons
    }
}
