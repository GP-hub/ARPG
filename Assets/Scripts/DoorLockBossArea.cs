using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate; // Drag the object you want to enable here
    [SerializeField] private LayerMask characterLayer;    // Assign the "Character" layer here

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered matches the character layer
        if (((1 << other.gameObject.layer) & characterLayer) != 0)
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                this.gameObject.SetActive(false); // Disable this trigger after activation
            }
        }
    }
}
