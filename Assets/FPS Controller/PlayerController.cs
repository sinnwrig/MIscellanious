using UnityEngine;


[RequireComponent(typeof(CharacterController), typeof(ControllerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Physics Settings")]
    public float gravity = 9.81f;
    public float acceleration = 10.0f;
    public float snapAcceleration = 50.0f;
    public float jumpTimeout = 0.0f;


    [Header("Movement Settings")]
    public float moveSpeed = 3.0f;
    public float sprintSpeed = 6.0f;
    public float jumpHeight = 5.0f;


    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 0.1f;
    public float maxCameraAngle = 81.0f;
    public bool invertX, invertY;


    [Header("Collision Settings")]
    public LayerMask groundMask = ~0;
    public Vector3 groundCheck = new Vector3(0, -0.6f, 0);
    public float groundRadius = 0.5f;

    [Header("Crouching")]
    public float crouchSpeed = 2.0f;
    public float crouchHeight = 1.0f;


    private CharacterController _controller;
    public CharacterController Controller
    {
        get
        {
            if (_controller == null)
                _controller = GetComponent<CharacterController>();
            
            return _controller;
        }
    }


    private ControllerInput _input;
    public ControllerInput Input
    {
        get
        {
            if (_input == null)
                _input = GetComponent<ControllerInput>();
            
            return _input;
        }
    }

    const float terminalVelocity = 53.0f;

    private Vector3 velocity;
    private float jumpTimeoutDelta;

    private float defaultHeight;
    private float cameraHeight;
    private float crouchAnim;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultHeight = Controller.height;
        cameraHeight = cameraTransform.localPosition.y;
    }

    
    private void Update()
    {
        Input.UpdateInputs();

        UpdateRotation();
        
        UpdateMovement();

        JumpAndGravity();

        UpdateCrouchState();
    }


    private void UpdateRotation()
    {
        float lookX = Input.mouseDelta.x * lookSensitivity * (invertX ? -1 : 1);
        float lookY = Input.mouseDelta.y * lookSensitivity * (invertY ? -1 : 1);

        transform.rotation = transform.rotation * Quaternion.AngleAxis(lookX, transform.up);
        
        Quaternion cameraRotation = cameraTransform.localRotation * Quaternion.AngleAxis(lookY, -Vector3.right);

        if (Quaternion.Angle(Quaternion.identity, cameraRotation) < maxCameraAngle) 
            cameraTransform.localRotation = cameraRotation;
    }


    private void UpdateMovement()
    {
        float speed = Input.isSprinting ? sprintSpeed : moveSpeed;

        Vector3 move = new Vector3(Input.movement.x, 0, Input.movement.y);

        move = Vector3.ClampMagnitude(move, 1.0f);
        move = transform.rotation * move * speed;
        move.y = velocity.y;

        bool canSnap = IsGrounded() && (new Vector2(velocity.x, velocity.z).magnitude <= sprintSpeed);        

        velocity = Vector3.MoveTowards(velocity, move, (canSnap ? snapAcceleration : acceleration) * Time.deltaTime);        

        Controller.Move(velocity * Time.deltaTime);
    }


    private static readonly Collider[] colliders = new Collider[2];

    bool IsGrounded() 
    {
        int colCount = Physics.OverlapSphereNonAlloc(transform.TransformPoint(groundCheck), groundRadius, colliders, groundMask);

        if (colCount == 0)
            return false;
        
        // Ensure that the controller does not trigger a false positive
        if (colCount == 1 && colliders[0].gameObject == Controller.gameObject)
            return false;
        
        return true;
    }


    private void JumpAndGravity()
	{
		if (IsGrounded())
        {
			if (velocity.y < 0.0f)
				velocity.y = -2f;

			if (Input.isJumping && jumpTimeoutDelta <= 0.0f)
			{
				velocity.y = jumpHeight;
                jumpTimeoutDelta = jumpTimeout;
                Input.isJumping = false;
			}

			if (jumpTimeoutDelta >= 0.0f)
				jumpTimeoutDelta -= Time.deltaTime;
		}
		else
		{
			// reset the jump timeout timer
			jumpTimeoutDelta = jumpTimeout;
            Input.isJumping = false;
		}

		if (velocity.y < terminalVelocity)
			velocity.y -= gravity * Time.deltaTime;
	}


    private void UpdateCrouchState()
    {
        crouchAnim = Mathf.MoveTowards(crouchAnim, Input.isCrouched ? 1 : 0, Time.deltaTime * crouchSpeed);

        Controller.height = Mathf.Lerp(defaultHeight, crouchHeight, crouchAnim);

        Vector3 center = new Vector3(0, -(crouchHeight * 0.5f), 0);
        Controller.center = Vector3.Lerp(Vector3.zero, center, crouchAnim);

        Vector3 camPos = cameraTransform.localPosition;
        camPos.y = Mathf.Lerp(cameraHeight, cameraHeight - crouchHeight, crouchAnim);
        cameraTransform.localPosition = camPos;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheck), groundRadius);
    }
}
