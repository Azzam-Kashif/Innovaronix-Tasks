using UnityEngine;
public class PlayerController : MovementWithSpline
{
    private float initialPositionX;
    private bool isDragging = false;
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        HandleMouseDrag();
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialPositionX = Input.mousePosition.x;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            float dragDistance = (Input.mousePosition.x - initialPositionX) / Screen.width;
            lateralOffset -= dragDistance * sidewaysForceMultiplier * dragSensitivity;
            lateralOffset = Mathf.Clamp(lateralOffset, -maxLateralOffset, maxLateralOffset);

            initialPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
}
