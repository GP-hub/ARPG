using UnityEngine;
using UnityEngine.AI;

public enum Quadrant { Front, Right, Back, Left }

public static class Utility
{
    // Fixed wander/flee distance
    private const float k_Distance = 3f;
    // How close NavMesh.SamplePosition is allowed to move the point (tweak to your grid density)
    private const float k_SampleRadius = 1f;

    public static Vector3 GetPositionPoint(Vector3 origin, Vector3 target, IdleBehaviorType behavior, NavMeshAgent agent, LayerMask obstacleMask)
    {
        // 1) Try primary wedge
        Vector3 candidate = SampleInWedge(origin, target, behavior, agent, obstacleMask);
        if (candidate != origin) return candidate;

        // 2) Fallback: wide wedge [-135°..+135°] around the “behind” direction
        //    (i.e. exclude a 90° forward cone; pick anywhere from 45° to 315° relative to facing)
        candidate = SampleInAngleRange(origin, target, agent, obstacleMask, 45f, 315f);
        return candidate;
    }

    private static Vector3 SampleInWedge(Vector3 origin, Vector3 target, IdleBehaviorType behavior, NavMeshAgent agent, LayerMask obstacleMask)
    {
        float baseYaw = Quaternion.LookRotation((target - origin).WithY(0)).eulerAngles.y;
        float angleOffset;

        switch (behavior)
        {
            case IdleBehaviorType.Agressive:
                angleOffset = Random.Range(-45f, 45f);
                break;
            case IdleBehaviorType.Flee:
                angleOffset = Random.Range(135f, 225f);
                break;
            case IdleBehaviorType.Wander:
                // left or right flank
                if (Random.value < .5f)
                    angleOffset = Random.Range(45f, 135f);
                else
                    angleOffset = Random.Range(225f, 315f);
                break;
            default:
                return origin;
        }

        return SampleAtAngle(baseYaw + angleOffset, origin, agent, obstacleMask);
    }

    private static Vector3 SampleInAngleRange(Vector3 origin, Vector3 target, NavMeshAgent agent, LayerMask obstacleMask, float minOffset, float maxOffset)
    {
        float baseYaw = Quaternion.LookRotation((target - origin).WithY(0)).eulerAngles.y;
        float angleOffset = Random.Range(minOffset, maxOffset);
        return SampleAtAngle(baseYaw + angleOffset, origin, agent, obstacleMask);
    }

    private static Vector3 SampleAtAngle(float worldYaw, Vector3 origin, NavMeshAgent agent, LayerMask obstacleMask)
    {
        Vector3 dir = Quaternion.Euler(0, worldYaw, 0) * Vector3.forward;
        Vector3 raw = origin + dir * k_Distance;
        int mask = agent.areaMask;

        // 1) Snap to mesh
        if (!NavMesh.SamplePosition(raw, out NavMeshHit navHit, k_SampleRadius, mask))
            return origin;

        Vector3 pos = navHit.position;

        // 2) Straight-line NavMesh check
        if (!NavMeshHasStraightLine(origin, pos, mask))
            return origin;

        // 3) Physics LOS
        Vector3 rayOrigin = origin + Vector3.up * 1.5f;
        if (Physics.Raycast(rayOrigin, (pos - rayOrigin).normalized, out _, k_Distance, obstacleMask))
            return origin;

        return pos;
    }

    // returns true if you can go in a straight line on the NavMesh, no circuitous detours
    private static bool NavMeshHasStraightLine(Vector3 from, Vector3 to, int areaMask)
    {
        // t will be the last valid point along the straight line
        return !NavMesh.Raycast(from, to, out _, areaMask);
    }


    private static Vector3 WithY(this Vector3 v, float y)
    {
        v.y = y;
        return v;
    }
    public static void RotateTowardsTarget(Transform source, Vector3 target, float rotationSpeed)
    {
        Vector3 direction = (target - source.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        source.rotation = Quaternion.Slerp(source.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    //public static float GetClipThreshold(BlendTree blendTree, AnimationClip clipToFind)
    //{
    //    if (blendTree == null || clipToFind == null)
    //    {
    //        Debug.Log("BlendTree or AnimationClip is null.");
    //        return -1f;
    //    }

    //    foreach (ChildMotion child in blendTree.children)
    //    {
    //        if (child.motion is AnimationClip clip && clip == clipToFind)
    //        {
    //            return child.threshold;
    //        }
    //        else if (child.motion is BlendTree nestedBlendTree)
    //        {
    //            float nestedThreshold = GetClipThreshold(nestedBlendTree, clipToFind);
    //            if (nestedThreshold != -1f)
    //            {
    //                return nestedThreshold;
    //            }
    //        }
    //    }

    //    Debug.Log($"Clip '{clipToFind.name}' not found in the blend tree.");
    //    return -1f;
    //}
}
