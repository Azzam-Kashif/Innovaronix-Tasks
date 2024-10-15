using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CustomerAI : MonoBehaviour
{
    public Transform target; // The destination the AI will move towards
    private Seeker seeker;
    private AIPath aiPath;
    private Animator animator;
    public float stopDistance = 0.5f;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        aiPath.destination = target.position;  // Set target position
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Update destination dynamically if target moves
        aiPath.destination = target.position;
        float speed = aiPath.velocity.magnitude;  // Get the current speed of the AI

        // Set the Speed parameter in the Animator to control walk/idle animations
        animator.SetFloat("Speed", speed);

        if (!aiPath.pathPending && aiPath.remainingDistance <= stopDistance)
        {
            // Stop the AI by setting its velocity to zero
            aiPath.isStopped = true;
            // Optional: Set the AI's speed to 0 to switch to idle animation
            animator.SetFloat("Speed", 0);
        }
        else
        {
            // Ensure AI keeps moving if it's not close enough to the target
            aiPath.isStopped = false;
        }
    }
}
