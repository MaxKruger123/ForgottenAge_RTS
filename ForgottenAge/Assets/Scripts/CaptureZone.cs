using UnityEngine;
using System.Collections;

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

    private MemoryTile nearestMemoryTile; // Reference to the nearest memory tile
    private MemoryTileConstruction numBuildings; // Reference to the MemoryTileConstruction associated with the nearest MemoryTile

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        FindNearestMemoryTile();

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

        spriteRenderer.color = Color.blue; // Start as uncaptured
        captured = true;
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

    

    

    private void Update()
    {
       
        
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
