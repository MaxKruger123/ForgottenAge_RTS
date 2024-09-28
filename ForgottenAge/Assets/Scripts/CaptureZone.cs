using UnityEngine;
using System.Collections;
using System.Linq;

public class CaptureZone : MonoBehaviour
{
    public float captureTime = 5.0f; // Time required to capture the platform

    private float captureProgress = 0;
    public string capturingSide = null;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    public TileController tileController;

    public bool captured = false;
    public bool priceIncrease;
    public bool cantBuild;

    private MemoryTile nearestMemoryTile; // Reference to the nearest memory tile
    private MemoryTileConstruction numBuildings; // Reference to the MemoryTileConstruction associated with the nearest MemoryTile
    public Axon[] nearestAxons = new Axon[2];

    private TutorialManager tutorialManager;
    private MemoryTileConstruction memoryTileConstruction;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        FindNearestMemoryTile();
        FindNearestAxons();

        if (nearestMemoryTile == null)
        {
            Debug.LogError("No MemoryTile found in the scene.");
        }
        else
        {
            numBuildings = nearestMemoryTile.GetComponent<MemoryTileConstruction>();
            if (numBuildings == null)
            {
                Debug.LogError("MemoryTileConstruction component not found in nearest MemoryTile: " + nearestMemoryTile.name);
            }
        }

        SetInitialColor();

        tutorialManager = FindObjectOfType<TutorialManager>();
        memoryTileConstruction = GetComponent<MemoryTileConstruction>();
    }

    private void Update()
    {
        UpdateCaptureState();
        if (!isFlashing)
        {
            UpdateColor();
        }
    }

    private void UpdateCaptureState()
    {
        if (nearestAxons[0].dead == true && nearestAxons[1].dead == false)
        {
            priceIncrease = true;
            cantBuild = false;
        }
        else if (nearestAxons[0].dead == false && nearestAxons[1].dead == true)
        {
            priceIncrease = true;
            cantBuild = false;
        }
        else if (nearestAxons[0].dead == true && nearestAxons[1].dead == true)
        {
            priceIncrease = false;
            cantBuild = true;
        }
        else if (nearestAxons[0].dead == false && nearestAxons[1].dead == false)
        {
            cantBuild = false;
            priceIncrease = false;
        }
    }

    private void UpdateColor()
    {
        if (cantBuild)
        {
            spriteRenderer.color = Color.gray;
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void SetInitialColor()
    {
        originalColor = Color.blue;
        spriteRenderer.color = originalColor;
        captured = true;
    }

    public void StartFlashing()
    {
        isFlashing = true;
    }

    public void StopFlashing()
    {
        isFlashing = false;
        UpdateColor();
    }

    private void FindNearestMemoryTile()
    {
        MemoryTile[] memoryTiles = FindObjectsOfType<MemoryTile>();
        float closestDistance = Mathf.Infinity;

        foreach (MemoryTile tile in memoryTiles)
        {
            float distance = Vector3.Distance(transform.position, tile.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestMemoryTile = tile;
            }
        }
    }

    public Axon[] FindNearestAxons()
    {
        // Find all objects with the "Axon" tag
        GameObject[] allAxons = GameObject.FindGameObjectsWithTag("Axon");

        if (allAxons.Length < 2)
        {
            Debug.LogWarning("Less than two axons found in the scene.");
            return null;
        }

        // Sort the axons by distance to this game object
        var sortedAxons = allAxons
            .OrderBy(axon => Vector3.Distance(transform.position, axon.transform.position))
            .Take(2)
            .ToArray();

        // Get the Axon script from each nearest axon
        nearestAxons[0] = sortedAxons[0].GetComponent<Axon>();
        nearestAxons[1] = sortedAxons[1].GetComponent<Axon>();

        return nearestAxons;
    }

    public void ChangeColorToBlue()
    {
        spriteRenderer.color = Color.blue;
    }

    private void DestroyBuildingOnTile()
    {
        if (nearestMemoryTile != null && nearestMemoryTile.building != null)
        {
            Destroy(nearestMemoryTile.building);
            nearestMemoryTile.building = null;
            Debug.Log("Destroyed building on memory tile: " + nearestMemoryTile.name);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            if (memoryTileConstruction.numBuildings < 1 && captured && !cantBuild)
            {
                // Open the construction menu
                Vector3 mouseScreenPosition = Input.mousePosition;
                memoryTileConstruction.constructionMenu.SetActive(true);
                memoryTileConstruction.constructionMenu.transform.position = mouseScreenPosition;

                // Notify the tutorial manager that the build menu has been opened
                OnBuildMenuOpened();
            }
            else if (memoryTileConstruction.numBuildings > 0)
            {
                // Handle existing buildings
                Building building = GetComponentInChildren<Building>();
                if (building != null)
                {
                    OpenBuildingMenu(building);
                }
            }
        }
    }

    private void OpenBuildingMenu(Building building)
    {
        switch (building.buildingType)
        {
            case Building.BuildingType.Default:
                MemoryTileConstruction.selectedBuilding = building;
                building.recruitmentMenu.SetActive(true);
                building.recruitmentMenu.transform.position = Input.mousePosition;
                building.recruitmentMenu.GetComponent<RecruitmentMenu>().SetButton(building);
                break;
            case Building.BuildingType.DefenseTower:
            case Building.BuildingType.UpgradedDefenseTower:
            case Building.BuildingType.AreaDamageTower:
            case Building.BuildingType.ConcentrationStorage:
                building.deconstructMenu.SetActive(true);
                building.deconstructMenu.transform.position = Input.mousePosition;
                MemoryTileConstruction.selectedBuilding = building;
                break;
            case Building.BuildingType.UpgradedBarracks:
                MemoryTileConstruction.selectedBuilding = building;
                building.recruitmentMenuTwo.SetActive(true);
                building.recruitmentMenuTwo.transform.position = Input.mousePosition;
                building.recruitmentMenuTwo.GetComponent<RecruitmentMenuTwo>().SetButton(building);
                break;
        }
    }

    public void OnBuildMenuOpened()
    {
        if (tutorialManager != null)
        {
            tutorialManager.OnBuildMenuOpened();
        }
        else
        {
            Debug.LogWarning("TutorialManager is null in CaptureZone.OnBuildMenuOpened()");
        }
    }

    public void OnBuildingSelected(Building.BuildingType buildingType)
    {
        if (tutorialManager != null)
        {
            tutorialManager.OnBuildingSelected(buildingType);
        }
    }
}
