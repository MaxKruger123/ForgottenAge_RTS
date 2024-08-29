using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnRepairTroop : MonoBehaviour
{
    public GameObject repairTroopPrefab; // Assign the prefab of the repair troop
    public float spawnInterval = 2f; // Interval between spawns
    public int maxRepairTroops = 2; // Maximum number of repair troops allowed at a time
    public float spawnRadius = 2f; // Radius around the axon to spawn the repair troops

    private int currentRepairTroops = 0;

    private NavMeshAgent agent;

    void Start()
    {
        StartCoroutine(SpawnRepairTroops());
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    IEnumerator SpawnRepairTroops()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentRepairTroops < maxRepairTroops)
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

                currentRepairTroops++;
            }
        }
    }
}
