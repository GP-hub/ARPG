using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private RectTransform targetCanvas;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Transform objectToFollow;
    [SerializeField] private Image healthbarImage;

    void Update()
    {
        if (objectToFollow != null)
        {
            RepositionHealthBar(objectToFollow);
        }
    }

    public void SetHealthBarData(Transform target, RectTransform healthBarPanel)
    {
        this.targetCanvas = healthBarPanel;
        healthBar = GetComponent<RectTransform>();
        objectToFollow = target;
        RepositionHealthBar(objectToFollow);
        healthBar.gameObject.SetActive(true);
    }

    public void OnHealthChanged(float healthFill)
    {
        healthbarImage.fillAmount = healthFill;

        if (healthbarImage.fillAmount <= 0)
        {
            this.transform.parent.gameObject.SetActive(false);
        }
    }

    private void RepositionHealthBar(Transform objectToFollow)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectToFollow.position + new Vector3(0, 1.8f, 0));
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)));
        //now you can set the position of the ui element
        healthBar.anchoredPosition = WorldObject_ScreenPosition;
    }

}