using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxonChecker : MonoBehaviour
{
    public GameObject objectToActivate; // The GameObject to activate when all axons are dead

    public bool allAxonsDead; // Whether all axons are dead
    void Start()
    {
        
    }

    void Update()
    {
        // Find all GameObjects with the tag "Axon"
        GameObject[] axons = GameObject.FindGameObjectsWithTag("Axon");

        // Assume all axons are dead until proven otherwise
        allAxonsDead = true;

        // Loop through each axon and check if it's dead
        foreach (GameObject axon in axons)
        {
            // Get the Axon script attached to the axon GameObject
            Axon axonScript = axon.GetComponent<Axon>();

            if (axonScript != null)
            {
                // Check if the axon is dead
                if (axonScript.dead)
                {
                    
                }
                else
                {
                    
                    allAxonsDead = false; // Found an axon that is alive, so not all are dead
                }
            }
            else
            {
                
            }
        }

        if (allAxonsDead)
        {
            Debug.Log("All axons are dead, activating lose screen.");
            objectToActivate.SetActive(true);
        }
    }
}
