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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log("CaptureZone script initialized. SpriteRenderer found: " + (spriteRenderer != null));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            playerCount++;
            Debug.Log("Player entered the zone. Player count: " + playerCount);
        }
        else if (other.CompareTag("Enemy"))
        {
            enemyCount++;
            Debug.Log("Enemy entered the zone. Enemy count: " + enemyCount);
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
        Debug.Log("Trigger exited by: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            playerCount--;
            Debug.Log("Player left the zone. Player count: " + playerCount);
        }
        else if (other.CompareTag("Enemy"))
        {
            enemyCount--;
            Debug.Log("Enemy left the zone. Enemy count: " + enemyCount);
        }

        if (playerCount == 0 && enemyCount == 0 && capturingSide != null)
        {
            uncaptureCoroutine = StartCoroutine(UncaptureZone());
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
                Debug.Log("Player capturing the zone. Progress: " + captureProgress);
                if (captureProgress >= captureTime)
                {
                    Capture("Player");
                }
            }
            else
            {
                captureProgress -= Time.deltaTime;
                Debug.Log("Player reducing enemy capture. Progress: " + captureProgress);
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
                Debug.Log("Enemy capturing the zone. Progress: " + captureProgress);
                if (captureProgress >= captureTime)
                {
                    Capture("Enemy");
                }
            }
            else
            {
                captureProgress -= Time.deltaTime;
                Debug.Log("Enemy reducing player capture. Progress: " + captureProgress);
                if (captureProgress <= 0)
                {
                    capturingSide = "Enemy";
                    captureProgress = 0;
                }
            }
        }
        else if (playerCount > 0 && enemyCount > 0)
        {
            Debug.Log("Zone is contested.");
            ResetFlashColor();
        }
    }

    private void Capture(string side)
    {
        Debug.Log($"{side} has captured the platform!");
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
        }

        // Add additional logic for what happens when the platform is captured
    }

    private void ResetFlashColor()
    {
        Debug.Log("Resetting flash color.");
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
        Debug.Log("Zone has become uncaptured.");
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
}
