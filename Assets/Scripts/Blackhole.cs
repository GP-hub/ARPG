using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{

    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private float radius = 10f;

    public void Explode()
    {
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            ImpactReceiver ir = hitColliders[i].GetComponent<ImpactReceiver>();

            if (ir != null)
            {
                if (ir.transform.tag != "Player")
                {
                    // PUSHING
                    ir.AddImpact(hitColliders[i].transform.position - this.transform.position, 50);

                    // PULLING
                    //ir.AddImpact(this.transform.position - hitColliders[i].transform.transform.position, 50);
                }
            }
        }
    }
}
