using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewall : MonoBehaviour
{
    private Vector3 previousPos;
    private Vector3 currentPos;



    [SerializeField] private string firewallPrefabName;
    [SerializeField] private float firewallDamagePerTick;
    private FirewallAttackContext attackContext;
    private Coroutine firewallCoroutine;

    public string FirewallPrefabName { get => firewallPrefabName; set => firewallPrefabName = value; }

    private void OnEnable()
    {
        EventManager.onDashing += StartCoroutineFirewall;
    }
    private void OnDisable()
    {
        EventManager.onDashing -= StartCoroutineFirewall;
    }

    private void OnDestroy()
    {
        EventManager.onDashing -= StartCoroutineFirewall;
    }

    public float GetFirewallDamagePerTick()
    {
        return firewallDamagePerTick;
    }


    private void StartCoroutineFirewall(bool isDashing)
    {
        if (isDashing)
        {
            previousPos = this.transform.position;
            attackContext = new FirewallAttackContext(); // Create context for this dash
            firewallCoroutine = StartCoroutine(TrackPosition(isDashing));
        }
        else
        {
            if (firewallCoroutine != null)
            {
                StopCoroutine(firewallCoroutine);
                firewallCoroutine = null;
            }

            attackContext = null; // Reset context
        }
    }


    private IEnumerator TrackPosition(bool isDashing)
    {
        while (isDashing)
        {
            // Get the current position of the player
            currentPos = this.transform.position;

            FirewallGenerator(previousPos, currentPos);

            // Store the current position in previousPosition
            previousPos = currentPos;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FirewallGenerator(Vector3 pointA, Vector3 pointB)
    {
        if (pointA == null || pointB == null) return;
        if (pointA == pointB) return;

        Vector3 midpoint = (pointA + pointB) / 2;

        float distance = Vector3.Distance(pointA, pointB);

        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(firewallPrefabName, midpoint);

        if (newObject != null)
        {
            // Set the box's scale to match the distance between the points
            Vector3 scale = newObject.transform.localScale;
            scale.x = distance;
            newObject.transform.localScale = scale;

            // Rotate the box to point from pointA to pointB
            Vector3 direction = pointB - pointA;
            newObject.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);

            FirewallHit firewallHit = newObject.GetComponent<FirewallHit>();
            if (firewallHit != null)
            {
                firewallHit.Initialize(this.GetComponent<AttackAndPowerCasting>().FireballPrefabName, attackContext); // This caches the name
            }
        }

    }

    public class FirewallAttackContext
    {
        public HashSet<Collider> tickHitEnemies = new HashSet<Collider>();
        public float lastTickTime = -999f;
    }
}
