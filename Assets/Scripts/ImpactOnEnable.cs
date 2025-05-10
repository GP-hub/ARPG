using UnityEngine;

public class ImpactOnEnable : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float impactForce = 50f;

    private void OnEnable()
    {
        const int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];

        int numHits = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, LayerMask.GetMask("Character"));

        for (int i = 0; i < numHits; i++)
        {
            hitColliders[i].GetComponent<ImpactReceiver>()?.AddImpact(hitColliders[i].transform.position - transform.position, impactForce);
        }
    }
}
