using UnityEngine;

public class ThirdPersonController3 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    private bool isJumping;


    public FixedTouchField touchField;
    public JoyStick1 joystick; // Reference to the JoyStick script

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Camera Settings")]
    public float minCameraDistance = 3f;
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float followDistance = 5f;
    public float followHeight = 3f;
    public float cameraSmoothSpeed = 5f;
    public float minYRotation = -35f;
    public float maxYRotation = 60f;

    public Vector3 currentVelocity; // For camera smoothing
    public Vector3 desiredPosition; // Desired camera position
    public float springStrength = 5f; // Spring force
    public float damping = 10f; // Damping to prevent oscillations

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection;
    private bool isGrounded;

    private float cameraPitch = 0f;
    private Vector3 cameraOffset;

    public float scaleFactor = 0.2f;
    public float lookatHeight = 1.5f;

    [Header("Jump Settings")]
    public float jumpCooldown = 1f; // Cooldown duration in seconds
    private float lastJumpTime;

    private Quaternion targetRotation;  // Target rotation for smooth transition
    private float rotationSpeedSmooth = 5f;

    private bool isRotating = false;
    private Vector3 lastMousePosition;

    private Vector3 rotationVelocity = Vector3.zero;
    private float rotationDamping = 5f; // Controls smooth stopping effect

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.None;

        cameraOffset = new Vector3(0, followHeight, -followDistance) * (1f / scaleFactor);
    }

    void Update()
    {
        UpdateAnimator();

        if (touchField.Pressed)
        {
            isRotating = true;
            //lastMousePosition = Input.mousePosition;
            float rotationX = touchField.TouchDist.x * rotationSpeed * Time.deltaTime;
            float rotationY = -touchField.TouchDist.y * rotationSpeed * Time.deltaTime;

            cameraTransform.RotateAround(transform.position, Vector3.up, rotationX);
            cameraPitch = Mathf.Clamp(cameraPitch + rotationY, minYRotation, maxYRotation);
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, cameraTransform.localEulerAngles.y, 0f);

        }
        else
        {
            ApplyCameraDamping(); // Smoothly slow down camera rotation
            HandleMovementInput();
        }
    }

    void FixedUpdate()
    {
        float hori = joystick.InputDirection.x;
        float ver = joystick.InputDirection.y;
        if (hori != 0 || ver != 0)
        {
            UpdateCameraPosition();
            MoveAndRotateCharacter();
        }
    }

    private void HandleMovementInput()
    {
        if (isJumping)
            return;

        float horizontal = joystick.InputDirection.x;
        float vertical = joystick.InputDirection.y;

        float deadZoneThreshold = 0.2f;
        if (Mathf.Abs(horizontal) < deadZoneThreshold && Mathf.Abs(vertical) < deadZoneThreshold)
        {
            moveDirection = Vector3.zero;
            return;
        }

        moveDirection = new Vector3(horizontal, 0f, Mathf.Clamp(vertical, 0, 1)).normalized;
    }

    public void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && Time.time >= lastJumpTime + jumpCooldown)
        {
            animator.SetTrigger("jump");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time; // Update the last jump time

            isJumping = true;
            Invoke("EnableMovement", 2f);
        }
    }

    private void EnableMovement()
    {
        isJumping = false;
    }

    private void MoveAndRotateCharacter()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 movement = cameraTransform.forward * moveDirection.z + cameraTransform.right * moveDirection.x;
            movement.y = 0;

            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    /*private void HandleMouseRotation()
    {
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        float rotationX = mouseDelta.x * rotationSpeed * Time.deltaTime;
        float rotationY = -mouseDelta.y * rotationSpeed * Time.deltaTime;

        // Apply velocity-based rotation
        rotationVelocity = new Vector3(rotationY, rotationX, 0);

        // Rotate the camera around the character
        cameraTransform.RotateAround(transform.position, Vector3.up, rotationVelocity.y);

        // Adjust camera pitch
        cameraPitch = Mathf.Clamp(cameraPitch + rotationVelocity.x, minYRotation, maxYRotation);
        cameraTransform.localEulerAngles = new Vector3(cameraPitch, cameraTransform.localEulerAngles.y, 0f);
    }*/

    private void ApplyCameraDamping()
    {
        if (rotationVelocity.magnitude > 0.01f) // If velocity is still significant
        {
            rotationVelocity = Vector3.Lerp(rotationVelocity, Vector3.zero, rotationDamping * Time.deltaTime);
            cameraTransform.RotateAround(transform.position, Vector3.up, rotationVelocity.y);
            cameraPitch = Mathf.Clamp(cameraPitch + rotationVelocity.x, minYRotation, maxYRotation);
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, cameraTransform.localEulerAngles.y, 0f);
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion targetRotation = Quaternion.Euler(cameraPitch, transform.eulerAngles.y, 0f);
        desiredPosition = transform.position + targetRotation * cameraOffset;

        Vector3 displacement = desiredPosition - cameraTransform.position;
        Vector3 springForce = displacement * springStrength;
        Vector3 dampingForce = -currentVelocity * damping;
        Vector3 acceleration = springForce + dampingForce;
        Vector3 headPosition = transform.position + Vector3.up * lookatHeight;

        currentVelocity += acceleration * Time.fixedDeltaTime;

        cameraTransform.position += currentVelocity * Time.fixedDeltaTime;
        cameraTransform.LookAt(transform.position + Vector3.up * lookatHeight);

        AddCameraTilt();
    }

    private void AddCameraTilt()
    {
        float tiltAmount = moveDirection.magnitude * 5f;
        Quaternion tiltRotation = Quaternion.Euler(cameraPitch - tiltAmount, transform.eulerAngles.y, 0f);
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, tiltRotation, cameraSmoothSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimator()
    {
        float speed = moveDirection.magnitude;
        animator.SetFloat("Speed", speed);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
