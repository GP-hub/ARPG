using UnityEngine;

public class LineToMouse : MonoBehaviour
{
    public Transform player; // Assign the player in the Inspector
    public LineRenderer lineRenderer;
    public LayerMask surfaceLayer; // The layer where the mouse can hit (ground)
    public LayerMask obstacleLayer; // The layer where obstacles may be present
    public float offsetDistance = 0.01f; // Prevents Z-fighting
    public float lineLength = 5f; // Define the length of the line

    void Update()
    {
        UpdateLine();
    }

    void UpdateLine()
    {
        if (player == null || lineRenderer == null) return;

        Vector3 playerOffset = new Vector3(0, 0.1f, 0); // Slightly above the player
        Vector3 startPosition = player.position + playerOffset;

        // Get mouse position projected onto the same Y height as the player
        Vector3? projectedMousePos = GetMousePositionOnPlayerPlane(player.position.y);

        if (projectedMousePos == null)
            return;

        Vector3 targetPosition = projectedMousePos.Value;

        // Adjust target to be on the ground plane (the Y value of the player's position)
        targetPosition.y = startPosition.y;

        // Get the direction from the player to the target position (mouse position on ground)
        Vector3 direction = (targetPosition - startPosition).normalized;

        if (direction == Vector3.zero)
            direction = player.forward; // Default direction if no valid mouse input

        // Raycast to detect if there are obstacles along the path
        RaycastHit hitInfo;
        if (Physics.Raycast(startPosition, direction, out hitInfo, lineLength, obstacleLayer))
        {
            // If an obstacle is hit, stop the line at the hit point
            targetPosition = hitInfo.point;
        }
        else
        {
            // If no obstacle is hit, use the defined line length
            targetPosition = startPosition + direction * lineLength;
        }

        // Set the LineRenderer positions
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, targetPosition);

        // Visualize the line path (optional)
        Debug.DrawLine(startPosition, targetPosition, Color.green, 0.1f);
    }

    // Projects the mouse cursor onto a horizontal plane at a specific Y height
    Vector3? GetMousePositionOnPlayerPlane(float heightY)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Plane at player's height
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, heightY, 0));

        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return null; // Mouse doesn't intersect the plane (unlikely)
    }
}
