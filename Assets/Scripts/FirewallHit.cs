using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallHit : MonoBehaviour
{
    private string playerFireballName;

    public void Initialize(string attackName)
    {
        playerFireballName = attackName.ToLower(); // cache as lowercase if needed
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.ToLower().Contains(playerFireballName))
        {
            if (!other.transform.name.ToLower().Contains("enemy"))
            {
                other.transform.GetComponent<Fireball>().procChance += 100;
            }
        }
    }
}
