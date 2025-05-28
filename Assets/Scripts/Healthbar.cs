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
    //[SerializeField] private Image fireChargeBarImage;
    [SerializeField] private Image[] fireChargeImages;

    private bool isPlayerBar;

    private void Start()
    {

    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        if (isPlayerBar) EventManager.onFireChargeCountChange -= UpdateFireChargeBarUI;
    }
    void OnDestroy()
    {
        if (isPlayerBar) EventManager.onFireChargeCountChange -= UpdateFireChargeBarUI;
    }

    void Update()
    {
        if (objectToFollow != null)
        {
            RepositionHealthBar(objectToFollow);
        }
    }
    private void FixedUpdate()
    {

    }

    public void SubscribeToFireChargeChange()
    {
        isPlayerBar = true;
        EventManager.onFireChargeCountChange += UpdateFireChargeBarUI;
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
        if (healthbarImage == null)
        {
            Debug.Log("Healthbar image is null. Skipping health update.");
            return;
        }
        healthbarImage.fillAmount = healthFill;

        if (healthbarImage.fillAmount <= 0)
        {
            this.transform.gameObject.SetActive(false);
        }
    }

    //public void UpdateFireChargeBarUI(int fireChargeFill)
    //{
    //    float normalizedValue = (float)fireChargeFill / (float)SpellCharge.maxFireCharge;
    //    fireChargeBarImage.fillAmount = normalizedValue;
    //}
    public void UpdateFireChargeBarUI(int fireChargeCount)
    {
        for (int i = 0; i < fireChargeImages.Length; i++)
        {
            fireChargeImages[i].enabled = i < fireChargeCount;
        }
    }

    private void RepositionHealthBar(Transform objectToFollow)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectToFollow.position + new Vector3(0, 0, 0));
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)));
        //now you can set the position of the ui element
        healthBar.anchoredPosition = WorldObject_ScreenPosition;
    }

}