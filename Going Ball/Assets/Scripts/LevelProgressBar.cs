using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    public Slider progressBar;  // Assign this in the inspector
    public BallController ballController;  // Assign this in the inspector
    public Transform endPoint;  // Assign this in the inspector

    private void Update()
    {
        if (ballController != null && endPoint != null)
        {
            float progress = CalculateProgress();
            progressBar.value = progress;
        }
    }

    private float CalculateProgress()
    {
        float ballPositionZ = ballController.transform.position.z;
        float endPointZ = endPoint.position.z;
        float startPointZ = ballController.startPosition.z;

        // Clamp and normalize the progress value
        float clampedProgress = Mathf.Clamp01((ballPositionZ - startPointZ) / (endPointZ - startPointZ));
        return clampedProgress;
    }
}
