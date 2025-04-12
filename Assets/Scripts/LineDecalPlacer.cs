using UnityEngine;

public class LineDecalPlacer : MonoBehaviour
{
    [SerializeField] private Transform player; // Assign your character
    [SerializeField] private LayerMask surfaceLayer; // Define valid surfaces
    [SerializeField] private float offsetDistance = 0.01f; // Prevents Z-fighting
    [Space(5)]
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;



    void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        if (player == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, surfaceLayer))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = ray.origin + ray.direction * 50f; 
        }

        transform.position = player.position + Vector3.up * offsetDistance;

        Vector3 decalPosition = player.position;
        Vector3 direction = targetPosition - decalPosition;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f) 
        {
            float yRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(offsetX, yRotation, offsetY);
        }
    }
}
