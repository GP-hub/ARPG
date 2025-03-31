using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 directionToTarget = target.position - rb.position;
            Vector3 moveDirection = directionToTarget.normalized;
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }
}
