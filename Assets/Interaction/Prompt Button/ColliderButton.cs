using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Collider))]
public class ColliderButton : Interactable
{
    [Tooltip("Distance before button can be interacted with")]
    public float minDistance = 2.0f;
    public LayerMask hitMask = ~0;


    [Tooltip("The input action that should be performed to trigger the interaction event")]
    public InputAction inputAction;


    [Tooltip("The event to invoke when the input action is started")]
    public UnityEvent onBeginInteract;

    [Tooltip("The event to invoke when the input action is performed")]
    public UnityEvent onInteract;

    [Tooltip("The event to invoke when the input action is canceled")]
    public UnityEvent onCancelInteract;



    private Collider[] _colliders;
    public Collider[] Colliders
    {
        get
        {
            if (_colliders == null)
                _colliders = GetComponents<Collider>();

            return _colliders;
        }
    }


    public override void OnGainedFocus(FocusData data) 
    { 
        inputAction.Enable();
        inputAction.started += BeginInteract;
        inputAction.performed += InvokeEvent;
        inputAction.canceled += EndInteract;
    }


    public override void OnLostFocus() 
    { 
        inputAction.started -= BeginInteract;
        inputAction.performed -= InvokeEvent;
        inputAction.canceled -= EndInteract;
        inputAction.Disable();
    }


    private void InvokeEvent(InputAction.CallbackContext ctx) => onInteract.Invoke();
    private void BeginInteract(InputAction.CallbackContext ctx) => onBeginInteract.Invoke();
    private void EndInteract(InputAction.CallbackContext ctx) => onCancelInteract.Invoke();


    public override bool IsValidFocus(Transform cameraTransform, out float distance)
    {
        distance = Mathf.Infinity;

        hitMask |= 1 << gameObject.layer;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, minDistance, hitMask))
        {
            if (Colliders.Contains(hit.collider))
            {
                distance = hit.distance;
                return true;
            }
        }

        return false;
    }
}
