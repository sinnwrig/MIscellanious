using System;
using UnityEngine;


public abstract class Interactable : MonoBehaviour
{
    public float appearanceRange;
    public float interactionRange;


    void OnEnable() => InteractionController.Register(this);
    void OnDisable() => InteractionController.Deregister(this);
    void OnDestroy() => OnDisable();


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, appearanceRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }


    public abstract void OnGainedFocus();
    public abstract void OnLostFocus();

    public abstract InteractionPrompt GetPrompt();


    public abstract void DistanceAndAngle(Vector3 cameraPosition, Vector3 cameraForward, out float distanceToSelf, out float angleToSelf);
}
