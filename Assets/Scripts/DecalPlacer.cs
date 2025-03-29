using UnityEngine;

public class DecalPlacer : MonoBehaviour
{
    public LayerMask surfaceLayer; // Set this in the Inspector to define valid surfaces
    public float offsetDistance = 0.01f; // Offset to prevent Z-fighting

    void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, surfaceLayer))
        {
            // Move to the hit point + small offset in the direction of the normal
            transform.position = hit.point + (hit.normal * offsetDistance);

            // Align with the surface normal
            transform.rotation = Quaternion.LookRotation(-hit.normal);
        }
    }
}
