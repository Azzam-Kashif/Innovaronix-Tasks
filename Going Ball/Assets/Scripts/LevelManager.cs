using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using Unity.Mathematics;

public class LevelController : MonoBehaviour
{
    public GameObject aiPrefab;
    public int numberOfAI;
    public SplineContainer originalSpline;
    public Transform playerSpawnPoint;
    public Transform[] aiSpawnPoints; // Predefined spawn points
    public GameObject player;

    public bool usePredefinedSpawnPoints = true; // Toggle for predefined or duplicate spline-based spawn

    // Platform bounds for keeping the splines inside
    public Vector3 platformMinBounds = new Vector3(-10f, 0, -10f);
    public Vector3 platformMaxBounds = new Vector3(10f, 0, 10f);

    void Start()
    {
        if (player != null)
        {
            player.transform.position = playerSpawnPoint.position;
            player.GetComponent<MovementWithSpline>().spline = originalSpline;
        }
        else
        {
            Debug.LogError("Player object is not assigned.");
        }

        // Spawn AI-controlled balls
        StartCoroutine(SpawnAIBalls());
    }

    IEnumerator SpawnAIBalls()
    {
        if (usePredefinedSpawnPoints)
        {
            // Spawn using predefined points
            SpawnFromPredefinedPoints();
        }
        else
        {
            // Duplicate the spline and spawn AI
            DuplicateSplineAndSpawnAI();
        }
        yield return null;
    }

    void SpawnFromPredefinedPoints()
    {
        if (aiSpawnPoints == null || aiSpawnPoints.Length == 0)
        {
            Debug.LogError("No AI spawn points assigned in the inspector.");
            return;
        }

        int spawnPointsAvailable = Mathf.Min(numberOfAI, aiSpawnPoints.Length);

        for (int i = 0; i < spawnPointsAvailable; i++)
        {
            if (aiSpawnPoints[i] != null)
            {
                Vector3 spawnPosition = aiSpawnPoints[i].position;
                InstantiateAIWithNewSpline(spawnPosition);
            }
            else
            {
                Debug.LogWarning("Spawn point " + i + " is null.");
            }
        }
    }

    void DuplicateSplineAndSpawnAI()
    {
        // Duplicate the original spline multiple times for the AI
        for (int i = 0; i < numberOfAI; i++)
        {
            // Create a new position inside the platform bounds
            Vector3 newPosition = GetRandomPositionInsideBounds();

            // Instantiate and assign a new duplicated spline to the AI
            InstantiateAIWithNewSpline(newPosition);
        }
    }

    Vector3 GetRandomPositionInsideBounds()
    {
        float x = UnityEngine.Random.Range(platformMinBounds.x, platformMaxBounds.x);
        float z = UnityEngine.Random.Range(platformMinBounds.z, platformMaxBounds.z);
        return new Vector3(x, platformMinBounds.y, z);
    }

    void InstantiateAIWithNewSpline(Vector3 position)
    {
        GameObject ai = Instantiate(aiPrefab, position, Quaternion.identity);

        // Duplicate the original spline without random offset
        SplineContainer newSplineContainer = DuplicateSpline(originalSpline, position);

        // Assign the new spline to the AI controller
        AIController aiController = ai.GetComponent<AIController>();
        if (aiController != null)
        {
            aiController.spline = newSplineContainer;
        }
        else
        {
            Debug.LogError("AIController component not found on AI prefab.");
        }
    }

    SplineContainer DuplicateSpline(SplineContainer originalSpline, Vector3 position)
    {
        // Create a new empty GameObject to hold the duplicated spline
        GameObject splineObject = new GameObject("AISpline");
        SplineContainer newSplineContainer = splineObject.AddComponent<SplineContainer>();

        // Duplicate the original spline and place it at the new position
        Spline newSpline = new Spline();
        for (int i = 0; i < originalSpline.Spline.Count; i++)
        {
            BezierKnot originalKnot = originalSpline.Spline[i];

            // Use the original knot positions without applying offsets
            BezierKnot newKnot = new BezierKnot(
                originalKnot.Position,
                originalKnot.TangentIn,
                originalKnot.TangentOut
            );
            newSpline.Add(newKnot);
        }

        // Assign the new spline to the new spline container
        newSplineContainer.Spline = newSpline;

        // Place the new spline container at the desired position
        newSplineContainer.transform.position = position;

        // Adjust spline to be within the bounds if necessary
        if (!IsSplineWithinBounds(newSplineContainer))
        {
            AdjustSplineWithinBounds(newSplineContainer);
        }

        return newSplineContainer;
    }

    bool IsSplineWithinBounds(SplineContainer splineContainer)
    {
        // Check if the spline's control points are within the platform bounds
        foreach (var knot in splineContainer.Spline)
        {
            Vector3 position = (Vector3)knot.Position + splineContainer.transform.position;
            if (position.x < platformMinBounds.x || position.x > platformMaxBounds.x ||
                position.z < platformMinBounds.z || position.z > platformMaxBounds.z)
            {
                return false;
            }
        }
        return true;
    }

    void AdjustSplineWithinBounds(SplineContainer splineContainer)
    {
        // Define a height above the platform for the spline
        float heightAboveGround = 0.18f; // Adjust this value as needed

        // Adjust position to ensure the spline is above the ground
        Vector3 newPosition = new Vector3(
            Mathf.Clamp(splineContainer.transform.position.x, platformMinBounds.x, platformMaxBounds.x),
            heightAboveGround, // Set to a fixed height above ground
            Mathf.Clamp(splineContainer.transform.position.z, platformMinBounds.z, platformMaxBounds.z)
        );

        splineContainer.transform.position = newPosition;
    }
}