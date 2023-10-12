using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("COLLIDED WITH PARTICLES: " + other.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLIDED WITH: " + other.name);
    }
}
