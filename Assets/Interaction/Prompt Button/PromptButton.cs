using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PromptButton : Interactable
{
    [Tooltip("Distance before button prompt is displayed")]
    public float displayDistance = 3.0f; 

    [Tooltip("Distance before button can be interacted with")]
    public float interactDistance = 2.0f;

    [Tooltip("Minimum angle from screen center button must be before it can be interacted with")]
    public float interactAngle = 10.0f;


    [Tooltip("Input icon to display when prompting user\nWill be used if character is empty and does not prefer keyboard")]
    public Sprite inputIcon;

    [Tooltip("Input character to display when prompting user\nWill be used if character is not empty and prefers keyboard")]
    public char inputChar;

    [Tooltip("Text description to display below input prompt")]
    public string promptText;

    [Tooltip("Duration that prompt should last if input action must be held")]
    public float inputDuration = 0.0f;

    [Tooltip("The input action that should be performed to trigger the interaction event")]
    public InputAction inputAction;

    [Tooltip("The event to invoke when the input action is performed")]
    public UnityEvent onInteract;

    [Tooltip("The UI prefab to use when displaying a prompt to the user")]
    public PromptIcon promptIconPrefab;


    private PromptIcon iconInstance;
    private bool canInteract;


    public override void OnGainedFocus(FocusData data) 
    { 
        iconInstance = Instantiate(promptIconPrefab, data.canvas.transform);
        iconInstance.ActivatePrompt(inputIcon, inputChar, promptText, data.preferKeyboard);
    }


    public override void OnLostFocus() 
    { 
        OnLeaveInteract();
        iconInstance.DestroyPrompt();
    }


    private void InvokeEvent(InputAction.CallbackContext ctx) 
    {
        iconInstance.OnEndInteract();
        onInteract.Invoke();
    }
    
    private void BeginInteract(InputAction.CallbackContext ctx) => iconInstance.OnBeganInteract(inputDuration);
    private void EndInteract(InputAction.CallbackContext ctx) => iconInstance.OnEndInteract();


    public override void UpdateState(FocusData data) 
    {
        Transform cameraTransform = data.camera.transform;
        Vector3 toObject = transform.position - cameraTransform.position;

        float distance = toObject.magnitude;
        float angle = Vector3.Angle(toObject, cameraTransform.forward);

        iconInstance.UpdateIcon(data.camera, transform.position);

        if (distance < interactDistance && angle < interactAngle)
            OnEnterInteract();
        else
            OnLeaveInteract();
    }


    private void OnEnterInteract()
    {
        if (canInteract)
            return;
        
        canInteract = true;
        
        inputAction.Enable();
        inputAction.started += BeginInteract;
        inputAction.performed += InvokeEvent;
        inputAction.canceled += EndInteract;
        iconInstance.SetInteractionState(true);
    }


    private void OnLeaveInteract()
    {
        if (!canInteract)
            return;
        
        iconInstance.OnEndInteract();
        canInteract = false;

        iconInstance.SetInteractionState(false);
        inputAction.started -= BeginInteract;
        inputAction.performed -= InvokeEvent;
        inputAction.canceled -= EndInteract;
        inputAction.Disable();
    }


    public override bool IsValidFocus(Transform cameraTransform, out float distance)
    {
        Vector3 toObject = transform.position - cameraTransform.position;

        distance = toObject.magnitude;

        bool canDisplay = distance < displayDistance;

        float angle = Vector3.Angle(toObject, cameraTransform.forward);

        distance += Mathf.Max(0, angle - interactAngle);

        return canDisplay && angle < 90.0f;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, displayDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
