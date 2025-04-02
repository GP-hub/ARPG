using UnityEngine;

public class DecalPlacer : MonoBehaviour
{
    public LayerMask surfaceLayer; // Set this in the Inspector to define valid surfaces
    public float offsetDistance; // Offset to prevent Z-fighting

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

//using UnityEngine;

//public class DecalPlacer : MonoBehaviour
//{
//    public LayerMask surfaceLayer; // Define valid surfaces
//    public float offsetDistance = 0.01f; // Prevents Z-fighting
//    public float maxSlopeAngle = 45f; // Maximum slope angle in degrees

//    void Update()
//    {
//        FollowMouse();
//    }

//    void FollowMouse()
//    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, Mathf.Infinity, surfaceLayer))
//        {
//            // Move to the hit point + small offset in the direction of the normal
//            transform.position = hit.point + (hit.normal * offsetDistance);

//            // Calculate the angle between the hit normal and the up vector (flat ground)
//            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

//            // Only modify rotation if slope is within the allowed range
//            if (slopeAngle <= maxSlopeAngle)
//            {
//                transform.rotation = Quaternion.LookRotation(-hit.normal);
//            }
//            else
//            {
//                // Keep the decal facing up (flat on the ground)
//                transform.rotation = Quaternion.Euler(90f, 0f, 0f);
//            }
//        }
//    }
//}
