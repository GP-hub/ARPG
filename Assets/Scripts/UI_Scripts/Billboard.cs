using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // Match camera rotation
        transform.LookAt(transform.position + cam.forward);
    }
}
