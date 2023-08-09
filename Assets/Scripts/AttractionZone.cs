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


//public class enemyMovement : MonoBehaviour
//{
//    public Transform player;
//    public Transform model;

//    public Transform proxy;
//    NavMeshAgent agent;
//    NavMeshObstacle obstacle;
//    Vector3 lastPosition;

//    void Start()
//    {
//        agent = proxy.GetComponent<NavMeshAgent>();
//        obstacle = proxy.GetComponent<NavMeshObstacle>();
//    }
//    void Update()
//    {
//        // Test if the distance between the agent (which is now the proxy) and the player is less than the attack range(or the stoppingDistance parameter)
//        if ((player.position - proxy.position).sqrMagnitude < Mathf.Pow(agent.stoppingDistance, 2))
//        {
//            //If the agent is in attack range, become an obstacle and  disable the NavMeshAgent component
//            obstacle.enabled = true;
//            agent.enabled = false;
//        }
//        else
//        {
//            //If we are not in range, become an agent again
//            obstacle.enabled = false;
//            agent.enabled = true;

//            //And move to the player's position
//            agent.destination = player.position;
//        }

//        model.position = Vector3.Lerp(model.position, proxy.position, Time.deltaTime * 2);

//        //Calculate the orientation based on the velocity of the agent
//        Vector3 orientation = model.position - lastPosition;

//        //Check if the agent has some minimal velocity
//        if (orientation.sqrMagnitude > 0.1f)
//        {
//            // We don't want him to look up or down
//            orientation.y = 0;
//            // Use Quaternion.LookRotation() to set the model's new rotation and smooth the transition with Quaternion.Lerp();
//            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(model.position - lastPosition), Time.deltaTime * 8);
//        }
//        else
//        {
//            // If the agent is stationary we tell him to assume the proxy's rotation 
//            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(proxy.forward), Time.deltaTime * 8);
//        }
//        //This is needed to calculate the orientation in the next frame
//        lastPosition = model.position;
//    }
//}

