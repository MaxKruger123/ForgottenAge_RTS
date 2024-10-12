using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxonBehaviour : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Vector3 targetPosition;  // The position of the target neuron's dendrite (in world space)
    public float growthSpeed = 2f;  // Speed of axon growth
    public bool hasTarget;
    public bool isRetracting = false;  // Indicates if the axon is retracting
    public float cooldownTime = 5f;  // Time before the axon can search for a new target
    private float cooldownTimer = 0f;

    public NeuronBehaviour neuronBehaviour;

    private List<Vector3> positions = new List<Vector3>();  // List to store positions dynamically
    private Vector3 controlPoint1;  // First control point for the Bezier curve
    private Vector3 controlPoint2;  // Second control point for the Bezier curve

    private int maxPoints = 500;  // Maximum number of points the axon will have
    private float segmentLength = 0.25f;  // Minimum distance between points before a new one is added
    private float t = 0;  // Progress along the Bezier curve

    public GameObject targetNeuron;  // Store the reference to the target neuron
    public GameObject targetDendrite;  // Store the reference to the target neuron
    public bool die=false;

    public Axon axon;

    void Start()
    {
        
        lineRenderer = GetComponent<LineRenderer>();
        growthSpeed = Random.Range(0.5f, 2f);
        // Initialize the LineRenderer with 2 points
        positions.Add(neuronBehaviour.transform.position);  // Start at the neuron's position
        positions.Add(neuronBehaviour.transform.position);  // Initial end point (same as start)
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());

        // Initialize control points for Bezier curve
        InitializeControlPoints();
    }

    void Update()
    {
        if (die)
        {
            Die();
            die = false;
        }
        
        if (isRetracting)
        {
            
            RetractAxon();
        }
        else if (hasTarget)
        {
            GrowAxon();
        }
        else if (neuronBehaviour.InitialisationCheck() && gameObject.tag == "Axon")  // Wait for dendrites to be initialized 
        {
            SearchForNewTarget();
        }
        
    }

    // Handles the axon growing towards its target
    private void GrowAxon()
    {
        // Increase t (progress along the Bezier curve)
        t += (growthSpeed * Time.deltaTime) / Vector3.Distance(transform.position, targetPosition);
        t = Mathf.Clamp01(t);  // Ensure t stays between 0 and 1

        // Move the current last point along the Bezier curve
        Vector3 bezierPosition = CalculateBezierPosition(t);
        positions[positions.Count - 1] = bezierPosition;

        // If the last point is far enough from the second-to-last point, add a new point
        if (positions.Count < maxPoints && Vector3.Distance(positions[positions.Count - 1], positions[positions.Count - 2]) > segmentLength)
        {
            if (Vector3.Distance(targetPosition, positions[positions.Count - 1]) < segmentLength)
            {
                //Debug.Log("Close enough, no need for a new point.");
            }
            else
            {
                AddNewPoint();
            }
        }

        // Update the LineRenderer
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());

        // Check if the axon has fully reached the target
        if (Vector3.Distance(bezierPosition, targetPosition) < 0.1f)
        {
            //Debug.Log("Axon reached target neuron!");
        }
    }

    // Handles the axon retracting back to its neuron
    private void RetractAxon()
    {
        // Retract the axon by moving points back toward the neuron
        for (int i = positions.Count - 1; i >= 1; i--)
        {
            positions[i] = Vector3.MoveTowards(positions[i], neuronBehaviour.transform.position, growthSpeed * Time.deltaTime);

            // Remove points that are close enough to the neuron
            if (Vector3.Distance(positions[i], neuronBehaviour.transform.position) < 0.1f)
            {
                positions.RemoveAt(i);  // Remove the point once it reaches the neuron
            }
        }

        // Update the LineRenderer to reflect the new list of positions
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());

        // Check if the axon has fully retracted (only the initial point remains)
        if (positions.Count <= 2 && Vector3.Distance(positions[positions.Count - 1], neuronBehaviour.transform.position) < 0.1f)
        {
            Debug.Log("Axon fully retracted.");
            isRetracting = false;
            //cooldownTimer = cooldownTime;  // Start the cooldown timer
            ResetAxon();  // Reset for future connections
        }
    }

    // Searches for a new neuron and sets it as the target
    private void SearchForNewTarget()
    {
        targetNeuron = neuronBehaviour.SearchForNeuron();  // Find the closest neuron within a certain distance

        if (targetNeuron != null)
        {
            NeuronBehaviour targetNeuronBehaviour = targetNeuron.GetComponent<NeuronBehaviour>();
            List<GameObject> targetDendrites = targetNeuronBehaviour.GetDendrites();
            float shortestDistance = Mathf.Infinity;
            GameObject nearestDendrite = null;

            foreach (GameObject dendrite in targetDendrites)
            {
                if (!targetNeuronBehaviour.connectedDendrites.Contains(dendrite))
                {
                    LineRenderer dendriteLineRenderer = dendrite.GetComponent<LineRenderer>();
                    Vector3 dendriteWorldPosition = dendriteLineRenderer.GetPosition(3);  // In world space

                    float distanceToDendrite = Vector3.Distance(transform.position, dendriteWorldPosition);
                    if (distanceToDendrite < shortestDistance)
                    {
                        shortestDistance = distanceToDendrite;
                        nearestDendrite = dendrite;
                    }
                }
            }

            if (nearestDendrite != null)
            {
                LineRenderer nearestLineRenderer = nearestDendrite.GetComponent<LineRenderer>();

                if (nearestLineRenderer != null && nearestLineRenderer.positionCount > 3)
                {
                    targetPosition = nearestLineRenderer.GetPosition(3);
                    hasTarget = true;

                    // Mark both neurons as connected to each other to avoid redundant connections
                    neuronBehaviour.AddConnectedNeuron(targetNeuron);
                    targetNeuron.GetComponent<NeuronBehaviour>().AddConnectedNeuron(neuronBehaviour.gameObject);

                    // Mark this dendrite as connected
                    targetNeuronBehaviour.AddConnectedDendrite(nearestDendrite);
                    targetDendrite = nearestDendrite;

                    // Reinitialize the control points for the Bezier curve
                    InitializeControlPoints();
                }
            }
            
        }
    }

    // Adds a new point to the axon as it grows
    private void AddNewPoint()
    {
        Vector3 previousPosition = positions[positions.Count - 1];  // Last point's position
        positions.Insert(positions.Count - 1, previousPosition);  // Insert a new point before the last one

        // Update the LineRenderer
        lineRenderer.positionCount = positions.Count;
    }

    // Initializes the control points for the Bezier curve with some random offsets
    private void InitializeControlPoints()
    {
        Vector3 startPosition = transform.position;
        float deviation = 0.5f;
        controlPoint1 = startPosition + new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), 0) * (Vector3.Distance(startPosition, targetPosition));
        controlPoint2 = targetPosition + new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), 0) * (Vector3.Distance(startPosition, targetPosition) / 5);
    }

    // Calculate the position along a cubic Bezier curve
    private Vector3 CalculateBezierPosition(float t)
    {
        Vector3 start = transform.position;
        Vector3 p1 = Vector3.Lerp(start, controlPoint1, t);
        Vector3 p2 = Vector3.Lerp(controlPoint1, controlPoint2, t);
        Vector3 p3 = Vector3.Lerp(controlPoint2, targetPosition, t);

        Vector3 p4 = Vector3.Lerp(p1, p2, t);
        Vector3 bezierPosition = Vector3.Lerp(p4, p3, t);

        return bezierPosition;
    }

    // Resets the axon for a new connection
    private void ResetAxon()
    {
        hasTarget = false;
        t = 0;
        positions.Clear();

        // Add the neuron's position twice (for start and end) to reset the axon
        Vector3 neuronPosition = neuronBehaviour.transform.position;
        positions.Add(neuronPosition);  // First point (start)
        positions.Add(neuronPosition);  // Second point (end)

        // Update the LineRenderer to reflect the reset state
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        growthSpeed = Random.Range(0.5f, 2f);
    }

    // Public method to trigger axon death and start retraction
    public void Die()
    {
        Debug.Log("Axon is dying and retracting.");
        isRetracting = true;
        hasTarget = false;

        // Disconnect the target neuron and remove it from the connected neurons list
        if (targetNeuron != null)
        {
            targetNeuron.GetComponent<NeuronBehaviour>().RemoveConnectedDendrite(targetDendrite);
            targetDendrite = null;
            neuronBehaviour.RemoveConnectedNeuron(targetNeuron);
            targetNeuron.GetComponent<NeuronBehaviour>().RemoveConnectedNeuron(neuronBehaviour.gameObject);
            targetNeuron = null;  // Clear reference after disconnection
        }
    }


}
