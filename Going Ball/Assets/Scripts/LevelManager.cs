using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class LevelController : MonoBehaviour
{
    public GameObject aiPrefab;
    public int numberOfAI = 3; // Number of AI-controlled balls to spawn
    public SplineContainer spline;
    public Transform playerSpawnPoint;
    public Transform[] aiSpawnPoints;
    public GameObject player;

    void Start()
    {
        // Spawn player
        if (player != null)
        {
            player.GetComponent<MovementWithSpline>().spline = spline;
        }
        else
        {
            Debug.LogError("Player object is not assigned.");
        }

        StartCoroutine(SpawnAIBalls());
    }

    IEnumerator SpawnAIBalls()
    {
        // Check if aiSpawnPoints length is sufficient
        int spawnPointsAvailable = Mathf.Min(numberOfAI, aiSpawnPoints.Length);

        for (int i = 0; i < spawnPointsAvailable; i++)
        {
            // Instantiate AI ball at each spawn point
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
            yield return new WaitForSeconds(0.1f);
        }

        // If there are more AI balls to spawn than predefined spawn points, use random positions
        int remainingAI = numberOfAI - spawnPointsAvailable;
        for (int i = 0; i < remainingAI; i++)
        {
            float xPos = Random.Range(1, 50);
            float zPos = Random.Range(1, 31);
            GameObject ai = Instantiate(aiPrefab, new Vector3(xPos, 43, zPos), Quaternion.identity);
            AIController aiController = ai.GetComponent<AIController>();
            if (aiController != null)
            {
                aiController.spline = spline;
            }
            else
            {
                Debug.LogError("AIController component not found on AI prefab.");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
