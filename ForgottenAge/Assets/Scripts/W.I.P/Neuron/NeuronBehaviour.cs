using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronBehaviour : MonoBehaviour
{
    public GameObject dendritePrefab;
    public GameObject dendriteBranchPrefab;
    public List<GameObject> dendrites;
    public List<GameObject> connectedDendrites = new List<GameObject>();  // List to track connected dendrites
    public GameObject Axon;

    public List<GameObject> connectedNeurons;
    public bool reinitialise = false;
    public bool initialised = false;

    public CaptureZone captureZone;

    public int searchDistance;
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

        if (connectedNeurons.Count == 0)
        {
            captureZone.DestroyNearestBuilding();
        }
        
    }
    // Only search for neurons that are not already connected to this neuron
    public GameObject SearchForNeuron()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchDistance);
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
        List<float> usedGradients = new List<float>();  // Track previously used gradients
        float minAngleDifference = 15f;  // Minimum angular difference between dendrites in degrees

        foreach (GameObject dendrite in dendrites)
        {
            float gradient;
            do
            {
                // Generate a random gradient between -2 and 2 (adjust as needed)
                gradient = Random.Range(-2f, 2f);
            }
            // Convert gradient to angle (in degrees) and check if it's too similar to any previous gradients
            while (!IsGradientAcceptable(gradient, usedGradients, minAngleDifference));

            // Store the used gradient
            usedGradients.Add(gradient);

            // Randomly flip direction along X and Y axes
            int flipX = Random.Range(0, 2) * 2 - 1;
            int flipY = Random.Range(0, 2) * 2 - 1;

            float length = Random.Range(1.25f, 2f);
            LineRenderer lineRenderer = dendrite.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 4;

            // Calculate deltaX and deltaY using the gradient and length
            float deltaX = flipX * (length / Mathf.Sqrt(1 + (gradient * gradient)));
            float deltaY = flipY * (gradient * deltaX);

            Vector3 neuronPosition = transform.position;
            int numPoints = 4;

            for (int i = 0; i < numPoints; i++)
            {
                float factor = i / (float)(numPoints - 1);
                float x = factor * deltaX;
                float y = factor * deltaY;
                Vector3 localPosition = new Vector3(x, y, 0);
                Vector3 worldPosition = neuronPosition + transform.TransformVector(localPosition);

                if (i == 1 || i == 2)
                {
                    float xDeviation = Random.Range(length / -4f, length / 4f);
                    float yDeviation = Random.Range(length / -4f, length / 4f);
                    worldPosition += new Vector3(xDeviation, yDeviation, 0);
                }

                lineRenderer.SetPosition(i, worldPosition);

                // Add branches at this point (optional for first and last points)
                if (i > 0 && i < numPoints - 1)
                {
                    AddDendriteBranch(worldPosition, neuronPosition);
                }
            }
        }
        initialised = true;
    }

    private bool IsGradientAcceptable(float newGradient, List<float> usedGradients, float minAngleDifference)
    {
        // Convert gradient to angle in degrees
        float newAngle = Mathf.Atan(newGradient) * Mathf.Rad2Deg;

        foreach (float usedGradient in usedGradients)
        {
            // Convert used gradient to angle in degrees
            float usedAngle = Mathf.Atan(usedGradient) * Mathf.Rad2Deg;

            // Check if the difference between angles is less than the minimum allowed
            if (Mathf.Abs(newAngle - usedAngle) < minAngleDifference)
            {
                return false;  // Too similar, reject this gradient
            }
        }

        return true;  // Gradient is acceptable
    }



    // Method to add a connected dendrite to the list
    public void AddConnectedDendrite(GameObject dendrite)
    {
        if (!connectedDendrites.Contains(dendrite))
        {
            connectedDendrites.Add(dendrite);
        }
    }

    // Method to remove a connected dendrite
    public void RemoveConnectedDendrite(GameObject dendrite)
    {
        if (connectedDendrites.Contains(dendrite))
        {
            connectedDendrites.Remove(dendrite);
        }
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

    // Method to remove a connected neuron
    public void RemoveConnectedNeuron(GameObject disconnectedNeuron)
    {
        if (connectedNeurons.Contains(disconnectedNeuron))
        {
            connectedNeurons.Remove(disconnectedNeuron);
        }
    }

    private void AddDendriteBranch(Vector3 branchStart, Vector3 parentPosition)
    {
        // Instantiate the dendrite branch prefab
        GameObject branch = Instantiate(dendriteBranchPrefab, branchStart, Quaternion.identity);

        // Parent it to the neuron (or dendrite if desired)
        branch.transform.parent = transform;

        // Access the LineRenderer of the prefab
        LineRenderer branchRenderer = branch.GetComponent<LineRenderer>();

        // Ensure the LineRenderer exists
        if (branchRenderer == null)
        {
            Debug.LogError("Prefab does not have a LineRenderer component!");
            return;
        }

        // Clone the material to ensure it's not shared with other LineRenderers
        branchRenderer.material = new Material(branchRenderer.material);

        // Set the position count to 4 for the branch
        branchRenderer.positionCount = 4;

        // Create a random length and angle for the branch
        float branchLength = Random.Range(0.5f, 1f) * 3;
        float branchAngle = Random.Range(-70f, 70f);

        // Calculate the direction of the branch
        Vector3 direction = (branchStart - parentPosition).normalized;
        Vector3 rotatedDirection = Quaternion.Euler(0, 0, branchAngle) * direction;

        // Calculate the final end point of the branch
        Vector3 branchEnd = branchStart + rotatedDirection * branchLength;

        // Add 4 points for the branch, including random deviations for the middle points
        for (int i = 0; i < 4; i++)
        {
            float factor = i / 3f;  // Divide the length into 4 points
            Vector3 pointPosition = Vector3.Lerp(branchStart, branchEnd, factor);

            // Add deviation to the middle points to make the branch less straight
            if (i == 1 || i == 2)
            {
                float deviationX = Random.Range(branchLength / -10f, branchLength / 10f);
                float deviationY = Random.Range(branchLength / -10f, branchLength / 10f);
                pointPosition += new Vector3(deviationX, deviationY, 0);
            }

            branchRenderer.SetPosition(i, pointPosition);
        }
    }




}
