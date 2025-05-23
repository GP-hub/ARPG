using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField;
    [SerializeField] private LayoutElement layoutElement;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = Math.Max(headerField.preferredWidth, contentField.preferredWidth) >= layoutElement.preferredWidth;

    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        Vector3 position = Input.mousePosition;
        Vector2 normalizedPosition = new Vector2(position.x / Screen.width, position.y / Screen.height);

        Vector2 pivot = CalculatePivot(normalizedPosition);

        rectTransform.pivot = pivot;
        transform.position = position;
    }



    private Vector2 CalculatePivot(Vector2 normalizedPosition)
    {
        Vector2 pivotTopLeft = new Vector2(-0.05f, 1.05f);
        Vector2 pivotTopRight = new Vector2(1.05f, 1.05f);
        Vector2 pivotBottomLeft = new Vector2(-0.05f, -0.05f);
        Vector2 pivotBottomRight = new Vector2(1.05f, -0.05f);

        if (normalizedPosition.x < 0.5f && normalizedPosition.y >= 0.5f)
        {
            return pivotTopLeft;
        }
        else if (normalizedPosition.x > 0.5f && normalizedPosition.y >= 0.5f)
        {
            return pivotTopRight;
        }
        else if (normalizedPosition.x <= 0.5f && normalizedPosition.y < 0.5f)
        {
            return pivotBottomLeft;
        }
        else
        {
            return pivotBottomRight;
        }
    }
}
