using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Make sure to include this if using standard UI
using TMPro; // Include this if using TextMeshPro

public class RankingManager : MonoBehaviour
{
    private List<MovementWithSpline> players = new List<MovementWithSpline>();
    private List<AIController> aiControllers = new List<AIController>();

    public Text rankingText; // For standard UI Text
    // public TextMeshProUGUI rankingText; // Uncomment this if using TextMeshPro

    public void RegisterPlayer(MovementWithSpline player)
    {
        players.Add(player);
    }

    public void RegisterAI(AIController ai)
    {
        aiControllers.Add(ai);
    }

    public void UpdateRankings()
    {
        List<(MovementWithSpline participant, float distance)> participants = new List<(MovementWithSpline, float)>();

        foreach (var player in players)
        {
            participants.Add((player, player.GetDistancePercentage()));
        }

        foreach (var ai in aiControllers)
        {
            participants.Add((ai, ai.GetDistancePercentage()));
        }

        // Sort participants by distance traveled
        participants.Sort((x, y) => y.distance.CompareTo(x.distance)); // Descending order

        // Build the ranking string
        string rankingString = "Rankings:\n";
        for (int i = 0; i < participants.Count; i++)
        {
            rankingString += $"Rank {i + 1}: {participants[i].participant.name} - Distance: {participants[i].distance:F2}\n";
        }

        // Update the UI text
        rankingText.text = rankingString;
        // rankingText.text = rankingString; // For TextMeshPro
    }
}
