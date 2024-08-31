using UnityEngine;
using System.Collections;
using System.Linq;

public class CaptureZone : MonoBehaviour
{
    public float captureTime = 5.0f; // Time required to capture the platform

    private float captureProgress = 0;
    public string capturingSide = null;

    private int playerCount = 0;
    private int enemyCount = 0;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    public TileController tileController;

    public bool captured = false;
    public bool priceIncrease;
    public bool cantBuild;

    private MemoryTile nearestMemoryTile; // Reference to the nearest memory tile
    private MemoryTileConstruction numBuildings; // Reference to the MemoryTileConstruction associated with the nearest MemoryTile
    public Axon[] nearestAxons = new Axon[2];

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        spriteRenderer.color = Color.blue; // Start as captured
        captured = true;
    }

    private void Update()
    {
        if (nearestAxons[0].dead == true && nearestAxons[1].dead == false)
        {
            priceIncrease = true;
            cantBuild = false;
        }
        else

        if (nearestAxons[0].dead == false && nearestAxons[1].dead == true)
        {
            priceIncrease = true;
            cantBuild = false;
        }
        else if (nearestAxons[0].dead == true && nearestAxons[1].dead == true)
        {
            priceIncrease = false;
            cantBuild = true;
            spriteRenderer.color = Color.gray;
        } else if (nearestAxons[0].dead == false && nearestAxons[1].dead == false)
        {
            cantBuild = false;
            priceIncrease = false;
        }

        
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






    private IEnumerator FlashColor(Color flashColor)
    {
        while (true)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
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
}
