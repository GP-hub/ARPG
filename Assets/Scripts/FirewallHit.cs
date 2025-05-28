using UnityEngine;
using static Firewall;

public class FirewallHit : MonoBehaviour
{
    private string playerFireballName;
    private FirewallAttackContext context;

    public void Initialize(string attackName, FirewallAttackContext newContext)
    {
        playerFireballName = attackName.ToLower();
        InitializeContextForDot(newContext);
        context = newContext;
    }

    private void OnTriggerEnter(Collider other)
    {
        FireballProcChanceBonus(other);
    }

    private void FireballProcChanceBonus(Collider other)
    {
        if (other.transform.name.ToLower().Contains(playerFireballName))
        {
            if (!other.transform.name.ToLower().Contains("enemy"))
            {
                other.transform.GetComponent<Fireball>().procChance += 100;
            }
        }
    }

    private void InitializeContextForDot(Firewall.FirewallAttackContext newContext)
    {
        FireGroundDoT dot = GetComponent<FireGroundDoT>();
        if (dot != null)
        {
            dot.SetContext(newContext);
        }
    }
}
