using UnityEngine;
using ProjectDawn.Navigation.Hybrid;
using ProjectDawn.Navigation;
using System.Collections;
using Unity.Transforms;

public class AgentSetDestination : MonoBehaviour
{
    public Transform Target;
    private AgentAuthoring agentAuthoring;
    private Rigidbody rb;

    void Start()
    {
        agentAuthoring = GetComponent<AgentAuthoring>();
        rb = GetComponent<Rigidbody>();
        //StartCoroutine(CurrentAndNextPosition());
    }

    private void Update()
    {
    }
    private void FixedUpdate()
    {
        //agentAuthoring.SetDestination(Target.position);
        var body = agentAuthoring.EntityBody;

        if (Target !=null)
        {
            body.IsStopped = false;
            body.Destination = Target.position;
            agentAuthoring.EntityBody = body;
        }
    }

}

