using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit:" + other.transform.name);
    }
}
