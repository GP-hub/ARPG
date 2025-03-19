using UnityEngine;

public static class Utility
{
    public static void RotateTowardsTarget(Transform source, Transform target, float rotationSpeed)
    {
        Vector3 direction = (target.position - source.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        source.rotation = Quaternion.Slerp(source.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}
