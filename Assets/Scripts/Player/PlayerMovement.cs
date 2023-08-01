using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed; // Максимальная скорость передвижения
    public float walkSpeed; // Скорость ходьбы
    public float sprintSpeed; // Скорость бега
    
    public float groundDrag; // Трение о землю

    [Header("Jumping")]
    public float jumpForce; // Сила прыжка
    public float jumpCooldown; // Задержка до следующего прыжка
    public float airMultiplier; // Множитель силы в воздухе
    public bool readyToJump; // Готовность прыжка

    [Header("Crouching")]
    public float crouchSpeed; // Скорость в приседи
    public float crouchYScale; // Величина сжатия в приседи
    private float startYScale; // Исходная величина сжатия

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space; // Клавиша прыжка
    public KeyCode sprintKey = KeyCode.LeftShift; // Клавиша бега
    public KeyCode crouchKey = KeyCode.LeftControl; // Клавиша приседа

    [Header("Ground Check")]
    public float playerHeight; // Высота игрока
    public LayerMask whatIsGround; // Тег земли
    public bool grounded; // Проверка нахожденяи на земле

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation; // Угол поворота в пространстве

    float horizontalInput; // Движение вправо-влево
    float verticalInput; // Движение вперед-назад

    Vector3 moveDirection; // Направление движения

    Rigidbody rb;   // Физика игрока

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

    }

    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // When to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // Stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - Crouching 
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
            return;
        }
        // Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            return;
        }
        // Mode - Walking
        if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            return;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
            return;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // On ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        // In air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // Turn gravity off while on slope
        rb.useGravity = !OnSlope();

    }

    private void SpeedControl()
    { 
        // Limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        // Limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        
    }

    private void Jump()
    {
        exitingSlope = true;
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
