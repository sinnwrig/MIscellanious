using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class ControllerInput : MonoBehaviour
{

#if ENABLE_INPUT_SYSTEM

    public InputActionAsset inputAsset;

    public string actionMapName = "Movement";
    public string movementBinding = "Move";
    public string lookBinding = "Look";
    public string jumpBinding = "Jump";
    public string sprintBinding = "Run";
    public string crouchBinding = "Crouch";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

#elif ENABLE_LEGACY_INPUT_MANAGER

    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    public KeyCode jump = KeyCode.Space;

    public KeyCode sprint = KeyCode.LeftShift;

    private Vector2 lastMouse;

#endif


    [NonSerialized]
    public Vector2 movement;

    [NonSerialized]
    public Vector2 mouseDelta;

    [NonSerialized]
    public bool isJumping;
    
    [NonSerialized]
    public bool isSprinting;

    [NonSerialized]
    public bool isCrouched;


    private void Awake()
    {
#if ENABLE_INPUT_SYSTEM
        SetupInputSystem();
#endif
    }


#if ENABLE_INPUT_SYSTEM

    private void SetupInputSystem()
    {
        InputActionMap inputMap = inputAsset.FindActionMap(actionMapName, true);

        moveAction = inputMap[movementBinding];
        lookAction = inputMap[lookBinding];
        jumpAction = inputMap[jumpBinding];
        sprintAction = inputMap[sprintBinding];
        crouchAction = inputMap[crouchBinding];

        inputMap.Enable();

        jumpAction.performed += ctx => isJumping = true;

        sprintAction.started += ctx => isSprinting = true;
        sprintAction.canceled += ctx => isSprinting = false; 

        crouchAction.started += ctx => isCrouched = true;
        crouchAction.canceled += ctx => isCrouched = false;
    }

#endif

    public void UpdateInputs()
    {
#if ENABLE_INPUT_SYSTEM
        UpdateInputsNew();
#elif ENABLE_LEGACY_INPUT_MANAGER
        UpdateInputsOld();
#endif
    }


#if ENABLE_INPUT_SYSTEM

    private void UpdateInputsNew()
    {
        movement = moveAction.ReadValue<Vector2>();
        mouseDelta = lookAction.ReadValue<Vector2>();
    }

#elif ENABLE_LEGACY_INPUT_MANAGER

    private void UpdateInputsOld()
    {
        mouseDelta.x = Input.GetAxis("Mouse X");
        mouseDelta.y = Input.GetAxis("Mouse Y");

        movement = Vector2.zero;
        movement.x += Input.GetKey(right) ? 1 : 0;
        movement.x -= Input.GetKey(left) ? 1 : 0;

        movement.y += Input.GetKey(forward) ? 1 : 0;
        movement.y -= Input.GetKey(back) ? 1 : 0;

        isJumping = !isJumping && Input.GetKeyDown(jump);
        isSprinting = Input.GetKey(sprint);
    }

#endif
}
