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

        spriteRenderer.color = Color.white; // Start as uncaptured
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AllyRanged") || other.CompareTag("AllyTank"))
        {
            playerCount++;
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("EnemyRanged"))
        {
            enemyCount++;
        }

        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AllyRanged"))
        {
            playerCount--;
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("EnemyRanged"))
        {
            enemyCount--;
        }
    }

    private void Update()
    {
        if (numBuildings == null) return; // Ensure numBuildings is not null

        if (playerCount > 0 && enemyCount == 0 && numBuildings.numBuildings == 0)
        {
            if (capturingSide == null || capturingSide == "Player")
            {
                if (flashCoroutine == null)
                {
                    flashCoroutine = StartCoroutine(FlashColor(Color.cyan));
                }

                captureProgress += Time.deltaTime;
                if (captureProgress >= captureTime)
                {
                    Capture("Player");
                }
            }
            else
            {
                captureProgress -= Time.deltaTime;
                if (captureProgress <= 0)
                {
                    capturingSide = "Player";
                    captureProgress = 0;
                }
            }
        }
        else if (enemyCount > 0 && playerCount == 0 && numBuildings.numBuildings == 0)
        {
            if (capturingSide == null || capturingSide == "Enemy")
            {
                if (flashCoroutine == null)
                {
                    flashCoroutine = StartCoroutine(FlashColor(Color.red));
                }

                captureProgress += Time.deltaTime;
                if (captureProgress >= captureTime)
                {
                    Capture("Enemy");
                }
            }
            else
            {
                captureProgress -= Time.deltaTime;
                if (captureProgress <= 0)
                {
                    capturingSide = "Enemy";
                    captureProgress = 0;
                }
            }
        }
        
    }

    private void Capture(string side)
    {
        captureProgress = captureTime; // Ensure capture progress is complete
        capturingSide = side;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (side == "Player")
        {
            spriteRenderer.color = Color.cyan;
            if (!captured)
            {
                tileController.tilesCaptured++;
            }
            captured = true;
        }
        else if (side == "Enemy")
        {
            spriteRenderer.color = Color.red;
            if (captured)
            {
                tileController.tilesCaptured--;
            }
            captured = true;
        }
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
