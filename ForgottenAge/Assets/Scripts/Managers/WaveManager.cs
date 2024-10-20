using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public List<GameObject> enemyPrefabs; // List of enemy prefabs for this wave
    public int enemiesToSpawn; // Number of enemies to spawn in this wave
}

public class WaveManager : MonoBehaviour
{
    public TMP_Text timerText; // Reference to the TextMeshPro text element displaying the timer
    public float waveDuration = 20f; // Duration of each wave
    public Transform[] spawnPoints; // Array of spawn points for enemies
    public List<Wave> waves; // List of waves
    public int currentWave = 0; // Current wave index
    private bool waveInProgress = false; // Flag to track if a wave is currently in progress

    public int cardWaveCounter; // int to track how many waves until the next card icon appears
    public CardScreen cardScreen; // reference to the card screen for card events
    public int wavesUntilCardEvent;
    public GameObject enemySpawnEffect;

    public int[] cutsceneWaves; // Waves at which cutscenes are played
    private CutsceneManager cutsceneManager; // Reference to the CutsceneManager

    private bool paused = false; // Flag to pause the countdown timer

    public float replacementChance = 15f; // Percentage chance for ally troop replacement after wave 10
    public GameObject enemyTroopPrefab; // Prefab for the enemy troop to replace allies
    public List<GameObject> enemyPrefabs;

    void Start()
    {
        cutsceneManager = FindObjectOfType<CutsceneManager>();
        StartCoroutine(StartWaveTimer());
    }

    IEnumerator StartWaveTimer()
    {
        float timer = waveDuration;
        while (timer > 0)
        {
            if (!paused)
            {
                timer -= Time.deltaTime;
                timerText.text = "Next Wave: " + Mathf.Ceil(timer).ToString();
            }
            yield return null;
        }

        // Start the next wave
        StartNextWave();
    }

    void StartWaveTimerCoroutine()
    {
        StartCoroutine(StartWaveTimer());
    }

    void StartNextWave()
    {
        if (currentWave < waves.Count)
        {
            currentWave++;
            cardWaveCounter++;
            timerText.text = "Wave " + currentWave.ToString();

            if (currentWave >= 10)
            {
                ReplaceAllyTroopsWithEnemies();
            }
            if (currentWave >= 15)
            {
                replacementChance = 15f;
                ReplaceAllyTroopsWithEnemies();
            }
            if (currentWave >= 20)
            {
                replacementChance = 20f;
                ReplaceAllyTroopsWithEnemies();
            }
            if (currentWave >= 25)
            {
                replacementChance = 30f;
                ReplaceAllyTroopsWithEnemies();
            }
            if (currentWave >= 25)
            {
                replacementChance = 50f;
                ReplaceAllyTroopsWithEnemies();
            }
            if (currentWave >= 30)
            {
                replacementChance = 80f;
                ReplaceAllyTroopsWithEnemies();
            }

            StartCoroutine(SpawnEnemies());
        }
        else
        {
            timerText.text = "All Waves Completed!";
        }
    }

    public IEnumerator SpawnEnemies()
    {
        waveInProgress = true;
        // Get the current wave
        Wave wave = waves[currentWave - 1];

        int block = (currentWave ) / 10;
        int increment = 5 * (int)Mathf.Pow(2, block);
        int baseEnemies = 5 * 10 * block;

        int enemiesToSpawn = baseEnemies + ((currentWave) - (block * 10)) * increment;
    

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Choose a random enemy prefab from the list
            // GameObject enemyPrefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Count)];
            GameObject enemyPrefab = null;
            if (currentWave <= 2)
            {
                enemyPrefab = enemyPrefabs[0];
            }else if (currentWave <= 4)
            {
                enemyPrefab = enemyPrefabs[Random.Range(0, 2)];
            }else if (currentWave <= 5)
            {
                enemyPrefab = enemyPrefabs[Random.Range(0, 3)];
            }else if (currentWave >= 6 )
            {
                enemyPrefab = enemyPrefabs[Random.Range(0, 4)];
            }

            // Choose a spawn point based on the index
            int spawnPointIndex = i % spawnPoints.Length;
            Vector3 spawnPosition = spawnPoints[spawnPointIndex].position;

            // Instantiate enemy at the chosen spawn point
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            Instantiate(enemySpawnEffect, new Vector3(spawnPosition.x, spawnPosition.y, -1), Quaternion.identity);

            yield return new WaitForSeconds(0.1f); // Adjust this delay as needed
        }
        


        // Check if all enemies are dead
        while (GameObject.FindGameObjectWithTag("Enemy") != null || GameObject.FindGameObjectWithTag("EnemyRanged") != null)
        {
            yield return null;
        }

        // End of wave, start countdown for next wave
        waveInProgress = false;
        StartWaveTimerCoroutine();

        /*if (cardWaveCounter == wavesUntilCardEvent)
        {
            cardWaveCounter = 0;
            cardScreen.ShowIconButton();
        }
        else
        {
            StartWaveTimerCoroutine();
        }*/
        //CheckForCutsceneOrNextWave();
    }

    // Function to replace ally troops with enemy troops
    void ReplaceAllyTroopsWithEnemies()
    {
        // Find all ally troops
        List<GameObject> allyTroops = new List<GameObject>();
        allyTroops.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        allyTroops.AddRange(GameObject.FindGameObjectsWithTag("AllyRanged"));
        allyTroops.AddRange(GameObject.FindGameObjectsWithTag("AllyTank"));

        foreach (GameObject ally in allyTroops)
        {
            if (Random.Range(0f, 100f) <= replacementChance)
            {
                Vector3 position = ally.transform.position;
                Destroy(ally);
                Instantiate(enemyTroopPrefab, position, Quaternion.identity);
            }
        }
    }

    // Public function so that a wave can be started after closing the card screen
    public void StartWave()
    {
        StartCoroutine(StartWaveTimer());
    }

    public void SetCurrentWave(int num)
    {
        currentWave = num;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    void CheckForCutsceneOrNextWave()
    {
        // Check if the current wave is a cutscene wave
        if (System.Array.Exists(cutsceneWaves, wave => wave == currentWave))
        {
            int cutsceneIndex = System.Array.IndexOf(cutsceneWaves, currentWave);
            Debug.Log("Cutscene wave detected: " + currentWave);
            Debug.Log("Calling PlayCutscene on CutsceneManager");
            paused = true; // Pause the countdown timer
            cutsceneManager.PlayCutscene(cutsceneIndex, () =>
            {
                paused = false; // Resume the countdown timer
                StartWaveTimerCoroutine();
            });
        }
        else
        {
            // Check if the player should receive a card event
            if (cardWaveCounter == wavesUntilCardEvent) 
            {
                cardWaveCounter = 0;
                cardScreen.ShowIconButton();
            }
            else
            {
                StartWaveTimerCoroutine();
            }
        }
    }
}
