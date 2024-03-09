using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeEnemySpell : MonoBehaviour
{
    [SerializeField] private float radius = 3f; // Adjust this value according to your needs
    [SerializeField] private LayerMask characterLayer;

    void OnEnable()
    {
        // Perform the sphere cast
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, Vector3.forward, 0f, characterLayer);

        // Iterate through the hits
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                EventManager.Instance.PlayerTakeHeal(100);
                // Do whatever you want with the player GameObject here
                Debug.Log("Player found in POWER aoe: do damage");
            }
        }
    }
}
