using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameObject projectilePrefab; // The prefab for the projectile
    public Transform firePoint; // The point from which the projectile will be fired

    public float projectileSpeed = 10f; // The speed of the projectile
    public float critChance = 0.1f; // The chance of the spell critting (10% in this example)
    public float cooldownTime = 1f;

    private bool isCooldown;
    private Coroutine castSpellCoroutine;

    private PlayerControls controls; // Reference to the Input System controls
    private TwinStickMovement twinStickMovement; // Reference to the Input System controls

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Controls.Attack.started += _ => CastSpell();
        controls.Controls.Attack.canceled += _ => CastSpell();
        twinStickMovement = GetComponent<TwinStickMovement>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    //private void StartCastingSpell()
    //{
    //    // Start the coroutine that casts the spell repeatedly while Fire is held down
    //    castSpellCoroutine = StartCoroutine(CastSpellRepeatedly());
    //}

    private void StopCastingSpell()
    {
        // Stop the coroutine that casts the spell repeatedly
        if (castSpellCoroutine != null)
        {
            StopCoroutine(castSpellCoroutine);
        }
    }

    public void CastSpell()
    {
        Debug.Log("CAST SPELL");
        // instantiate the spell prefab at the current position of the caster
        GameObject spell = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // calculate the direction to the target position and apply the speed to get the velocity
        Vector3 direction = (twinStickMovement.WorldAim - transform.position).normalized;
        Vector3 velocity = direction * projectileSpeed;

        // apply the velocity to the spell's rigidbody component
        Rigidbody rb = spell.GetComponent<Rigidbody>();
    }

    //private IEnumerator CastSpellRepeatedly()
    //{
    //    while (true)
    //    {
    //        // Check if the spell is on cooldown
    //        if (isCooldown)
    //        {
    //            //Debug.Log("The spell is on cooldown!");
    //        }
    //        else
    //        {
    //            Debug.Log("point:" + twinStickMovement.WorldAim);

    //            // Instantiate the projectile prefab at the fire point

    //            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

    //            // Get the Rigidbody component from the projectile
    //            Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();


    //            Vector3 direction = twinStickMovement.WorldAim - this.transform.position + new Vector3(0, 1f, 0);
    //            projectileRigidbody.velocity = direction.normalized * projectileSpeed;


    //            //projectileRigidbody.velocity = projectile.transform.right * projectileSpeed;

    //            // Check if the spell crits
    //            float randomValue = Random.value; // Generate a random value between 0 and 1
    //            if (randomValue < critChance)
    //            {
    //                Debug.Log("The spell crits!");
    //                // Do something special for the crit
    //            }

    //            // Set the spell on cooldown
    //            isCooldown = true;
    //            StartCoroutine(Cooldown());
    //        }

    //        // Wait for a short time before trying to cast the spell again
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}


    private IEnumerator Cooldown()
    {
        // Wait for the cooldown time
        yield return new WaitForSeconds(cooldownTime);
        // Reset the spell cooldown
        isCooldown = false;
    }
}

//using UnityEngine;
//using UnityEngine.InputSystem;

//public class Fireball : MonoBehaviour
//{
//    [SerializeField] private GameObject spellPrefab;
//    [SerializeField] private float spellSpeed = 10f;
//    [SerializeField] private float cooldownTime = 1f;
//    [SerializeField] private float critChance = 0.1f;
//    [SerializeField] private float critMultiplier = 2f;

//    private bool isOnCooldown = false;
//    private PlayerControls controls;

//    private void Awake()
//    {
//        controls = new PlayerControls();
//    }

//    private void OnEnable()
//    {
//        controls.Controls.Attack.performed += CastSpell;
//    }

//    private void OnDisable()
//    {
//        controls.Controls.Attack.performed -= CastSpell;
//    }

//    private void CastSpell(InputAction.CallbackContext context)
//    {
//        Debug.Log("Cast spell!");
//        if (isOnCooldown)
//        {
//            return;
//        }

//        // Instantiate the spell prefab
//        GameObject spell = Instantiate(spellPrefab, transform.position, Quaternion.identity);

//        // Calculate the direction to the target position
//        Vector2 targetPosition = Mouse.current.position.ReadValue();
//        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

//        // Set the spell's velocity to move it in the direction of the target position
//        Rigidbody spellRigidbody = spell.GetComponent<Rigidbody>();
//        if (spellRigidbody != null)
//        {
//            spellRigidbody.velocity = direction * spellSpeed;
//        }

//        // Check if the spell crits
//        float critRoll = Random.value;
//        if (critRoll <= critChance)
//        {
//            Debug.Log("The spell crits!");
//        }

//        // Start cooldown timer
//        isOnCooldown = true;
//        Invoke(nameof(ResetCooldown), cooldownTime);
//    }

//    private void ResetCooldown()
//    {
//        isOnCooldown = false;
//    }
//}



