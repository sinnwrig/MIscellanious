using System;
using UnityEngine;


public abstract class Interactable : MonoBehaviour
{
    private void OnEnable() => InteractionController.Register(this);
    private void OnDisable() => InteractionController.Deregister(this);
    private void OnDestroy() => OnDisable();


    public virtual void OnGainedFocus(FocusData data) { }
    public virtual void OnLostFocus() { }

    public virtual void UpdateState(FocusData data) { }


    /// <summary>
    /// The cumulative distance and angle to the given camera transform
    /// </summary>
    public abstract bool IsValidFocus(Transform cameraTransform, out float distance);
}
