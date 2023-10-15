using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallHit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.ToLower().Contains("fireball"))
        {
            other.transform.GetComponent<Fireball>().currentProcChance = 100;
        }
    }
}
