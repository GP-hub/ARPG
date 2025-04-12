using UnityEngine;

public class LineDecalPlacer : MonoBehaviour
{
    public Transform player; // Assign your character
    public LayerMask surfaceLayer; // Define valid surfaces
    public float offsetDistance = 0.01f; // Prevents Z-fighting

    public float offsetX = 0f; // X-axis offset
    public float offsetY = 0f; // Y-axis offset
    public float offsetZ = 0f; // Z-axis offset

    void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        if (player == null) return;

        // Raycast from mouse to world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, surfaceLayer))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = ray.origin + ray.direction * 50f; // Default far point if no hit
        }

        // Set the decal position at the player's position with offsets
        transform.position = player.position + new Vector3(offsetX, offsetY, offsetZ) + Vector3.up * offsetDistance;

        Vector3 decalPosition = player.position + new Vector3(offsetX, offsetY, offsetZ);
        Vector3 direction = targetPosition - decalPosition;
        direction.y = 0;

        // Compute rotation but lock it to Y-axis only
        if (direction.sqrMagnitude > 0.01f) // Prevents issues when cursor is on player
        {
            float yRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(90, yRotation, 90);
        }
    }
}
