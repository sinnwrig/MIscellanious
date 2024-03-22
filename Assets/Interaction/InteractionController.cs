using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionController : MonoBehaviour
{
    private static readonly HashSet<Interactable> interactables = new HashSet<Interactable>();

    public static void Register(Interactable item) => interactables.Add(item);
    public static void Deregister(Interactable item) => interactables.Remove(item);


    public Camera controlCamera;

    public Canvas focusCanvas;
    public bool preferKeyboard;


    [HideInInspector]
    public Interactable target;

    public System.Action onTargetUpdated;


    private FocusData FocusData => new FocusData 
    { 
        camera = controlCamera, 
        canvas = focusCanvas, 
        preferKeyboard = preferKeyboard 
    };


    private IEnumerator Start() 
    {
        SetTarget(null);

        yield return UpdateInteractables();
    }


    private IEnumerator UpdateInteractables()
    {
        var wait = new WaitForSeconds(0.1f);

        Loop:

        if (target != null)
            target.UpdateState(FocusData);

        SetTarget(FindClosestInteractable());

        yield return wait;            
        
        goto Loop;
    }


    private void Update()
    {
        if (target != null)
            target.UpdateState(FocusData);
    }


    private Interactable FindClosestInteractable()
    {
        Interactable closest = null;

        float closestDistance = Mathf.Infinity;

        foreach (Interactable item in interactables)
        {
            if (!item.IsValidFocus(controlCamera.transform, out float dist))
                continue;

            if (dist > closestDistance)
                continue;
            
            closestDistance = dist;
            closest = item;
        }

        return closest;
    }


    private void SetTarget(Interactable target)
    {
        if (this.target == target)
            return;

        if (this.target != null)
            this.target.OnLostFocus();
        
        this.target = target;

        if (this.target != null)
            this.target.OnGainedFocus(FocusData);

        onTargetUpdated?.Invoke();
    }
}
