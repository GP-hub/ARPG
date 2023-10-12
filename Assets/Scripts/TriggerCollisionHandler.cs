using UnityEngine;

public class TriggerCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided with: " + other.transform.name);
    }
}
