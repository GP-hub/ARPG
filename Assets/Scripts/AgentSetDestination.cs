using UnityEngine;


public class AgentSetDestination : MonoBehaviour
{
    public Transform Target;
    //private AgentAuthoring agentAuthoring;
    private Rigidbody rb;
    private Vector3 move;

    void Start()
    {
        //agentAuthoring = GetComponent<AgentAuthoring>();
        rb = GetComponent<Rigidbody>();
        //StartCoroutine(CurrentAndNextPosition());
    }

    private void Update()
    {
    }
    private void FixedUpdate()
    {
        //MoveToTarget();
        //var body = agentAuthoring.EntityBody;

        //if (Target != null)
        //{
        //    body.IsStopped = true;
        //    body.Destination = Target.position;
        //    agentAuthoring.EntityBody = body;
        //    Vector3 move = new Vector3(body.Force.x, 0, body.Force.y);
        //    rb.MovePosition(transform.position + (move * 4.5f * Time.fixedDeltaTime));

        //}

    }
    private void LateUpdate()
    {
    }

    void MoveToTarget()
    {
        if (Target != null)
        {
            Vector3 direction = (Target.position - transform.position).normalized;
            Vector3 movement = direction * 4.5f * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + movement);

            // Rotate to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 5f));
        }
    }


}



