using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RepairTroop : MonoBehaviour
{
    public float healAmount = 5f; // Amount of health to heal each time
    public float healInterval = 2f; // Time between each heal

    private NavMeshAgent agent;
    private Axon targetAxon;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(RepairBehavior());

       

        if (agent != null)
        {
            agent.updateUpAxis = false;
            agent.updateRotation = false;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError("EnemyTroop " + gameObject.name + " is not on a NavMesh!");
            }
        }
    }

    IEnumerator RepairBehavior()
    {
        while (true)
        {
            // Find the nearest Axon
            FindNearestAxon();

            // If a target axon is found, move towards it
            if (targetAxon != null)
            {
                agent.SetDestination(targetAxon.transform.position);

                // Check if the repair troop is within healing range
                if (Vector3.Distance(transform.position, targetAxon.transform.position) <= agent.stoppingDistance)
                {

                    Debug.Log("STOP");
                    // Heal the axon if its health is less than max health
                    if (targetAxon.currentHealth < targetAxon.maxHealth)
                    {
                        targetAxon.Heal(healAmount);
                        
                    }

                    // Wait for the next heal interval
                    yield return new WaitForSeconds(healInterval);
                }
                else
                {
                    // If not in range, keep moving toward the axon
                    yield return null;
                }
            }
            else
            {
                // If no axon found, wait a bit and try again
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void FindNearestAxon()
    {
        GameObject[] axons = GameObject.FindGameObjectsWithTag("Axon");

        if (axons.Length == 0)
        {
            targetAxon = null;
            return;
        }

        float minDistance = Mathf.Infinity;
        Axon nearestAxon = null;

        foreach (GameObject axon in axons)
        {
            float distance = Vector3.Distance(transform.position, axon.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestAxon = axon.GetComponent<Axon>();
            }
        }

        targetAxon = nearestAxon;
    }
}
