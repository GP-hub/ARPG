using UnityEditor.Animations;
using UnityEngine;

public static class Utility
{
    public static void RotateTowardsTarget(Transform source, Transform target, float rotationSpeed)
    {
        Vector3 direction = (target.position - source.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        source.rotation = Quaternion.Slerp(source.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public static float GetClipThreshold(BlendTree blendTree, AnimationClip clipToFind)
    {
        if (blendTree == null || clipToFind == null)
        {
            Debug.Log("BlendTree or AnimationClip is null.");
            return -1f;
        }

        foreach (ChildMotion child in blendTree.children)
        {
            if (child.motion is AnimationClip clip && clip == clipToFind)
            {
                return child.threshold;
            }
            else if (child.motion is BlendTree nestedBlendTree)
            {
                float nestedThreshold = GetClipThreshold(nestedBlendTree, clipToFind);
                if (nestedThreshold != -1f)
                {
                    return nestedThreshold;
                }
            }
        }

        Debug.Log($"Clip '{clipToFind.name}' not found in the blend tree.");
        return -1f;
    }
}
