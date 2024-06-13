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
    public int numBuildings;

    public static MemoryTileConstruction selectedTile; // Track the selected tile for construction

    public static Building selectedBuilding; // Track the selected building for deconstruction

    void Start()
    {
        constructionMenu.SetActive(false); // Ensure the construction menu is initially inactive
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && numBuildings < 1)
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

    public void ConstructBuilding()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1)
        {
            GameObject currentBuilding = Instantiate(buildingPrefab, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            concentration.SubtractConcentration(20);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
        }
        else
        {
            Debug.Log("No tile selected or tile already has a building.");
        }


    }

    public void ConstructDefenseTower()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1)
        {
            GameObject currentBuilding = Instantiate(defenseTowerPrefab, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            concentration.SubtractConcentration(20);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
        }
        else
        {
            Debug.Log("No tile selected or tile already has a building.");
        }


    }

    public void ConstructUpgradedBarracks()
    {
        if (selectedTile != null && selectedTile.numBuildings < 1)
        {
            GameObject currentBuilding = Instantiate(upgradedBarracksPrefab, new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, selectedTile.transform.position.z - 0.5f), Quaternion.identity);
            concentration.SubtractConcentration(50);
            selectedTile.numBuildings++;
            currentBuilding.GetComponent<Building>().memoryTile = selectedTile; // Assign the memory tile to the building
            constructionMenu.SetActive(false);
            selectedTile = null; // Clear the selected tile
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
