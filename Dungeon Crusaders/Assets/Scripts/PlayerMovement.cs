using System;
using System.Numerics;
using Shops;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{ 
    public CharacterController controller;
    public Transform cameraTransform;
    public float speed = 6f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    public Animator animator;

    private bool _isGrounded;
    private float _turnSmoothVelocity;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private void Start()
    {
        _isGrounded = false;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        _isGrounded = controller.isGrounded;
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
        {
            animator.SetFloat(Speed, 0);
        }
        
        var direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (!(direction.magnitude >= 0.1f)) return;
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
            turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        var moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        animator.SetFloat(Speed, (moveDirection.normalized * speed).magnitude);

        if (!_isGrounded)
        {
            var downVelocity = Vector3.down;
            downVelocity.y += gravity * Time.deltaTime;
            controller.Move(downVelocity * Time.deltaTime);
        }
    }
}
