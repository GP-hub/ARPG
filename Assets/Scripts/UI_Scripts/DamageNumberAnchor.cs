using UnityEngine;

public class DamageNumberAnchor : MonoBehaviour
{
    private Vector3 worldPosition;

    public void Initialize(Vector3 worldPos)
    {
        worldPosition = worldPos;
    }

    void Update()
    {
        // Convert world position to screen position each frame
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        transform.position = screenPos;
    }
}
