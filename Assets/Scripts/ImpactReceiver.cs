using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ImpactReceiver : MonoBehaviour
{
    [SerializeField] bool canBeMoved = true;
    private float gravity = -9.81f;
    private Vector3 playerVelocity;
    float mass = 3.0F; // defines the character mass
    Vector3 impact = Vector3.zero;
    private CharacterController character;

    //
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    //
    void Update()
    {
        if (!character.enabled || !gameObject.activeInHierarchy) return;
        if (!character.isGrounded)
        {
            playerVelocity.y += gravity * Time.deltaTime;
            character.Move(playerVelocity * Time.deltaTime);
        }

        // apply the impact force:
        if (impact.magnitude > 0.2F) character.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
    // call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        Debug.Log("AddImpact: " + dir + " " + force);
        if (!canBeMoved) return;
        dir.Normalize();
        dir.y = 0;
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }
}

