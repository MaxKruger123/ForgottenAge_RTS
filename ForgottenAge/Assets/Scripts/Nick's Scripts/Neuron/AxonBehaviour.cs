using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxonBehaviour : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Vector2 targetPosition;  // The position of the target neuron (in world space)
    public float growthSpeed = 2f;  // Speed of axon growth
    public bool hasTarget;

    public NeuronBehaviour neuronBehaviour;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Set initial start and end points of the line (world space)
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);  // Start at the axon's world position (neuron body center)
        lineRenderer.SetPosition(1, transform.position);  // Initial end point at the same position (no growth yet)
    }

    void Update()
    {
        if (hasTarget)
        {
            // Smoothly move the end point toward the target position (world space)
            Vector3 currentEndPosition = lineRenderer.GetPosition(1);
            Vector3 newEndPosition = Vector3.MoveTowards(currentEndPosition, targetPosition, growthSpeed * Time.deltaTime);

            // Update the end position of the axon in the LineRenderer (world space)
            lineRenderer.SetPosition(1, newEndPosition);

            // Optional: Check if the axon has reached the target position
            if (Vector3.Distance(newEndPosition, targetPosition) < 0.1f)
            {
                
                // Trigger any synaptic event or connection effect here
            }
        }
        else if (neuronBehaviour.InitialisationCheck())  // Wait for dendrites to be initialized
        {
            GameObject targetNeuron = neuronBehaviour.SearchForNeuron();  // Find the closest neuron within a certain distance

            if (targetNeuron != null)
            {
                List<GameObject> targetDendrites = targetNeuron.GetComponent<NeuronBehaviour>().GetDendrites();
                float shortestDistance = Mathf.Infinity;
                GameObject nearestDendrite = null;

                foreach (GameObject dendrite in targetDendrites)
                {
                    // Get the world position of the 4th point of the dendrite's LineRenderer
                    LineRenderer dendriteLineRenderer = dendrite.GetComponent<LineRenderer>();
                    Vector3 dendriteWorldPosition = dendriteLineRenderer.GetPosition(3);  // In world space

                    float distanceToDendrite = Vector3.Distance(transform.position, dendriteWorldPosition);
                    if (distanceToDendrite < shortestDistance)
                    {
                        shortestDistance = distanceToDendrite;
                        nearestDendrite = dendrite;
                    }
                }

                // Check if a nearest dendrite was found and its LineRenderer has enough positions
                if (nearestDendrite != null)
                {
                    LineRenderer nearestLineRenderer = nearestDendrite.GetComponent<LineRenderer>();

                    if (nearestLineRenderer != null && nearestLineRenderer.positionCount > 3)
                    {
                        // Get the 4th point in world space and set it as the target position
                        targetPosition = nearestLineRenderer.GetPosition(3);
                        hasTarget = true;

                        // Mark both neurons as connected to each other to avoid redundant connections
                        neuronBehaviour.AddConnectedNeuron(targetNeuron);
                        targetNeuron.GetComponent<NeuronBehaviour>().AddConnectedNeuron(neuronBehaviour.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("Nearest dendrite's LineRenderer has fewer than 4 positions.");
                    }
                }
                else
                {
                    Debug.LogWarning("No nearest dendrite found.");
                }
            }
        }
    }
}
