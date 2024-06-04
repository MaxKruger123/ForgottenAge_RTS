using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public TMP_Text timerText; // Reference to the TextMeshPro text element displaying the timer
    public float waveDuration = 20f; // Duration of each wave
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public Transform[] spawnPoints; // Array of spawn points for enemies
    public int[] enemiesPerWave; // Number of enemies to spawn in each wave
    private int currentWave = 0; // Current wave index
    private bool waveInProgress = false; // Flag to track if a wave is currently in progress

    void Start()
    {
        StartCoroutine(StartWaveTimer());
    }

    IEnumerator StartWaveTimer()
    {
        // Start countdown timer before the first wave
        float timer = waveDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = "Next Wave: " + Mathf.Ceil(timer).ToString();
            yield return null;
        }

        // Start the next wave
        StartNextWave();
    }

    void StartNextWave()
    {
        currentWave++;
        timerText.text = "Wave " + currentWave.ToString();

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        waveInProgress = true;

        // Spawn enemies for the current wave
        int enemiesToSpawn = enemiesPerWave[currentWave - 1];
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Choose a spawn point based on the index
            int spawnPointIndex = i % spawnPoints.Length;
            Vector3 spawnPosition = spawnPoints[spawnPointIndex].position;

            // Instantiate enemy at the chosen spawn point
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(1f); // Adjust this delay as needed
        }

        // Check if all enemies are dead
        while (GameObject.FindWithTag("Enemy") != null)
        {
            yield return null;
        }

        // End of wave, start countdown for next wave
        waveInProgress = false;
        StartCoroutine(StartWaveTimer());
    }
}
