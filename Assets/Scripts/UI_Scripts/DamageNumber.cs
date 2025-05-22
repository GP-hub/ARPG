using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void Initialize(string value, Color color)
    {
        text.text = value;
        text.color = color;
    }

    public void OnAnimationEnd()
    {
        DamageNumberPool.Instance.ReturnToPool(transform.parent.gameObject);
    }

    public void ResetLocalPosition()
    {
        transform.localPosition = Vector3.zero;
    }

}
