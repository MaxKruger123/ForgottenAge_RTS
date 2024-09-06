using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryTileConstruction : MonoBehaviour
{
    public Concentration concentration;
    public GameObject constructionMenu;
    public GameObject buildingPrefab;
    public GameObject defenseTowerPrefab;
    public GameObject upgradedBarracksPrefab;
    public GameObject concentrationStorage;
    public GameObject upgradedDefenseTower;
    public GameObject areaDamageTower;
    public int numBuildings;
    public CaptureZone captureZone;

    public int barracksPrice;
    public int towerPrice;
    public int upgradedBarracksPrice;
    public int conStoragePrice;
    public int upgradedTowerPrice;
    public int areaTowerPrice;

    private bool priceFlag;

    public GameObject spawnEffect;

    public static MemoryTileConstruction selectedTile; // Track the selected tile for construction

    public static Building selectedBuilding; // Track the selected building for deconstruction

    void Start()
    {
        
        constructionMenu.SetActive(false); // Ensure the construction menu is initially inactive
        concentration = GameObject.Find("UICanvas").GetComponent<Concentration>();
        areaTowerPrice = 250;
        upgradedTowerPrice = 150;
        conStoragePrice = 200;
        upgradedBarracksPrice = 100;
        towerPrice = 40;
        barracksPrice = 20;
        
        constructionMenu.GetComponent<ConstructionMenu>().SetPrices(barracksPrice, towerPrice, upgradedBarracksPrice, conStoragePrice, upgradedTowerPrice, areaTowerPrice);
    }

     void Update()
     {
        
        
        if (captureZone.cantBuild == false)
        {
            captureZone.ChangeColorToBlue();
        }
     }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && numBuildings < 1 && captureZone.captured && captureZone.cantBuild == false)
        {
            selectedTile = this; // Set the selected tile for construction
            Vector3 mouseScreenPosition = Input.mousePosition;
            constructionMenu.SetActive(true);
            constructionMenu.transform.position = mouseScreenPosition;

            // Enable/disable deconstruct button based on whether there's a building
            if (selectedBuilding != null)
            {
                constructionMenu.GetComponent<ConstructionMenu>().deconstructButton.interactable = true;
            }
            else
            {
                constructionMenu.GetComponent<ConstructionMenu>().deconstructButton.interactable = false;
            }
        }
    }

    public void AllPricesIncrease()
    {
        areaTowerPrice += 5;
        upgradedTowerPrice += 5;
        conStoragePrice += 5;
        upgradedBarracksPrice += 5;
        towerPrice += 5;
        barracksPrice += 5;
        Debug.Log("Price Increase: " + barracksPrice);
        constructionMenu.GetComponent<ConstructionMenu>().SetPrices(barracksPrice, towerPrice, upgradedBarracksPrice, conStoragePrice, upgradedTowerPrice, areaTowerPrice);
    }

    public void AllPricesDecrease()
    {
        areaTowerPrice -= 5;
        upgradedTowerPrice -= 5;
        conStoragePrice -= 5;
        upgradedBarracksPrice -= 5;
        towerPrice -= 5;
        barracksPrice -= 5;
        Debug.Log("Price Decrease: " + barracksPrice);
        constructionMenu.GetComponent<ConstructionMenu>().SetPrices(barracksPrice, towerPrice, upgradedBarracksPrice, conStoragePrice, upgradedTowerPrice, areaTowerPrice);
    }

    public void ConstructBuilding()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1 && concentration.concentration >= barracksPrice && captureZone.cantBuild == false)
        {
            GameObject currentBuilding = Instantiate(buildingPrefab, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            
            concentration.SubtractConcentration(barracksPrice);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
            Instantiate(spawnEffect, currentBuilding.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("No tile selected or tile already has a building.");
        }


    }

    public void ConstructConcentrationStorage()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1 && concentration.concentration >= conStoragePrice && captureZone.cantBuild == false)
        {
            GameObject currentBuilding = Instantiate(concentrationStorage, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            concentration.SubtractConcentration(conStoragePrice);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
            concentration.CS = true;
            Instantiate(spawnEffect, currentBuilding.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("No tile selected or tile already has a building.");
        }


    }

    public void ConstructDefenseTower()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1 && concentration.concentration >= towerPrice && captureZone.cantBuild == false)
        {
            GameObject currentBuilding = Instantiate(defenseTowerPrefab, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            concentration.SubtractConcentration(towerPrice);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
            Instantiate(spawnEffect, currentBuilding.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("No tile selected or tile already has a building.");
        }


    }

    public void ConstructUpgradedBarracks()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1 && concentration.concentration >= upgradedBarracksPrice && captureZone.cantBuild == false)
        {
            GameObject currentBuilding = Instantiate(upgradedBarracksPrefab, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            concentration.SubtractConcentration(upgradedBarracksPrice);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
            Instantiate(spawnEffect, currentBuilding.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("No tile selected or tile already has a building.");
        }


    }

    public void ConstructUpgradedDefenseTower()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1 && concentration.concentration >= upgradedTowerPrice && captureZone.cantBuild == false)
        {
            
            GameObject currentBuilding = Instantiate(upgradedDefenseTower, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            concentration.SubtractConcentration(upgradedTowerPrice);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
            Instantiate(spawnEffect, currentBuilding.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("No tile selected or tile already has a building.");
        }


    }

    public void ConstructAreaDamageTower()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1 && concentration.concentration >= areaTowerPrice && captureZone.cantBuild == false)
        {
            GameObject currentBuilding = Instantiate(areaDamageTower, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            concentration.SubtractConcentration(areaTowerPrice);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
            Instantiate(spawnEffect, currentBuilding.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("No tile selected or tile already has a building.");
        }


    }

    public void DeconstructBuilding()
    {
        if (selectedBuilding != null)
        {
            // Deconstruct the selected building
            Destroy(selectedBuilding.gameObject);
            selectedBuilding.memoryTile.numBuildings--; // Decrement the number of buildings on the associated tile
            selectedBuilding = null; // Clear the selected building reference
        }
        else
        {
            Debug.Log("No building selected.");
        }
    }

  
}
