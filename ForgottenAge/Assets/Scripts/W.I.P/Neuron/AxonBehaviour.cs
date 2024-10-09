using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AxonBehaviour : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Vector3 targetPosition;  // The position of the target neuron (in world space)
    public float growthSpeed = 2f;  // Speed of axon growth
    public bool hasTarget;

    public NeuronBehaviour neuronBehaviour;

    private List<Vector3> positions = new List<Vector3>();  // List to store positions dynamically
    private Vector3 controlPoint1;  // First control point for the Bezier curve
    private Vector3 controlPoint2;  // Second control point for the Bezier curve

    private int maxPoints = 500;  // Maximum number of points the axon will have
    private float segmentLength = 0.25f;  // Minimum distance between points before a new one is added
    private float t = 0;  // Progress along the Bezier curve

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Initialize the LineRenderer with 2 points
        positions.Add(neuronBehaviour.transform.position);  // Start at the neuron's position
        positions.Add(neuronBehaviour.transform.position);  // Initial end point (same as start)
        lineRenderer.positionCount = positions.Count;

        // Initialize control points for Bezier curve
        InitializeControlPoints();
    }

    void Update()
    {
        if (hasTarget)
        {
            
            //lineRenderer.startColor = new Color(255, 237, 112, 255);
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
                    Debug.Log("Close enough dont need new point");
                }
                else
                {
                    AddNewPoint();
                }
                
            }

            // Update the LineRenderer
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());

            // Optional: Check if the axon has fully reached the target
            if (Vector3.Distance(bezierPosition, targetPosition) < 0.1f)
            {
                Debug.Log("Axon reached target neuron!");
            }
        }
        else if (neuronBehaviour.InitialisationCheck())  // Wait for dendrites to be initialized
        {
            GameObject targetNeuron = neuronBehaviour.SearchForNeuron();  // Find the closest neuron within a certain distance

            if (targetNeuron != null)
            {
                NeuronBehaviour targetNeuronBehaviour = targetNeuron.GetComponent<NeuronBehaviour>();
                List<GameObject> targetDendrites = targetNeuronBehaviour.GetDendrites();
                float shortestDistance = Mathf.Infinity;
                GameObject nearestDendrite = null;

                foreach (GameObject dendrite in targetDendrites)
                {
                    // Ensure the dendrite has not already been connected
                    if (!targetNeuronBehaviour.connectedDendrites.Contains(dendrite))
                    {
                        // Get the world position of the 4th point of the dendrite's LineRenderer
                        if (dendrite.GetComponent<LineRenderer>() != null)
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

                        // Mark this dendrite as connected
                        targetNeuronBehaviour.AddConnectedDendrite(nearestDendrite);

                        // Reinitialize the control points for the Bezier curve
                        InitializeControlPoints();
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
        //float deviation = 2;
        // Control points are set with some random offset to create the bending effect
        //controlPoint1 = startPosition + new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), 0) * (Vector3.Distance(startPosition, targetPosition) / 5);
        //controlPoint2 = targetPosition + new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), 0) * (Vector3.Distance(startPosition, targetPosition) / 10);

        float deviation = 0.5f;
        controlPoint1 = startPosition + new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), 0) * (Vector3.Distance(startPosition, targetPosition));
        controlPoint2 = targetPosition + new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), 0) * (Vector3.Distance(startPosition, targetPosition)/5);
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
}
