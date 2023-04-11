using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class TwinStickMovement : MonoBehaviour
{
    private float gravity = -9.81f;

    [SerializeField] private float playerSpeed = 5f;

    private CharacterController controller;

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;

    private Animator animator;
    private Vector2 movement;
    private Vector2 aim;
    private Vector3 playerVelocity;
    private float smoothnessInputTransition = 20.0f;

    private PlayerControls playerControls;

    //
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerControls = new PlayerControls();


        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;
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
        HandleAnimation();
    }

    private void LateUpdate()
    {
        HandleNavMeshAgentObstacle();
    }

    private void HandleAnimation()
    {
        Vector3 moveDirection = new Vector3(movement.x, 0, movement.y);

        if (moveDirection.magnitude > 0.01f)
        {
            float angle = Vector3.SignedAngle(transform.forward, moveDirection.normalized, Vector3.up);

            float targetInputX = Mathf.Sin(angle * Mathf.Deg2Rad);
            float targetInputY = Mathf.Cos(angle * Mathf.Deg2Rad);

            animator.SetFloat("InputX", Mathf.Lerp(animator.GetFloat("InputX"), targetInputX, Time.deltaTime * smoothnessInputTransition));
            animator.SetFloat("InputY", Mathf.Lerp(animator.GetFloat("InputY"), targetInputY, Time.deltaTime * smoothnessInputTransition));
        }
        else
        {
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

    private void HandleNavMeshAgentObstacle()
    {
        Vector3 moveDirection = new Vector3(movement.x, 0, movement.y);

        if (moveDirection.magnitude > 0.01f)
        {
            agent.enabled = true;
            obstacle.enabled = false;
        }
        else
        {
            agent.enabled = false;
            obstacle.enabled = true;
        }
    }
}
