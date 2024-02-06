using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class EventInteraction : Interactable
{
    [Header("Icon Display")]
    public char iconKey;
    public Sprite icon;

    [Header("Prompt")]
    public string promptText;

    [Header("Interaction")]
    [Space(5)]
    public InputAction inputAction;

    [Space(15)]
    public UnityEvent onInteract;


    [Header("Prompt")]
    public EventPrompt eventPrompt;


    public override void OnGainedFocus()
    {
        inputAction.Enable();
        inputAction.performed += InvokeEvent;
    }


    private void InvokeEvent(InputAction.CallbackContext ctx)
    {
        onInteract.Invoke();
    }


    public override void OnLostFocus()
    {
        inputAction.performed -= InvokeEvent;
        inputAction.Disable();
    }


    public override InteractionPrompt GetPrompt() => eventPrompt;


    public override void DistanceAndAngle(Vector3 camera, Vector3 viewDir, out float distSqr, out float angle)
    {
        Vector3 pos = transform.position;

        Vector3 dirToItem = pos - camera;
        distSqr = dirToItem.magnitude;

        angle = Vector3.Angle(viewDir, dirToItem); 
    }
}
