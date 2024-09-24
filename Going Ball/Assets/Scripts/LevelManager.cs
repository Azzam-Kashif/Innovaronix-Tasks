using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class LevelController : MonoBehaviour
{
    public GameObject aiPrefab;  // Prefab for the AI-controlled ball
    public int numberOfAI;   // Number of AI-controlled balls to spawn
    public SplineContainer spline;  // Reference to the spline
    public Transform playerSpawnPoint;  // Player spawn point
    public Transform[] aiSpawnPoints;  // Array of predefined AI spawn points
    public GameObject player;  // Reference to the player object

    public Vector3 offsetRange = new Vector3(2f, 0, 2f);  // Define a range for random offset (x and z directions)

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
        // Check if predefined spawn points are available and valid
        if (aiSpawnPoints == null || aiSpawnPoints.Length == 0)
        {
            Debug.LogError("No AI spawn points assigned in the inspector.");
            yield break;
        }

        int spawnPointsAvailable = Mathf.Min(numberOfAI, aiSpawnPoints.Length);

        // Spawn AI balls at predefined spawn points with a random offset
        for (int i = 0; i < spawnPointsAvailable; i++)
        {
            if (aiSpawnPoints[i] != null)
            {
                // Apply a random offset within the defined range
                Vector3 randomOffset = new Vector3(Random.Range(-offsetRange.x, offsetRange.x), 0, Random.Range(-offsetRange.z, offsetRange.z));
                Vector3 spawnPosition = aiSpawnPoints[i].position + randomOffset;

                GameObject ai = Instantiate(aiPrefab, spawnPosition, Quaternion.identity);
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
            else
            {
                Debug.LogWarning("Spawn point " + i + " is null.");
            }
        }
    }
}
