using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllySpawner : MonoBehaviour
{
    public GameObject basicAllyTroopPrefab; // Reference to the prefab of the ally troop to spawn
    public GameObject rangedAllyTroopPrefab;
    public float spawnRadius = 5f; // Radius within which the ally troops will be spawned

    // Function to spawn an ally troop at a random position within the spawn radius
    public void SpawnBasicAllyTroop()
    {
        // Calculate a random position within the spawn radius
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

        // Spawn the ally troop at the calculated position
        Instantiate(basicAllyTroopPrefab, spawnPosition, Quaternion.identity);
    }

    public void SpawnRangedAllyTroop()
    {
        // Calculate a random position within the spawn radius
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

        // Spawn the ally troop at the calculated position
        Instantiate(rangedAllyTroopPrefab, spawnPosition, Quaternion.identity);
    }


}
