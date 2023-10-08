using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("COLLIDED WITH PARTICLES");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLIDED WITH: " + other.name);
    }
}
