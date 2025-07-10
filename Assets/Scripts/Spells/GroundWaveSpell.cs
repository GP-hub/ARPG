using System.Collections;
using UnityEngine;

public class GroundWaveSpell : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float timeFailsafe;

    [Header("Collision")]
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Damage")]
    [SerializeField] private AbilityValues abilityValues;

    private Vector3 startPosition;
    private float maxTravelDistanceSqr;

    private void OnEnable()
    {
        startPosition = transform.position;
        maxTravelDistanceSqr = maxTravelDistance * maxTravelDistance;
        StartCoroutine(DisableAfterTime(timeFailsafe));
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);

        if ((transform.position - startPosition).sqrMagnitude >= maxTravelDistanceSqr)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int layer = 1 << other.gameObject.layer;

        if ((obstacleLayer.value & layer) != 0)
        {
            gameObject.SetActive(false); // Stop at obstacle
        }
        else if ((characterLayer.value & layer) != 0 && other.CompareTag("Player"))
        {
            abilityValues.playersToDamage.Add(other.gameObject);
            abilityValues.DoDamage(abilityValues.Damage);
            // Keep moving — no disable
        }
    }

    private IEnumerator DisableAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }


}
