using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    // List to store the rankings
    private List<Ranking> rankings = new List<Ranking>();

    // Update the rankings
    public void UpdateRankings(List<GameObject> balls)
    {
        // Clear the current rankings
        rankings.Clear();

        // Loop through the balls and update their rankings
        foreach (GameObject ball in balls)
        {
            // Get the ball's position
            Vector3 ballPosition = ball.transform.position;

            // Calculate the ball's distance from the start point
            float distance = CalculateDistance(ballPosition);

            // Create a new ranking
            Ranking ranking = new Ranking(ball, distance);

            // Add the ranking to the list
            rankings.Add(ranking);
        }

        // Sort the rankings by distance
        rankings.Sort((a, b) => a.distance.CompareTo(b.distance));
    }

    // Calculate the distance from the start point
    private float CalculateDistance(Vector3 position)
    {
        // Define the start point
        Vector3 startPoint = new Vector3(0, 0, 0);

        // Calculate the distance
        float distance = Vector3.Distance(startPoint, position);

        return distance;
    }

    // Get the rankings
    public List<Ranking> GetRankings()
    {
        return rankings;
    }
}

// Ranking class
[System.Serializable]
public class Ranking
{
    public GameObject ball;
    public float distance;

    public Ranking(GameObject ball, float distance)
    {
        this.ball = ball;
        this.distance = distance;
    }
}