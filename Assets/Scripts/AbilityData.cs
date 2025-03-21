using UnityEngine;

[CreateAssetMenu(fileName = "NewAbilityData", menuName = "AbilityData")]
public class AbilityData : ScriptableObject
{
    public string moveName;
    public float maxAttackRange;
    public float minAttackRange;
    public int damage;
    public float cooldown;
    public GameObject projectilePrefab; // Optional, for ranged attacks
    public AnimationClip animationClip; // Animation for the attack move
    public string functionName; // Name of the function to handle the attack behavior
}
