using UnityEngine;
using UnityEngine.AI;

public class AttractionZone : MonoBehaviour
{
    //public float attractionForce;

    //public float explosionForce = 100f;

    public float explosionRadius = 10f;

    public bool explode;

    //public float pullingForce = 10000f;
    //public float pullingRange = 5f;

    [SerializeField] private LayerMask characterLayer;

    private void Start()
    {
        InvokeRepeating("Explode", 1f, 3f);
    }

    private void FixedUpdate()
    {
        //Explode();
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


        ////RIGIDBODY EXPLOSION
        //Collider[] colliders = Physics.OverlapSphere(this.transform.position, explosionRadius);

        //foreach (Collider col in colliders)
        //{
        //    Rigidbody rb = col.GetComponent<Rigidbody>();

        //    if (rb != null)
        //    {
        //        // Calculate the explosion force based on distance
        //        float distance = Vector3.Distance(rb.transform.position, this.transform.position);
        //        float forceMagnitude = 1 - (distance / explosionRadius);
        //        forceMagnitude = Mathf.Clamp01(forceMagnitude); // Clamp to [0, 1]

        //        // Apply explosion force
        //        //rb.AddExplosionForce(explosionForce * forceMagnitude, explosionCenter, explosionRadius);
        //        rb.AddExplosionForce(50, this.transform.position, 10);
        //    }
        //}

        // OLD SCRIPT FOR USING RIGIDBODY
        // Max number of entities in the OverlapSphere
        //int maxColliders = 10;
        //Collider[] hitColliders = new Collider[maxColliders];
        //int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, hitColliders, characterLayer);

        //for (int i = 0; i < numColliders; i++)
        //{
        //    Rigidbody rb = hitColliders[i].GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {
        //        // Calculate the explosion force based on distance
        //        float distance = Vector3.Distance(rb.transform.position, this.transform.position);
        //        float forceMagnitude = 1 - (distance / explosionRadius);
        //        forceMagnitude = Mathf.Clamp01(forceMagnitude); // Clamp to [0, 1]

        //        // Apply explosion force
        //        //rb.AddExplosionForce(explosionForce * forceMagnitude, explosionCenter, explosionRadius);
        //        rb.AddExplosionForce(300, this.transform.position, 200);
        //    }
        //}

        // PUSHING AND PULLING USING CHARACTER CONTROLLER
        // Max number of entities in the OverlapSphere
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            CharacterController cc = hitColliders[i].GetComponent<CharacterController>();
            if (cc != null)
            {
                ImpactReceiver ir = cc.GetComponent<ImpactReceiver>();
                
                if (ir!=null)
                {
                    // PUSHING
                    //ir.AddImpact(cc.transform.position-this.transform.position, 50);

                    // PULLING
                    //ir.AddImpact(this.transform.position-cc.transform.position, 50);
                }
            }
        }

        explode = false;
    }
}