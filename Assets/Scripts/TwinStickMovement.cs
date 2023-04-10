using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class TwinStickMovement : MonoBehaviour
{
    private float gravity = -9.81f;

    [SerializeField] private float playerSpeed = 5f;

    private CharacterController controller;
    private Animator animator;
    private Vector2 movement;
    private Vector2 aim;
    private Vector3 playerVelocity;

    private PlayerControls playerControls;
    //private PlayerInput playerInput;

    //
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        //playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }


    // 
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();

        Vector3 moveDirection = new Vector3(movement.x, 0, movement.y);

        if (moveDirection.magnitude > 0.01f)
        {
            float smoothness = 20.0f;

            float angle = Vector3.SignedAngle(transform.forward, moveDirection.normalized, Vector3.up);
            float targetInputX = Mathf.Sin(angle * Mathf.Deg2Rad);
            float targetInputY = Mathf.Cos(angle * Mathf.Deg2Rad);

            animator.SetFloat("InputX", Mathf.Lerp(animator.GetFloat("InputX"), targetInputX, Time.deltaTime * smoothness));
            animator.SetFloat("InputY", Mathf.Lerp(animator.GetFloat("InputY"), targetInputY, Time.deltaTime * smoothness));
        }
        else
        {
            // Reset the animator parameters to their default values
            animator.SetFloat("InputX", 0);
            animator.SetFloat("InputY", 0);
        }
    }

    private void HandleInput()
    {
        movement = playerControls.Controls.Movements.ReadValue<Vector2>();
        aim = playerControls.Controls.Aim.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(aim);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            if (new Vector3(movement.x, 0, movement.y).magnitude > 0.01f)
            {
                LookAt(point);
            }
        }
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }
}
