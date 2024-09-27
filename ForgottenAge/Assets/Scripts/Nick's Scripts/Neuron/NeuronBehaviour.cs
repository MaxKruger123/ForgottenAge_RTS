using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronBehaviour : MonoBehaviour
{
    public GameObject dendritePrefab;
    public List<GameObject> dendrites;
    public GameObject Axon;

    public List<GameObject> connectedNeurons;
    public bool reinitialise = false;
    public bool initialised = false;
    // Start is called before the first frame update
    void Start()
    {
        
        // Creates dendrite arms so that every neuron will have 3-5 dendrites
        int numDendrites = Random.Range(3, 6);
        for (int i = 0; i < numDendrites; i++)
        {
            GameObject newdendrite = Instantiate(dendritePrefab, transform.position, Quaternion.identity);
            newdendrite.transform.parent = transform.GetChild(0); // parents new dendrite to neuron body
            dendrites.Add(newdendrite);
        }
            
        InitialiseDendrites();
    }

    // Update is called once per frame
    void Update()
    {
        if (reinitialise)
        {
            InitialiseDendrites();
            reinitialise = false;
        }
        
    }

    // Only search for neurons that are not already connected to this neuron
    public GameObject SearchForNeuron()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestNeuron = null;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("MemoryTile") && collider.gameObject != this.gameObject)
            {
                NeuronBehaviour potentialNeuron = collider.GetComponent<NeuronBehaviour>();

                // Check if the potential neuron is already connected
                if (potentialNeuron != null && !connectedNeurons.Contains(potentialNeuron.gameObject) && !potentialNeuron.connectedNeurons.Contains(this.gameObject))
                {
                    float distanceToNeuron = Vector3.Distance(transform.position, collider.transform.position);
                    if (distanceToNeuron < shortestDistance)
                    {
                        shortestDistance = distanceToNeuron;
                        nearestNeuron = collider.gameObject;
                    }
                }
            }
        }
        return nearestNeuron;
    }

    public void InitialiseDendrites()
    {
        foreach (GameObject dendrite in dendrites)
        {
            // Random gradient and fixed length for each dendrite
            float gradient = Random.Range(-2f, 2f);  // Use floats and adjust range for more diversity
            float length = Random.Range(1f, 2f);  // Fixed length for the dendrite

            // Randomly flip direction along X and Y axes
            int flipX = Random.Range(0, 2) * 2 - 1; // Randomly either 1 or -1
            int flipY = Random.Range(0, 2) * 2 - 1; // Randomly either 1 or -1

            // Access the LineRenderer component
            LineRenderer lineRenderer = dendrite.GetComponent<LineRenderer>();

            // Ensure the LineRenderer has enough positions for 4 points
            lineRenderer.positionCount = 4;

            // Calculate deltaX and deltaY using the gradient and length
            float deltaX = flipX * (length / Mathf.Sqrt(1 + (gradient * gradient)));  // Change in X with possible flip
            float deltaY = flipY * (gradient * deltaX);  // Change in Y based on the gradient and possible flip

            // Get the parent's position (the neuron) in world space
            Vector3 neuronPosition = transform.position;

            // Define the number of points along the line
            int numPoints = 4;

            // Loop through and set each point along the line
            for (int i = 0; i < numPoints; i++)
            {
                // Divide the total deltaX and deltaY by (numPoints - 1) to get step size
                float factor = i / (float)(numPoints - 1);

                // Calculate the x and y coordinates for each point in local space
                float x = factor * deltaX;
                float y = factor * deltaY;

                // Create a local position relative to the neuron
                Vector3 localPosition = new Vector3(x, y, 0);

                // Convert the local position to world space
                Vector3 worldPosition = neuronPosition + transform.TransformVector(localPosition);

                // Add deviation to the second and third points
                if (i == 1 || i == 2)
                {
                    // length divided by 8 (if length is 2f, deviation will be 0.25f)
                    float xDeviation = Random.Range(length / -8f, length / 8f);
                    float yDeviation = Random.Range(length / -8f, length / 8f);

                    // Apply deviation and update the world position
                    Vector3 deviation = new Vector3(xDeviation, yDeviation, 0);
                    worldPosition += deviation;
                }

                // Set the position in the LineRenderer (world space)
                lineRenderer.SetPosition(i, worldPosition);
            }
        }
        initialised = true;
    }


    public List<GameObject> GetDendrites()
    {
        return dendrites;
    }

    public bool InitialisationCheck()
    {
        return initialised;
    }

    public void AddConnectedNeuron(GameObject connectedNeuron)
    {
        if (!connectedNeurons.Contains(connectedNeuron))
        {
            connectedNeurons.Add(connectedNeuron);
        }
    }
}
