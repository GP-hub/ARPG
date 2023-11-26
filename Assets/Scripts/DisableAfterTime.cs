using System.Collections;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    [SerializeField]private float disableTime;
    private void OnEnable()
    {
        StartCoroutine(DisableAfterDelay(disableTime));
    }

    IEnumerator DisableAfterDelay(float disableTime)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(disableTime);

        // Disable the GameObject after the specified time
        this.gameObject.SetActive(false);
    }
}
