using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnRepairTroop : MonoBehaviour
{
    public GameObject repairTroopPrefab; // Assign the prefab of the repair troop
    public float initialSpawnInterval = 30f; // Interval between initial spawns
    public float respawnDelay = 40f; // Delay before respawning a dead repair troop
    public int maxRepairTroops = 1; // Maximum number of repair troops allowed at a time (changed to 1)
    public float spawnRadius = 2f; // Radius around the axon to spawn the repair troops

    public List<GameObject> activeRepairTroops = new List<GameObject>(); // List to keep track of active repair troops
    private bool isSpawning = false;

    void Start()
    {
        // Start initial spawning of repair troops
        StartCoroutine(InitialSpawnRepairTroops());
    }

    void Update()
    {
        // Continuously check the status of active repair troops
        activeRepairTroops.RemoveAll(troop => troop == null);

        // If less than the maximum number of repair troops and not currently spawning, trigger respawn
        if (activeRepairTroops.Count < maxRepairTroops && !isSpawning)
        {
            StartCoroutine(HandleTroopRespawn());
        }
    }

    IEnumerator InitialSpawnRepairTroops()
    {
        // Spawn one repair troop at the start
        yield return new WaitForSeconds(initialSpawnInterval);
        SpawnRepairTroopp();
    }

    IEnumerator HandleTroopRespawn()
    {
        isSpawning = true;

        // Wait for the respawn delay (e.g., 40 seconds) after detecting a missing troop
        yield return new WaitForSeconds(respawnDelay);

        // Spawn a new repair troop if needed
        if (activeRepairTroops.Count < maxRepairTroops)
        {
            SpawnRepairTroopp();
        }

        isSpawning = false;
    }

    void SpawnRepairTroopp()
    {
        Vector3 randomOffset = Random.insideUnitCircle * spawnRadius; // Random position within the radius
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        GameObject repairTroop = Instantiate(repairTroopPrefab, spawnPosition, Quaternion.identity);

        // Disable NavMeshAgent temporarily
        NavMeshAgent agent = repairTroop.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;  // Disable the agent
        }

        // Set the correct rotation
        repairTroop.transform.rotation = Quaternion.identity;

        // Re-enable the NavMeshAgent
        if (agent != null)
        {
            agent.enabled = true;  // Enable the agent again
        }

        // Add the newly spawned repair troop to the list
        activeRepairTroops.Add(repairTroop);
    }
}
