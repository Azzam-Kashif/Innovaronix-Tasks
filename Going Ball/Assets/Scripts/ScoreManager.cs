using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public Text scoreText; // Assign a UI Text element to display the score
    private int score = 0; // Initialize score

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Method to add score when a coin is collected
    public void AddScore(int value)
    {
        score += value; // Increment the score
        UpdateScoreUI(); // Update the score display
    }

    // Update the score display
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // Update the score text
        }
    }
}
