using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class LevelController : MonoBehaviour
{
    public GameObject aiPrefab;  // Prefab for the AI-controlled ball
    public int numberOfAI = 3;   // Number of AI-controlled balls to spawn
    public SplineContainer spline;  // Reference to the spline
    public Transform playerSpawnPoint;  // Player spawn point
    public Transform[] aiSpawnPoints;  // Array of predefined AI spawn points
    public GameObject player;  // Reference to the player object

    // Grid parameters for dynamic AI spawning
    public Vector3 gridStartPosition = new Vector3(0, 43, 0);  // Starting point of the grid
    public float gridSpacingX = 5f;  // Distance between AI balls horizontally
    public float gridSpacingZ = 5f;  // Distance between AI balls vertically
    public int gridRows = 2;  // Number of rows in the grid

    void Start()
    {
        // Spawn player at the player spawn point
        if (player != null)
        {
            player.transform.position = playerSpawnPoint.position;
            player.GetComponent<MovementWithSpline>().spline = spline;
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
        // Check if predefined spawn points are available
        int spawnPointsAvailable = Mathf.Min(numberOfAI, aiSpawnPoints.Length);

        // Spawn AI balls at predefined spawn points
        for (int i = 0; i < spawnPointsAvailable; i++)
        {
            GameObject ai = Instantiate(aiPrefab, aiSpawnPoints[i].position, Quaternion.identity);
            AIController aiController = ai.GetComponent<AIController>();
            if (aiController != null)
            {
                aiController.spline = spline;
            }
            else
            {
                Debug.LogError("AIController component not found on AI prefab.");
            }
            yield return new WaitForSeconds(0.1f);  // Small delay between spawns
        }

        // Spawn remaining AI balls in a racing grid formation
        int remainingAI = numberOfAI - spawnPointsAvailable;
        int aiSpawnedInGrid = 0;

        for (int row = 0; row < gridRows && aiSpawnedInGrid < remainingAI; row++)
        {
            for (int col = 0; col < (remainingAI / gridRows) + 1 && aiSpawnedInGrid < remainingAI; col++)
            {
                Vector3 gridPosition = gridStartPosition + new Vector3(col * gridSpacingX, 0, row * gridSpacingZ);
                GameObject ai = Instantiate(aiPrefab, gridPosition, Quaternion.identity);
                AIController aiController = ai.GetComponent<AIController>();
                if (aiController != null)
                {
                    aiController.spline = spline;
                }
                else
                {
                    Debug.LogError("AIController component not found on AI prefab.");
                }
                aiSpawnedInGrid++;
                yield return new WaitForSeconds(0.1f);  // Small delay between spawns
            }
        }
    }
}
