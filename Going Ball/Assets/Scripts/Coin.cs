using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // Value of the coin

    // Detect collision with the ball
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            CollectCoin();
        }
    }

    // Handle coin collection
    private void CollectCoin()
    {
        // Update the score
        ScoreManager.Instance.AddScore(coinValue);

        // Destroy the coin
        Destroy(gameObject);
    }
}
