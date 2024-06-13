using UnityEngine;
using System.Collections;

public class CaptureZone : MonoBehaviour
{
    public float captureTime = 5.0f; // Time required to capture the platform
    public float uncaptureTime = 20.0f; // Time required to fully uncapture the platform

    private float captureProgress = 0;
    private string capturingSide = null;

    private int playerCount = 0;
    private int enemyCount = 0;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;
    private Coroutine uncaptureCoroutine;

    private MemoryTile memoryTile; // Reference to the memory tile associated with this capture zone

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        memoryTile = GetComponentInParent<MemoryTile>(); // Assuming the CaptureZone is a child of the MemoryTile

        if (memoryTile == null)
        {
            Debug.LogError("MemoryTile component not found in parent of " + gameObject.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AllyRanged"))
        {
            playerCount++;
        }
        else if (other.CompareTag("Enemy"))
        {
            enemyCount++;
        }

        if (uncaptureCoroutine != null)
        {
            StopCoroutine(uncaptureCoroutine);
            uncaptureCoroutine = null;
        }

        if (playerCount > 0 && enemyCount > 0)
        {
            ResetFlashColor();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AllyRanged"))
        {
            playerCount--;
        }
        else if (other.CompareTag("Enemy"))
        {
            enemyCount--;
        }
    }

    private void Update()
    {
        if (playerCount > 0 && enemyCount == 0)
        {
            if (capturingSide == null || capturingSide == "Player")
            {
                if (flashCoroutine == null)
                {
                    flashCoroutine = StartCoroutine(FlashColor(Color.blue));
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
        else if (enemyCount > 0 && playerCount == 0)
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
        else if (playerCount > 0 && enemyCount > 0)
        {
            // Zone is contested
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
            spriteRenderer.color = Color.blue;
        }
        else if (side == "Enemy")
        {
            spriteRenderer.color = Color.red;
            DestroyBuildingOnTile(); // Destroy the building on this memory tile
        }
    }

    private void ResetFlashColor()
    {
        captureProgress = 0;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        spriteRenderer.color = Color.white; // Reset to default color when contested or neutral
    }

    private IEnumerator UncaptureZone()
    {
        float uncaptureProgress = 0;
        Color originalColor = spriteRenderer.color;

        while (uncaptureProgress < uncaptureTime)
        {
            uncaptureProgress += Time.deltaTime;
            float t = uncaptureProgress / uncaptureTime;
            spriteRenderer.color = Color.Lerp(originalColor, Color.white, t);
            yield return null;
        }

        capturingSide = null;
        captureProgress = 0;
        spriteRenderer.color = Color.white;
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
        if (memoryTile != null && memoryTile.building != null)
        {
            Destroy(memoryTile.building);
            memoryTile.building = null;
            Debug.Log("Destroyed building on memory tile: " + memoryTile.name);
        }
    }
}
