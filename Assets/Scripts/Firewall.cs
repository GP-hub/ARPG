using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Firewall : MonoBehaviour
{
    private Vector3 previousPos;
    private Vector3 currentPos;

    private List<GameObject> firewallObjectPool = new List<GameObject>();

    [SerializeField] private GameObject firewallPrefab;
    [SerializeField] private float firewallLifetime = 3f;
    private int maxObjectsForPooling = 20;

    private Coroutine firewallCoroutine;

    private void Start()
    {
        EventManager.Instance.onDashing += StartCoroutineFirewall;
        PoolingFirewallObject();
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

            TestFireWall(previousPos, currentPos);

            // Store the current position in previousPosition
            previousPos = currentPos;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void TestFireWall(Vector3 pointA, Vector3 pointB)
    {
        if (pointA == null || pointB == null) return;
        if (pointA == pointB) return;

        // Calculate the midpoint position
        Vector3 midpoint = (pointA + pointB) / 2;

        // Calculate the distance between pointA and pointB
        float distance = Vector3.Distance(pointA, pointB);

        //if (distance < 1) return;

        GameObject newObject = GetPooledFirewallObject(midpoint);

        if (newObject != null)
        {
            newObject.transform.position = midpoint;
            newObject.SetActive(true);

            // Set the box's scale to match the distance between the points
            Vector3 scale = newObject.transform.localScale;
            scale.x = distance;
            newObject.transform.localScale = scale;

            // Rotate the box to point from pointA to pointB
            Vector3 direction = pointB - pointA;
            newObject.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);

            Transform newObjectTransform = newObject.GetComponent<Transform>();
            if (newObjectTransform != null)
            {
                StartCoroutine(DisableFirewallObjectAfterTime(newObject, firewallLifetime));
            }
        }

    }

    private void PoolingFirewallObject()
    {
        for (int i = 0; i < maxObjectsForPooling; i++)
        {
            GameObject newPowerObject = Instantiate(firewallPrefab, Vector3.zero, Quaternion.identity);
            newPowerObject.SetActive(false);
            firewallObjectPool.Add(newPowerObject);
        }
    }


    private GameObject GetPooledFirewallObject(Vector3 pos)
    {
        for (int i = 0; i < firewallObjectPool.Count; i++)
        {
            if (!firewallObjectPool[i].activeInHierarchy)
            {
                return firewallObjectPool[i];
            }
        }

        if (firewallObjectPool.Count < maxObjectsForPooling)
        {
            GameObject newObject = Instantiate(firewallPrefab, pos, Quaternion.identity);
            newObject.SetActive(false);
            firewallObjectPool.Add(newObject);
            return newObject;
        }
        return null;
    }

    private IEnumerator DisableFirewallObjectAfterTime(GameObject objectToDisable, float time)
    {
        yield return new WaitForSeconds(time);
        objectToDisable.SetActive(false);
    }
}
