using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingManager : MonoBehaviour
{
    private List<MovementWithSpline> players = new List<MovementWithSpline>();
    private List<AIController> aiControllers = new List<AIController>();

    public Text rankingText; // For standard UI Text
    // public TextMeshProUGUI rankingText; // Uncomment this if using TextMeshPro

    public void RegisterPlayer(MovementWithSpline player)
    {
        // Ensure the player is only registered once and is tagged as "Player"
        if (!players.Contains(player) && player.gameObject.CompareTag("Player"))
        {
            players.Add(player);
            Debug.Log("Player Registered: " + player.name);
        }
    }

    public void RegisterAI(AIController ai)
    {
        // Ensure the AI is only registered once and is tagged as "AI"
        if (!aiControllers.Contains(ai) && ai.gameObject.CompareTag("AI"))
        {
            aiControllers.Add(ai);
            Debug.Log("AI Registered: " + ai.name);
        }
    }

    public void UpdateRankings()
    {
        List<(MovementWithSpline participant, float distance)> participants = new List<(MovementWithSpline, float)>();

        // Add players, ensuring they are active and tagged correctly
        foreach (var player in players)
        {
            if (player != null && player.gameObject.activeInHierarchy)
            {
                participants.Add((player, player.GetDistancePercentage()));
            }
        }

        // Add AIs, ensuring they are active and tagged correctly
        foreach (var ai in aiControllers)
        {
            if (ai != null && ai.gameObject.activeInHierarchy)
            {
                participants.Add((ai, ai.GetDistancePercentage()));
            }
        }

        // Sort participants by distance traveled (Descending order)
        participants.Sort((x, y) => y.distance.CompareTo(x.distance));

        // Build the ranking string
        string rankingString = "Rankings:\n";
        for (int i = 0; i < participants.Count; i++)
        {
            rankingString += $"Rank {i + 1}: {participants[i].participant.name} - Distance: {participants[i].distance:F2}\n";
        }

        // Update the UI text
        rankingText.text = rankingString;
    }
}
