using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityValues))]
public class AoeEnemySpell : MonoBehaviour
{
    [SerializeField] private float radius = 3f; // Adjust this value according to your needs
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private AbilityValues abilityValues;


    void OnEnable()
    {
        transform.localScale = new Vector3(radius * 2, transform.localScale.y, radius * 2);

        // Max number of entities in the OverlapSphere
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                abilityValues.playersToDamage.Add(hitColliders[i].gameObject);
                //EventManager.PlayerTakeDamage(20);
            }
        }
    }
    public void DoDamage(int damage)
    {
          abilityValues.DoDamage(damage);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
