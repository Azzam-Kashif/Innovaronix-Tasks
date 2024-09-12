using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 turnDirection = Vector3.forward; // Set this in the inspector or dynamically

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assuming the ball is tagged as "Player"
        {
            Debug.Log("CurvePoint detected!");
            // Call StartTurn on the ball controller
            BallController.Instance.StartTurn(turnDirection);
        }
    }
}
