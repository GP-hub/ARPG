using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Firewall : MonoBehaviour
{
    private Vector3 previousPos;
    private Vector3 currentPos;


    [SerializeField] private string firewallPrefabName;

    private Coroutine firewallCoroutine;

    private void Start()
    {
        EventManager.onDashing += StartCoroutineFirewall;
    }


    private void StartCoroutineFirewall(bool isDashing)
    {
        if (isDashing == true)
        {
            previousPos = this.transform.position;

            firewallCoroutine = StartCoroutine(TrackPosition(isDashing));
        }
        if (isDashing == false)
        {
            StopCoroutine(firewallCoroutine);

            firewallCoroutine = null;
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

        // Calculate the midpoint position
        Vector3 midpoint = (pointA + pointB) / 2;

        // Calculate the distance between pointA and pointB
        float distance = Vector3.Distance(pointA, pointB);

        //if (distance < 1) return;

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
        }

    }
}
