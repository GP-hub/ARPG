using UnityEngine;

public class LineToMouse : MonoBehaviour
{
    public Transform player; // Assign the player in the Inspector
    public LineRenderer lineRenderer;
    public LayerMask surfaceLayer; // The layer where the mouse can hit
    public float offsetDistance = 0.01f; // Prevents Z-fighting

    void Update()
    {
        UpdateLine();
    }

    void UpdateLine()
    {
        if (player == null || lineRenderer == null) return;

        // Define an upward offset to prevent the line from starting too low
        Vector3 playerOffset = new Vector3(0, 0.1f, 0); // Adjust Y offset as needed
        Vector3 startPosition = player.position + playerOffset;

        // Raycast from the player toward the mouse direction
        Ray ray = new Ray(startPosition, (GetMouseWorldPosition() - startPosition).normalized);
        RaycastHit hit;
        Vector3 endPosition;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, surfaceLayer))
        {
            endPosition = hit.point; // Stop at first hit
        }
        else
        {
            endPosition = ray.origin + ray.direction * 50f; // Default far point if no hit
        }

        // Update the LineRenderer positions
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    // Helper method to get the world position of the mouse cursor
    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, surfaceLayer))
        {
            return hit.point;
        }

        return ray.origin + ray.direction * 50f; // Default far point
    }
}
