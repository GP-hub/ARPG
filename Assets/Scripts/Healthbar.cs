using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private RectTransform targetCanvas;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Vector3 objectToFollow;
    [SerializeField] private Image healthbarImage;

    public void SetHealthBarData(Vector3 targetTransform, RectTransform healthBarPanel)
    {
        this.targetCanvas = healthBarPanel;
        healthBar = GetComponent<RectTransform>();
        objectToFollow = targetTransform;
        RepositionHealthBar();
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

    void Update()
    {
        if (objectToFollow != null)
        {
            RepositionHealthBar();
        }
    }

    private void RepositionHealthBar()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectToFollow);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)));
        //now you can set the position of the ui element
        healthBar.anchoredPosition = WorldObject_ScreenPosition;
    }

}