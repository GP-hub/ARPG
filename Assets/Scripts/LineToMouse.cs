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

        // Raycast from the mouse cursor
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, surfaceLayer))
        {
            targetPosition = hit.point + (hit.normal * offsetDistance); // Apply offset
        }
        else
        {
            targetPosition = ray.origin + ray.direction * 50f; // Default far point if no hit
        }

        // Update the LineRenderer positions
        lineRenderer.SetPosition(0, player.position);
        lineRenderer.SetPosition(1, targetPosition);
    }
}
