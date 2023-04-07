using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementControler : MonoBehaviour
{
    Player_inputs playerInputs;

    CharacterController characterController;
    Animator animator;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    float rotationFactorPerFrame = 15.0f;

    //
    private void Awake()
    {
        playerInputs = new Player_inputs();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInputs.CharacterControls.Move.started += OnMovementInput;
        playerInputs.CharacterControls.Move.canceled += OnMovementInput;
        playerInputs.CharacterControls.Move.performed += OnMovementInput;
    }

    // 
    void Update()
    {
        HandleRotation();
        HandleAnimation();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    //
    private void OnMovementInput (InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

    }

    //
    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    //
    void HandleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
    }

    //
    private void OnEnable()
    {
        playerInputs.CharacterControls.Enable();
    }

    //
    private void OnDisable()
    {
        playerInputs.CharacterControls.Disable();

    }
}
