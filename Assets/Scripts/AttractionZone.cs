using UnityEngine;
using UnityEngine.AI;

public class AttractionZone : MonoBehaviour
{
    public float attractionForce;

    public float explosionForce = 100f;
    public float explosionRadius = 5f;

    public bool explode;

    public float pullingForce = 10000f;
    public float pullingRange = 5f;

    private void FixedUpdate()
    {
        Explode();
        //if (explode)
        //{
        //    Invoke("Explode", 0f);
        //}
    }


    private void OnTriggerEnter(Collider other)
    {

    }
    void Explode()
    {
        //// RIGIDBODY PULLING
        //// Get all colliders within the pulling range
        //Collider[] colliders = Physics.OverlapSphere(transform.position, pullingRange);

        //foreach (Collider col in colliders)
        //{
        //    if (col.tag == "Player")
        //    {

        //        Rigidbody rb = col.GetComponent<Rigidbody>();

        //        if (rb != null)
        //        {
        //            Debug.Log("exploded: " +col.name);
        //            // Calculate pull direction
        //            Vector3 pullDirection = transform.position - rb.transform.position;

        //            // Apply pulling force
        //            rb.AddForce(pullDirection.normalized * pullingForce);
        //        }
        //    }
        //}


        //RIGIDBODY EXPLOSION
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, explosionRadius);

        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Debug.Log("exploded");
                // Calculate the explosion force based on distance
                float distance = Vector3.Distance(rb.transform.position, this.transform.position);
                float forceMagnitude = 1 - (distance / explosionRadius);
                forceMagnitude = Mathf.Clamp01(forceMagnitude); // Clamp to [0, 1]

                // Apply explosion force
                //rb.AddExplosionForce(explosionForce * forceMagnitude, explosionCenter, explosionRadius);
                rb.AddExplosionForce(50, this.transform.position, 10);
            }
        }

        // CHARACTER CONTROLLER PUSHING
        //Vector3 explosionCenter = transform.position; // Use your explosion's center
        //Collider[] colliders = Physics.OverlapSphere(explosionCenter, explosionRadius);

        //foreach (Collider col in colliders)
        //{
        //    Rigidbody rb = col.GetComponent<Rigidbody>();

        //    if (rb != null)
        //    {
        //        // Calculate the push direction based on the distance
        //        Vector3 pushDirection = rb.transform.position - explosionCenter;
        //        float distance = pushDirection.magnitude;
        //        float forceMagnitude = 1 - (distance / explosionRadius);
        //        forceMagnitude = Mathf.Clamp01(forceMagnitude); // Clamp to [0, 1]

        //        // Apply push and pull effect
        //        Vector3 pushForce = pushDirection.normalized * explosionForce * forceMagnitude;
        //        rb.AddForce(pushForce, ForceMode.Impulse);

        //        // Move character controller in opposite direction for pull effect
        //        StartCoroutine(PullEffect(characterController, pushForce, pushDuration));
        //    }
        //}



        explode = false;
    }
}