using UnityEngine;

public class ClickToRotate : MonoBehaviour
{
    [SerializeField]private float rotationSpeed;
    private bool isDragging = false;

    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void Update()
    {
        if (isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, -mouseX * rotationSpeed * Time.deltaTime);
        }
    }
}
