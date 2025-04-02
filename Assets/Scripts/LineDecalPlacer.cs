using UnityEngine;

public class LineDecalPlacer : MonoBehaviour
{
    public Transform player; // Assign your character
    public LayerMask surfaceLayer; // Define valid surfaces
    public float offsetDistance = 0.01f; // Prevents Z-fighting

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

        // Set the decal position at the player's position (with small height offset)
        transform.position = player.position + Vector3.up * offsetDistance;

        // Get direction from player to cursor position
        Vector3 direction = targetPosition - player.position;
        direction.y = 0; // Completely ignore vertical movement

        // Compute rotation but lock it to Y-axis only
        if (direction.sqrMagnitude > 0.01f) // Prevents issues when cursor is on player
        {
            float yRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(90, yRotation, 90);
        }
    }
}
