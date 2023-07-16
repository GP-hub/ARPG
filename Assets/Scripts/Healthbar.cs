using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Healthbar : MonoBehaviour
{
    #region PRIVATE_VARIABLES
    private Vector2 positionCorrection = new Vector2(0, 100);
    #endregion
    #region PUBLIC_REFERENCES
    public RectTransform targetCanvas;
    public RectTransform healthBar;
    public Vector3 objectToFollow;

    #endregion
    #region PUBLIC_METHODS
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
        healthBar.GetComponent<Image>().fillAmount = healthFill;
    }
    #endregion
    #region UNITY_CALLBACKS
    void Update()
    {
        if (objectToFollow != null)
        {
            RepositionHealthBar();
        }
    }
    #endregion
    #region PRIVATE_METHODS
    private void RepositionHealthBar()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectToFollow);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)));
        //now you can set the position of the ui element
        healthBar.anchoredPosition = WorldObject_ScreenPosition;
    }
    #endregion
}